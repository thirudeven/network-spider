using System;
using System.Collections.Generic;
using System.Text;

using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Services.Core.Lifecycle;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Lifecycle;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class LifecycleServiceDemo : DemoBase
    {
        private ILifecycleService lifecycleService;

        public LifecycleServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            lifecycleService =
                serviceFactory.GetRemoteService<ILifecycleService>(DemoServiceContext);
        }

        public void AttachLifecycle(ObjectIdentity objId, ObjectIdentity policyId, String aliasSetName)
        {
            AttachLifecycleInfo attachLcInfo = new AttachLifecycleInfo();

            // this must be the name of an alias set listed in
            // the alias_set_ids attribute of the dm_policy (lifecycle) object
            // if null or empty the content server will set the policy scope
            attachLcInfo.PolicyScope = aliasSetName;

            attachLcInfo.PolicyId = policyId;
            attachLcInfo.ObjectId = objId;
            OperationOptions operationOptions = null;
            List<AttachLifecycleInfo> attachLcInfoList = new List<AttachLifecycleInfo>();
            attachLcInfoList.Add(attachLcInfo);

            lifecycleService.Attach(attachLcInfoList, operationOptions);
        }

        public void ShowLifecycleInfo(ObjectIdentity objectIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet(objectIdentity);
            List<LifecycleInfo> lcInfoList = lifecycleService.GetLifecycle(objIdSet, null);
            LifecycleInfo lcInfo = lcInfoList[0];
            // if no lifecycle is attached, the policyId will have the null id
            if (lcInfo.PolicyId.GetValueAsString().Equals("0000000000000000"))
            {
                Console.WriteLine("No lifecycle attached to object.");
            }
            else
            {
                Console.WriteLine("Lifecycle name is " + lcInfo.PolicyName);
                Console.WriteLine("Current state is " + lcInfo.StateName);
                Console.WriteLine("Available operations are:");
                foreach (LifecycleOperation lcOperation in lcInfo.EnabledOperations)
                {
                    Console.WriteLine("    " + lcOperation.Name);
                }
            }
        }

        public void PromoteLifecycle(ObjectIdentity objectIdentity)
        {
            LifecycleOperation lifecycleOperation = new LifecycleOperation();
            lifecycleOperation.Name = LifecycleOperation.PROMOTE;
            lifecycleOperation.Label = "Promote";
            lifecycleOperation.ObjectId = objectIdentity;

            List<LifecycleOperation> lcOperationsList = new List<LifecycleOperation>();
            lcOperationsList.Add(lifecycleOperation);

            OperationOptions operationOptions = null;
            lifecycleService.Execute(lcOperationsList, operationOptions);
        }

        public void DemoteLifecycleToBase(ObjectIdentity objectIdentity)
        {
            LifecycleOperation lifecycleOperation = new LifecycleOperation();
            lifecycleOperation.Name = LifecycleOperation.DEMOTE;
            lifecycleOperation.Label = "Demote";
            lifecycleOperation.ObjectId = objectIdentity;

            LifecycleExecutionProfile lcExecProfile = new LifecycleExecutionProfile();
            lcExecProfile.ResetToBase = true;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.Profiles.Add(lcExecProfile);

            List<LifecycleOperation> lcOperationsList = new List<LifecycleOperation>();
            lcOperationsList.Add(lifecycleOperation);

            lifecycleService.Execute(lcOperationsList, operationOptions);
        }

        public void DetachLifecycle(ObjectIdentity objectIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet(objectIdentity);
            OperationOptions operationOptions = null;
            lifecycleService.Detach(objIdSet, operationOptions);
        }

        public DataPackage GetLifecycles(String repository)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            IQueryService queryService = null;

            queryService = serviceFactory
                .GetRemoteService<IQueryService>(lifecycleService.GetServiceContext());

            PassthroughQuery query = new PassthroughQuery();

            // this query does not necessarily contain everything you will need
            // but shows most of the lifecycle-related properties on dm_policy
            query.QueryString = "select r_object_id, " +
                    "object_name, " +
                    "acl_name, " +
                    "included_type, " +
                    "include_subtypes, " +
                    "state_name, " +
                    "state_description, " +
                    "state_class, " +
                    "r_resume_state, " +
                    "r_current_state, " +
                    "entry_criteria_id, " +
                    "user_criteria_id, " +
                    "action_object_id, " +
                    "user_action_id, " +
                    "exception_state, " +
                    "allow_attach, " +
                    "allow_schedule, " +
                    "return_to_base, " +
                    "allow_demote, " +
                    "alias_set_ids, " +
                    "return_condition " +
                    "from dm_policy";
            query.AddRepository(repository);
            QueryExecution queryEx = new QueryExecution();
            queryEx.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;
            queryEx.MaxResultCount = 50;
            OperationOptions operationOptions = null;
            Console.WriteLine("Executing query " + query.QueryString);
            QueryResult queryResult = queryService.Execute(query, queryEx,
                    operationOptions);
            return queryResult.DataPackage;
        }
    }
}

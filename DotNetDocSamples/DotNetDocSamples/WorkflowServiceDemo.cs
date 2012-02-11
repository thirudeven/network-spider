using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Services.Bpm;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Bpm;

using System;
using System.Collections.Generic;
using System.Text;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class WorkflowServiceDemo : DemoBase
    {
        private IWorkflowService workflowService;

        public WorkflowServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            workflowService =
                serviceFactory.GetRemoteService<IWorkflowService>(DemoServiceContext, "bpm");
        }

        public DataPackage processTemplates()
        {
            DataPackage processTemplates
                = workflowService.GetProcessTemplates(DefaultRepository,
                                                      null,
                                                      "object_name");
            foreach (DataObject dObj in processTemplates.DataObjects)
            {
                Console.WriteLine(dObj.Identity.GetValueAsString());
                Console.WriteLine(dObj.Properties.Get("object_name"));
            }

            return processTemplates;
        }

        public ProcessInfo processInfo(ObjectIdentity processId)
        {
            try
            {
                ProcessInfo processInfo = workflowService.GetProcessInfo(processId);

                Console.WriteLine("Process template "
                                   + processId.GetValueAsString());
                Console.WriteLine("Name is " + processInfo.ProcessInstanceName);
                Console.WriteLine("isAliasAssignmentRequired == "
                                   + processInfo.IsAliasAssignmentRequired());
                Console.WriteLine("isPerformerAssignmentRequired == "
                                   + processInfo.IsPerformerAssignmentRequired());
                return processInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw new Exception(e.Message);
            }
        }

        public void startProcess(String processId,
                             String processName,
                             String supervisor,
                             ObjectId wfAttachment,
                             List<ObjectId> docIds,
                             String noteText,
                             String userName,
                             String groupName,
                             String queueName)
        {
            // get the template ProcessInfo
            ObjectId objId = new ObjectId(processId);
            ProcessInfo info = workflowService
                .GetProcessInfo(new ObjectIdentity(objId, DefaultRepository));

            // set specific info for this workflow
            info.Supervisor = supervisor;
            info.ProcessInstanceName = processName + new DateTime();

            // workflow attachment
            info.AddWorkflowAttachment("dm_sysobject", wfAttachment);

            // packages
            List<ProcessPackageInfo> pkgList = info.Packages;
            foreach (ProcessPackageInfo pkg in pkgList)
            {
                pkg.AddDocuments(docIds);
                pkg.AddNote("note for " + pkg.PackageName + " " + noteText, true);
            }

            // alias
            if (info.IsAliasAssignmentRequired())
            {
                List<ProcessAliasAssignmentInfo> aliasList
                    = info.AliasAssignments;
                foreach (ProcessAliasAssignmentInfo aliasInfo in aliasList)
                {
                    String aliasName = aliasInfo.AliasName;
                    String aliasDescription = aliasInfo.AliasDescription;
                    int category = aliasInfo.AliasCategory;
                    if (category == 1) // User
                    {
                        aliasInfo.AliasValue = userName;
                    }
                    else if (category == 2 || category == 3) // group, user or group
                    {
                        aliasInfo.AliasValue = groupName;
                    }

                    Console.WriteLine("Set alias: "
                                       + aliasName
                                       + ", description: "
                                       + aliasDescription
                                       +  ", category: "
                                       + category
                                       + " to "
                                       + aliasInfo.AliasValue);
                }
            }

            // Performer.
            if (info.IsPerformerAssignmentRequired())
            {
                List<ProcessPerformerAssignmentInfo> perfList
                    = info.PerformerAssignments;
                foreach (ProcessPerformerAssignmentInfo perfInfo in perfList)
                {
                    int category = perfInfo.Category;
                    int perfType = perfInfo.PerformerType;
                    String name = "";
                    List<String> nameList = new List<String>();
                    if (category == 0) // User
                    {
                        name = userName;
                    }
                    else if (category == 1 || category == 2) // Group, user or group
                    {
                        name = groupName;
                    }
                    else if (category == 4)     // work queue
                    {
                        name = queueName;
                    }
                    nameList.Add(name);
                    perfInfo.Performers = nameList;

                    Console.WriteLine("Set performer perfType: " + perfType +
                                       ", category: " + category + " to " + name);
                }
            }

            ObjectIdentity wf = workflowService.StartProcess(info);
            Console.WriteLine("started workflow: " + wf.GetValueAsString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;


namespace HelloDFS
{
    /**
    * This class demonstrates how to code a typical request to a DFS core service(in 
    * this case QueryService). The code goes through the steps of creating a
    * ServiceContext, which contains repository and credential information, creating
    * a profile, which contains properties that determine how a service operation 
    * behaves, and  calling the service with the profile.
    * 
    * This sample assumes that you have a working installation
    * of DFS that points to a working Content Server. 
    *
    */


    class QueryServiceTest
    {
        /************************************************************
         * You must supply valid values for the following fields: */

        /* The repository that you want to run the query on */
        private String repository = "MOH_TST";

        /* The username to login to the repository */
        private String userName = "dmadmin";

        /* The password for the username */
        private String password = "dm.admin";

        /* The address where the DFS services are located */
        private String address = "http://192.168.200.9:9080/services";

        /***********************************************************/

        /* The module name for the DFS core services */
        private static String moduleName = "core";
        private IServiceContext serviceContext;

        public void setContext()
        {
            /* 
             * Get the service context and set the user 
             * credentials and repository information
             */
            ContextFactory contextFactory = ContextFactory.Instance;
            serviceContext = contextFactory.NewContext();
            RepositoryIdentity repositoryIdentity =
                new RepositoryIdentity(repository, userName, password, "");
            serviceContext.AddIdentity(repositoryIdentity);
        }

        /*
         * Demonstrates a typical scenario for calling the query service.
         * Gets an instance of the Query service and calls the execute operation
         * with a hard-coded query and operation options.
         */
        public void callQueryService()
        {
            /*
             * Get an instance of the QueryService by passing 
             * in the service context to the service factory.
             */
            try
            {

                ServiceFactory serviceFactory = ServiceFactory.Instance;
                IQueryService querySvc = serviceFactory.GetRemoteService<IQueryService>(serviceContext, moduleName, address);

                /*
                * Construct the query and the QueryExecution options
                */
                PassthroughQuery query = new PassthroughQuery();
                query.QueryString = "select r_object_id, object_name from dm_cabinet";
                query.AddRepository(repository);
                QueryExecution queryEx = new QueryExecution();
                queryEx.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

                /*
                 * Execute the query passing in the operation options and print the result
                 */
                OperationOptions operationOptions = null;
                QueryResult queryResult = querySvc.Execute(query, queryEx, operationOptions);
                Console.WriteLine("QueryId == " + query.QueryString);
                Console.WriteLine("CacheStrategyType == " + queryEx.CacheStrategyType);
                DataPackage resultDp = queryResult.DataPackage;
                List<DataObject> dataObjects = resultDp.DataObjects;
                int numberOfObjects = dataObjects.Count;
                Console.WriteLine("Total objects returned is: " + numberOfObjects);
                foreach (DataObject dObj in dataObjects)
                {
                    PropertySet docProperties = dObj.Properties;
                    String objectId = dObj.Identity.GetValueAsString();
                    String docName = docProperties.Get("object_name").GetValueAsString();
                    Console.WriteLine("Document " + objectId + " name is " + docName);
                }
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
        }

        public static void Main(String[] args)
        {
            Console.WriteLine("Starting.....");
            QueryServiceTest t = new QueryServiceTest();
            t.setContext();
            t.callQueryService();
        }
    }
}
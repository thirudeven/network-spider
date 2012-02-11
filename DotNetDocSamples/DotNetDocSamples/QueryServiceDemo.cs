using System;
using System.Collections.Generic;
using System.Text;

using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class QueryServiceDemo : DemoBase
    {
        private IQueryService queryService;

        public QueryServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            queryService =
                serviceFactory.GetRemoteService<IQueryService>(DemoServiceContext);
        }

        public void BasicPassthroughQuery()
        {
            PassthroughQuery query = new PassthroughQuery();
            query.QueryString = "select r_object_id, "
                                + "object_name from dm_cabinet";
            query.AddRepository(DefaultRepository);
            QueryExecution queryEx = new QueryExecution();
            queryEx.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;
            OperationOptions operationOptions = null;
            QueryResult queryResult = queryService.Execute(query, queryEx, operationOptions);
            Console.WriteLine("QueryId == " + query.QueryString);
            Console.WriteLine("CacheStrategyType == " + queryEx.CacheStrategyType);
            DataPackage resultDp = queryResult.DataPackage;
            List<DataObject> dataObjects = resultDp.DataObjects;
            Console.WriteLine("Total objects returned is: " + dataObjects.Count);
            foreach (DataObject dObj in dataObjects)
            {
                PropertySet docProperties = dObj.Properties;
                String objectId = dObj.Identity.GetValueAsString();
                String docName = docProperties.Get("object_name").GetValueAsString();
                Console.WriteLine("Document " + objectId + " name is " + docName);
            }
        }

        /*
        *    Sequentially processes a cached query result
        *    Terminates when end of query result is reached
        */
        public void CachedPassthroughQuery()
        {
            PassthroughQuery query = new PassthroughQuery();
            query.QueryString = "select r_object_id, "
                                + "object_name from dm_cabinet";
            Console.WriteLine("Query string is " + query.QueryString);
            query.AddRepository(DefaultRepository);
            QueryExecution queryEx = new QueryExecution();
            OperationOptions operationOptions = null;
            queryEx.CacheStrategyType = CacheStrategyType.BASIC_FILE_CACHE_STRATEGY;
            Console.WriteLine("CacheStrategyType == " + queryEx.CacheStrategyType);
            queryEx.MaxResultCount = 10;
            Console.WriteLine("MaxResultCount = " + queryEx.MaxResultCount);

            while (true)
            {
                QueryResult queryResult = queryService.Execute(query, queryEx,
                                                           operationOptions);
                DataPackage resultDp = queryResult.DataPackage;
                List<DataObject> dataObjects = resultDp.DataObjects;
                if (dataObjects.Count == 0)
                {
                    break;
                }
                Console.WriteLine("Total objects returned is: " + dataObjects.Count);
                foreach (DataObject dObj in dataObjects)
                {
                    PropertySet docProperties = dObj.Properties;
                    String objectId = dObj.Identity.GetValueAsString();
                    String cabinetName =
                        docProperties.Get("object_name").GetValueAsString();
                    Console.WriteLine("Cabinet " + objectId + " name is "
                                       + cabinetName);
                }
                queryEx.StartingIndex += 10;
            }
        }
    }
}

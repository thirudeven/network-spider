using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.Services.Search;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

using System;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.FS.DataModel.Core.Profiles;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class SearchServiceDemo: DemoBase
    {
        private ISearchService searchService;
        
        public SearchServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            searchService =
                serviceFactory.GetRemoteService<ISearchService>(DemoServiceContext, "search");
        }

        public List<Repository> RepositoryList()
        {
            List<Repository> repositoryList = searchService.GetRepositoryList(null);
            foreach (Repository r in repositoryList)
            {
                Console.WriteLine(r.Name);
            }
            return repositoryList;
        }

        public QueryResult SimplePassthroughQuery()
        {
            QueryResult queryResult;
            string queryString = "select distinct r_object_id from dm_document order by r_object_id ";
            int startingIndex = 0;
            int maxResults = 60;
            int maxResultsPerSource = 20;

            PassthroughQuery q = new PassthroughQuery();
            q.QueryString = queryString;
            q.AddRepository(DefaultRepository);

            QueryExecution queryExec = new QueryExecution(startingIndex,
                                                          maxResults,
                                                          maxResultsPerSource);
            queryExec.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;

            queryResult = searchService.Execute(q, queryExec, null);

            QueryStatus queryStatus = queryResult.QueryStatus;
            RepositoryStatusInfo repStatusInfo = queryStatus.RepositoryStatusInfos[0];
            if (repStatusInfo.Status == Status.FAILURE)
            {
                Console.WriteLine(repStatusInfo.ErrorTrace);
                throw new Exception("Query failed to return result.");
            }
            Console.WriteLine("Query returned result successfully.");
            DataPackage dp = queryResult.DataPackage;
            Console.WriteLine("DataPackage contains " + dp.DataObjects.Count + " objects.");
            foreach (DataObject dataObject in dp.DataObjects)
            {
                Console.WriteLine(dataObject.Identity.GetValueAsString());
            }
            return queryResult;
        }

        public void SimpleStructuredQuery(String docName)
        {
            String repoName = DefaultRepository;

            PropertyProfile propertyProfile = new PropertyProfile();
            propertyProfile.FilterMode = PropertyFilterMode.IMPLIED;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.Profiles.Add(propertyProfile);

            // Create query
            StructuredQuery q = new StructuredQuery();
            q.AddRepository(repoName);
            q.ObjectType = "dm_document";
            q.IsIncludeHidden = true;
            q.IsDatabaseSearch = true;
            ExpressionSet expressionSet = new ExpressionSet();
            expressionSet.AddExpression(new PropertyExpression("object_name",
                                                               Condition.CONTAINS,
                                                               docName));
            q.RootExpressionSet = expressionSet;

            // Execute Query
            int startingIndex = 0;
            int maxResults = 60;
            int maxResultsPerSource = 20;
            QueryExecution queryExec = new QueryExecution(startingIndex,
                                                          maxResults,
                                                          maxResultsPerSource);
            QueryResult queryResult = searchService.Execute(q, queryExec, operationOptions);

            QueryStatus queryStatus = queryResult.QueryStatus;
            RepositoryStatusInfo repStatusInfo = queryStatus.RepositoryStatusInfos[0];
            if (repStatusInfo.Status == Status.FAILURE)
            {
                Console.WriteLine(repStatusInfo.ErrorTrace);
                throw new Exception("Query failed to return result.");
            }
            Console.WriteLine("Query returned result successfully.");

            // print results
            Console.WriteLine("DataPackage contains " + queryResult.DataObjects.Count + " objects.");
            foreach (DataObject dataObject in queryResult.DataObjects)
            {
                Console.WriteLine(dataObject.Identity.GetValueAsString());
            }
        }
    }

}

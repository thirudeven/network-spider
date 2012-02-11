using System;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Schema;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Services.Core;


namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class SchemaServiceDemo : DemoBase
    {
        private ISchemaService schemaService;

        public SchemaServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            schemaService =
                serviceFactory.GetRemoteService<ISchemaService>(DemoServiceContext);
        }

        public RepositoryInfo RepositoryInfoDemo()
        {
            OperationOptions operationOptions = new OperationOptions();
            RepositoryInfo repositoryInfo = schemaService.GetRepositoryInfo(DefaultRepository, 
                                                                            operationOptions);
 
            Console.WriteLine(repositoryInfo.Name);
            Console.WriteLine("Default schema name: " + repositoryInfo.DefaultSchemaName);
            Console.WriteLine("Label: " + repositoryInfo.Label);
            Console.WriteLine("Description: " + repositoryInfo.Description);
            Console.WriteLine("Schema names:");
            List<String> schemaList = repositoryInfo.SchemaNames;
            foreach (String schemaName in schemaList)
            {
                Console.WriteLine(schemaName);
            }
            return repositoryInfo;
        }

        public TypeInfo TypeInfoDemo()
        {
            SchemaProfile schemaProfile = new SchemaProfile();
            schemaProfile.IncludeProperties = true;
            schemaProfile.IncludeValues = true;
            DemoServiceContext.SetProfile(schemaProfile);

            OperationOptions operationOptions = null;
            TypeInfo typeInfo = schemaService.GetTypeInfo(DefaultRepository,
                                                      null,
                                                      "dm_document",
                                                      operationOptions);

            Console.WriteLine("Name: " + typeInfo.Name);
            Console.WriteLine("Label: " + typeInfo.Label);
            Console.WriteLine("Description: " + typeInfo.Description);
            Console.WriteLine("Parent name : " + typeInfo.ParentName);
            List<PropertyInfo> propertyInfoList;
            propertyInfoList = typeInfo.PropertyInfos;
            Console.WriteLine("Properties: ");
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                Console.WriteLine("  " + propertyInfo.Name);
                Console.WriteLine("  " + propertyInfo.DataType.ToString());
            }
            return typeInfo;
        }

        public PropertyInfo PropertyInfoDemo()
        {
            OperationOptions operationOptions = null;
            PropertyInfo propertyInfo = schemaService.GetPropertyInfo(DefaultRepository,
                                                                  null,
                                                                  "dm_document",
                                                                  "subject",
                                                                  operationOptions);
            Console.WriteLine("Getting property info for property 'subject' of dm_document.");
            Console.WriteLine("Name: " + propertyInfo.Name);
            Console.WriteLine("Label: " + propertyInfo.Label);
            Console.WriteLine("Description: " + propertyInfo.Description);

            return propertyInfo;
        }

        public void ValueInfoDemo()
        {
            SchemaProfile schemaProfile = new SchemaProfile();
            schemaProfile.IncludeValues = true;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.SchemaProfile = schemaProfile;

            Console.WriteLine("Printing value info:");
            ValueAssist valueAssist = schemaService.GetDynamicAssistValues(DefaultRepository,
                                                                       null,
                                                                       "dm_document",
                                                                       "subject",
                                                                       null,
                                                                       operationOptions);
            if (valueAssist == null)
            {
                Console.WriteLine("valueAssist is null.");
                return;
            }
            foreach (String value in valueAssist.Values)
            {
                Console.WriteLine("  " + value);
            }
        }
    }
}

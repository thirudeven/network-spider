using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Acl;
using Emc.Documentum.FS.Services.Core.Acl;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Properties;

namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class SampleContentManager
    {
        public SampleContentManager(DemoBase serviceDemo)
        {
            this.serviceDemo = serviceDemo;
            createdObjectIdentities = new ObjectIdentitySet();
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            objectService =
                serviceFactory.GetRemoteService<IObjectService>(serviceDemo.DemoServiceContext);
            aclService = serviceFactory.GetRemoteService<IAccessControlService>(serviceDemo.DemoServiceContext);
            setSampleFolderRelationship();
        }

        private void setSampleFolderRelationship()
        {
            ObjectPath objectPath = new ObjectPath(sourcePath);
            ObjectIdentity sampleFolderIdentity = new ObjectIdentity(objectPath, serviceDemo.DefaultRepository);
            sampleFolderRelationship = new ReferenceRelationship();
            sampleFolderRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            sampleFolderRelationship.Target = sampleFolderIdentity;
            sampleFolderRelationship.TargetRole = Relationship.ROLE_PARENT;
        }

        public void DeleteTestCabinet(String repository)
        {
            if (!isDataCleanedUp)
            {
                Console.WriteLine("Test cabinet, folders, and sample images will not be deleted because SampleContentManager.isDataCleanedUp = false");
                return;
            }
            DeleteProfile deleteProfile = new DeleteProfile();
            deleteProfile.IsDeepDeleteFolders = true;
            deleteProfile.IsDeepDeleteChildrenInFolders = true;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.DeleteProfile = deleteProfile;

            ObjectPath objectPath = new ObjectPath(testCabinetPath);
            ObjectIdentity sampleCabinetIdentity = new ObjectIdentity(objectPath, repository);
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.AddIdentity(sampleCabinetIdentity);

            objectService.Delete(objIdSet, operationOptions);
            Console.WriteLine("Deleted test cabinet " + testCabinetPath + " on " + repository);
        }

        public void DeleteTestCabinet()
        {
            DeleteTestCabinet(serviceDemo.DefaultRepository);
        }

        public void DeleteSecondaryTestCabinet()
        {
            DeleteTestCabinet(serviceDemo.SecondaryRepository);
        }

        /*
       *  Deletes all objects in createdObjectIdentities
       *  This is intended for cleanup after completed test
       * */
        public void DeleteCreatedObjects()
        {
            if (!isDataCleanedUp)
            {
                Console.WriteLine("Data created during test will not be deleted because SampleContentManager.isDataCleanedUp = false");
                return;
            }
            if (createdObjectIdentities == null)
            {
                return;
            }
            try
            {
                DeleteProfile deleteProfile = new DeleteProfile();
                deleteProfile.IsDeepDeleteFolders = true;
                deleteProfile.IsDeepDeleteChildrenInFolders = true;
                OperationOptions operationOptions = new OperationOptions();
                operationOptions.DeleteProfile = deleteProfile;
                objectService.Delete(createdObjectIdentities, operationOptions);
                Console.WriteLine("Cleaned up the following sample data in repository (including all folder descendants):");
                foreach (ObjectIdentity id in createdObjectIdentities.Identities)
                {
                    Console.WriteLine(id.GetValueAsString());
                }   
                createdObjectIdentities.Identities.Clear();
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

        public void DeleteSampleAcl(AclIdentity aclIdentity)
        {
            if (this.IsDataCleanedUp)
            {
                List<AclIdentity> aclIdentityList = new List<AclIdentity>();
                aclIdentityList.Add(aclIdentity);
                aclService.Delete(aclIdentityList);
                Console.WriteLine("Deleted sample ACL " + aclIdentity.Name);
            }
        }

        public void DeleteAll()
        {
            DeleteCreatedObjects();
            DeleteTestCabinet();
        }


        /*
         *  Adds the ObjectIdentity of each DataObject in DataPackage to createdObjectIdentities
         *  Builds a list of objects to clean up after a test
         * */
        public void AddCreatedObjects(DataPackage dataPackage)
        {
            List<DataObject> dataObjectList = dataPackage.DataObjects;
            foreach (DataObject dataObject in dataObjectList)
            {
                createdObjectIdentities.AddIdentity(dataObject.Identity);
                Console.WriteLine("Added " + dataObject.Identity.GetValueAsString() + " to collection of sample data.");
            }

        }

        /*
         *  Adds an ObjectIdentity to createdObjectIdentities
         *  Builds a list of objects to clean up after a test
         * */
        public void AddCreatedObjects(ObjectIdentity objectIdentity)
        {
            createdObjectIdentities.AddIdentity(objectIdentity);
            Console.WriteLine("Added " + objectIdentity.GetValueAsString() + " to collection of sample data.");
        }

        /*
         *   Creates a place to put objects to be used by get operation tests
         * */
        public void CreateTestPaths(string repositoryName)
        {
            if (repositoryName == null) { repositoryName = serviceDemo.DefaultRepository; }
            ObjectPath cabinetPath = new ObjectPath();
            cabinetPath.Path = testCabinetPath;
            objectService.CreatePath(cabinetPath, serviceDemo.DefaultRepository);

            ObjectPath sourceFolderPath = new ObjectPath(sourcePath);
            ObjectPath targetFolderPath = new ObjectPath(targetPath);

            objectService.CreatePath(sourceFolderPath, repositoryName);
            objectService.CreatePath(targetFolderPath, repositoryName);
            Console.WriteLine("Creating test cabinet and subfolders on " + repositoryName);
        }

        /*
         *   Creates a set of sample objects to be used by get operation tests
         * */
        public void CreateDemoObjects()
        {
            DataPackage dataPackage = new DataPackage();
            String[] keywords = new String[] { "lions", "tigers", "bears" };
            dataPackage.AddDataObject(BuildTestDoc(gifImageObjectName, "upwind", "Sail upwind", keywords, gifImageFilePath));
            dataPackage.AddDataObject(BuildTestDoc(gifImage1ObjectName, "downwind", "Sail downwind", keywords, gifImage1FilePath));
            try
            {
                CreateTestPaths(serviceDemo.DefaultRepository);
                String toRepositoryName = serviceDemo.SecondaryRepository;
                if (serviceDemo.DefaultRepository != null
                    && serviceDemo.DefaultRepository != serviceDemo.DefaultRepository)
                {
                    CreateTestPaths(serviceDemo.SecondaryRepository);
                    Console.WriteLine("Created test paths on second repository " + serviceDemo.DefaultRepository);
                }
                OperationOptions operationOptions = null;
                sampleData = objectService.Create(dataPackage, operationOptions);
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

        private DataObject BuildTestDoc(String objectName, String subject, String title, String[] keywords, String contentPath)
        {
            String repositoryName = serviceDemo.DefaultRepository;
            ObjectIdentity sampleObjId = new ObjectIdentity(repositoryName);
            DataObject sampleDataObject = new DataObject(sampleObjId, "dm_document");

            sampleDataObject.Properties.Set("object_name", objectName);
            sampleDataObject.Properties.Set("subject", subject);
            sampleDataObject.Properties.Set("title", title);
            sampleDataObject.Properties.Set("keywords", keywords);

            if (!File.Exists(contentPath))
            {
                throw new Exception("Test file: " + contentPath + " does not exist");
            }
            sampleDataObject.Properties.Set("a_content_type", "gif");
            sampleDataObject.Contents.Add(new FileContent(Path.GetFullPath(contentPath), "gif"));

            sampleDataObject.Relationships.Add(sampleFolderRelationship);

            return sampleDataObject;
        }

        public void VersionSampleObjects()
        {
            Console.WriteLine("Creating versions of sample data for VersionControlService samples.");
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            IVersionControlService versionSvc
                = serviceFactory.GetRemoteService<IVersionControlService>(serviceDemo.DemoServiceContext);

            ObjectIdentitySet objIdSet = new ObjectIdentitySet();

            ObjectIdentity docObjId = new ObjectIdentity();
            docObjId.RepositoryName = serviceDemo.DefaultRepository;
            docObjId.Value = new ObjectPath(gifImageObjPath);
            ObjectIdentity doc1ObjId = new ObjectIdentity();
            doc1ObjId.RepositoryName = serviceDemo.DefaultRepository;
            doc1ObjId.Value = new ObjectPath(gifImage1ObjPath);
            objIdSet.AddIdentity(docObjId);
            objIdSet.AddIdentity(doc1ObjId);

            OperationOptions operationOptions = new OperationOptions();
            ContentProfile contentProfile
                = new ContentProfile(FormatFilter.ANY,
                                     null,
                                     PageFilter.ANY, -1,
                                     PageModifierFilter.ANY,
                                     null);
            operationOptions.ContentProfile = contentProfile;

            DataPackage checkinPackage = versionSvc.Checkout(objIdSet, operationOptions);
            Console.WriteLine("Checked out sample objects.");

            for (int i = 0; i <= 1; i++)
            {
                DataObject checkinObj = checkinPackage.DataObjects[i];
                checkinObj.Contents = null;
                FileContent newContent = new FileContent();
                newContent.LocalPath = gifImageFilePath;
                newContent.RenditionType = RenditionType.PRIMARY;
                newContent.Format = "gif";
                checkinObj.Contents.Add(newContent);
            }

            bool retainLock = false;
            List<String> labels = new List<String>();
            labels.Add("test_version");
            versionSvc.Checkin(checkinPackage, VersionStrategy.NEXT_MINOR, retainLock, labels, operationOptions);
            Console.WriteLine("Checked in sample object with label 'test_version'");
        }


        public ObjectIdentitySet CreatedObjectIdentities
        {
            get { return createdObjectIdentities; }
            set { createdObjectIdentities = value; }
        }

        public IObjectService ObjectService
        {
            get { return objectService; }
            set { objectService = value; }
        }

        public ObjectIdentity GetSampleObjectId(String name)
        {
            int i;
            DataObject obj;
            for (i = 0; i < sampleData.DataObjects.Count; i++ )
            {
                obj = sampleData.DataObjects[i];
                if (!obj.Identity.ValueType.Equals(ObjectIdentityType.OBJECT_ID))
                {
                    throw new Exception("Expected object id, got another id type.");
                }
                if (obj.Properties.Get("object_name").GetValueAsString().Equals(name))
                {
                    return obj.Identity;
                }
            }
            return null;
        }

        public String GetUserNameFromRepository()
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            IQueryService queryService = serviceFactory
                .GetRemoteService<IQueryService>(serviceDemo.DemoServiceContext);

            PassthroughQuery query = new PassthroughQuery();

            query.QueryString = "select user_name from dm_user where user_login_name = '" + serviceDemo.UserName + "'";
            query.AddRepository(serviceDemo.DefaultRepository);
            QueryExecution queryEx = new QueryExecution();
            queryEx.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            queryEx.MaxResultCount = -1; // use default limit configured in dfs.properties
            OperationOptions operationOptions = null;
            QueryResult queryResult = queryService.Execute(query, queryEx, operationOptions);
            PropertySet objectProperties = queryResult.DataPackage.DataObjects[0].Properties;
            return objectProperties.Get("user_name").GetValueAsString();
        }

        public bool IsDataCleanedUp
        {
            get { return isDataCleanedUp; }
            set { isDataCleanedUp = value; }
        }


        private ReferenceRelationship sampleFolderRelationship;
        private DemoBase serviceDemo;
        private ObjectIdentitySet createdObjectIdentities;

        // set the following to false if you want to preserve sample data
        // bear in mind this may lead to duplicate file errors if you run multiple samples
        private bool isDataCleanedUp = true;

        private IObjectService objectService;
        private IAccessControlService aclService;
        private DataPackage sampleData;

        public const String gifImageFilePath = ".\\content\\bl_upwind.gif";
        public const String gifImage1FilePath = ".\\content\\bl_downwind.gif";
        public const String gifImageObjectName = "DFS_sample_image";
        public const String gifImage1ObjectName = "DFS_sample1_image";
        public const String testVdmObjectName = "Test_Virtual_Document";
        public const String snapshotObjectName = "Snapshot_doc";

        // if multiple developers are testing the samples, create a unique name testCabinetPath to avoid conflicts
        public const String testCabinetPath = "/DFSTestCabinetXX";
        public const String sourcePath = testCabinetPath + "/SourceFolder";
        public const String targetPath = testCabinetPath + "/TargetFolder";
        public const String gifImageObjPath = sourcePath + "/" + gifImageObjectName;
        public const String gifImage1ObjPath = sourcePath + "/" + gifImage1ObjectName;
        public const String gifImageQualString = "dm_document where object_name = '" +
                                                 gifImageObjectName + "' and folder('" + sourcePath + "')";
        public const String gifImage1QualString = "dm_document where object_name = '" +
                                                  gifImage1ObjectName + "' and folder('" + sourcePath + "')";
        public const String testVdmQualString = "dm_document where object_name = '" +
                                                testVdmObjectName + "' and folder('" + sourcePath + "')";
        public const String snapshotQualString = "dm_document where object_name = '" +
                                                 snapshotObjectName + "' and folder('" + sourcePath + "')";
    }
}

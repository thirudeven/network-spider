using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Acl;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Properties;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TObjectServiceDemo : TestBase
    {
        public TObjectServiceDemo()
        {
            try
            {
                objectServiceDemo = new ObjectServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(objectServiceDemo);
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

        public void TestCreateContentless()
        {
            Console.WriteLine("\nTesting creation of contentless document.");
            try
            {
                DataPackage resultDataPackage = objectServiceDemo.CreateContentlessDocument();
                Console.WriteLine("Created contentless document in default repository location.");
                sampleContentManager.AddCreatedObjects(resultDataPackage);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteCreatedObjects();
            }
        }

        public void TestCreateNewFolder()
        {
            Console.WriteLine("\nTesting creation of new folder.");
            try
            {
                DataPackage resultDataPackage = objectServiceDemo.CreateNewFolder();
                sampleContentManager.AddCreatedObjects(resultDataPackage);
                DataObject resultObject = resultDataPackage.DataObjects[0];
                Console.WriteLine("Created folder " + resultObject.Identity.GetValueAsString() + " in default repository location.");
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteCreatedObjects();
            }
        }

        public void TestCreateWithContentDefaultContext()
        {
            Console.WriteLine("\nTesting creation of content using service context defaults.");
            try
            {
                DataPackage resultDataPackage
                    = objectServiceDemo.CreateWithContentDefaultContext(SampleContentManager.gifImage1FilePath);
                DataObject resultObject = resultDataPackage.DataObjects[0];
                Console.WriteLine("Created object with content " + resultObject.Identity.GetValueAsString());
                sampleContentManager.AddCreatedObjects(resultDataPackage);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteCreatedObjects();
            }
        }

        public void TestCreateWithContentModifyContext()
        {
            Console.WriteLine("\nTesting creation of content with modified service context.");
            try
            {
                DataPackage resultDataPackage
                    = objectServiceDemo.CreateWithContentModifyContext(SampleContentManager.gifImageFilePath);
                sampleContentManager.AddCreatedObjects(resultDataPackage);
                Console.WriteLine("Created object "
                                  + resultDataPackage.DataObjects[0].Identity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteCreatedObjects();
            }
        }

        public void TestCreateAndLinkToFolder()
        {
            Console.WriteLine("\nTesting creation of new document and linking it to a folder.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                DataObject resultDataObject = objectServiceDemo.CreateAndLinkToFolder(SampleContentManager.sourcePath);
                Console.WriteLine("Created document " + resultDataObject.Identity.GetValueAsString() + " in " + SampleContentManager.sourcePath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteAll();
            }
        }

        public void TestCreateFolderAndLinkedDoc()
        {
            Console.WriteLine("\nTesting creation of new folder and document linked to it.");
            try
            {
                DataPackage resultDataPackage = objectServiceDemo.CreateFolderAndLinkedDoc();
                DataObject resultDataObject = resultDataPackage.DataObjects[0];

                // get the parent relationship to the folder object
                ReferenceRelationship refRelationship = (ReferenceRelationship)resultDataObject.Relationships[0];
                String folderId = refRelationship.Target.Value.ToString();
                ObjectIdentity folderIdentity = new ObjectIdentity(new ObjectId(folderId), DefaultRepository);

                Console.WriteLine("Created document " + resultDataObject.Identity.GetValueAsString() + " in folder " + folderId);

                // add the folder identity to the collection of stuff to delete after running the sample
                sampleContentManager.AddCreatedObjects(folderIdentity);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteCreatedObjects();
            }
        }


        public void TestCreatePath()
        {
            Console.WriteLine("\nTesting creation of new path in repository.");
            try
            {
                String folderPath = SampleContentManager.testCabinetPath + "/testCreatePathFolder";
                Console.WriteLine("Calling CreateFolderInCabinet sample method.");
                ObjectIdentity objectIdentity = objectServiceDemo.CreateFolderInCabinet(folderPath);
                Console.WriteLine("Created path: " + objectIdentity.GetValueAsString() + ", " + SampleContentManager.testCabinetPath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestObjDelete()
        {
            Console.WriteLine("\nTesting object deletion.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                string path = SampleContentManager.gifImageObjPath;
                objectServiceDemo.ObjServiceDelete(path);
                Console.WriteLine("Deleted object " + path);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetObjectDefaultsObjPath()
        {
            Console.WriteLine("\nTesting object get with default settings using a path as ObjectIdentity.");
            sampleContentManager.CreateDemoObjects();
            ObjectPath objectPath = new ObjectPath(SampleContentManager.sourcePath);
            ObjectIdentity objIdentity = new ObjectIdentity(objectPath, objectServiceDemo.DefaultRepository);
            try
            {
                DataObject dataObject = objectServiceDemo.GetObjectWithDefaults(objIdentity);
                Console.WriteLine("Got object " + objIdentity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetFilterProperties()
        {
            Console.WriteLine("\nTesting object get with property filter.");
            sampleContentManager.CreateDemoObjects();
            ObjectIdentity objIdentity =
                new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                   objectServiceDemo.DefaultRepository);
            try
            {
                DataObject dataObject = objectServiceDemo.GetObjectFilterProperties(objIdentity);
                Console.WriteLine("Got object " + objIdentity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetFilterPropertiesInclude()
        {
            Console.WriteLine("\nTesting get object using property include setting.");
            sampleContentManager.CreateDemoObjects();
            ObjectIdentity objIdentity =
                new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                   objectServiceDemo.DefaultRepository);
            try
            {
                DataObject dataObject = objectServiceDemo.GetObjectFilterPropertiesInclude(objIdentity);
                Console.WriteLine("Got object " + "'" + dataObject.Identity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetFilterPropertiesExclude()
        {
            Console.WriteLine("\nTesting get object using property exclude setting.");
            sampleContentManager.CreateDemoObjects();
            ObjectIdentity objIdentity =
                new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                   objectServiceDemo.DefaultRepository);
            try
            {
                DataObject dataObject = objectServiceDemo.GetObjectFilterPropertiesExclude(objIdentity);
                Console.WriteLine("Got object " + objIdentity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }

        public void TestGetObjectFilterRelationParentOnly()
        {
            Console.WriteLine("\nTesting get object using filter to get only parent relationships.");
            sampleContentManager.CreateDemoObjects();
            ObjectIdentity objIdentity =
                 new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                    objectServiceDemo.DefaultRepository);
            try
            {
                DataObject dataObject = objectServiceDemo.GetObjectFilterRelationsParentOnly(objIdentity);
                Console.WriteLine("Got object with relationships" + objIdentity.GetValueAsString());
                ReferenceRelationship parentFolderReference = (ReferenceRelationship)dataObject.Relationships[0];
                Console.WriteLine("Related folder is " + parentFolderReference.Target.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        private void GetWithContent(string geoLoc, ContentTransferMode trMode)
        {
            sampleContentManager.CreateDemoObjects();
            if (geoLoc.Equals("") || geoLoc.Equals(null)) { geoLoc = "Pleasanton"; }
            if (trMode.Equals(null)) trMode = ContentTransferMode.MTOM;
            try
            {
                ObjectIdentity objIdentity =
                                new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                                   objectServiceDemo.DefaultRepository);
                DataObject dataObject = objectServiceDemo.GetWithContent(objIdentity, geoLoc, trMode);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }

        public void TestMTOM()
        {
            Console.WriteLine("\nTesting get with MTOM content transfer.");
            ContentTransferMode trMode = ContentTransferMode.MTOM;
            GetWithContent("Pleasanton", trMode);
        }


        public void TestBase64()
        {
            Console.WriteLine("\nTesting get with Base64 content transfer.");
            ContentTransferMode trMode = ContentTransferMode.BASE64;
            GetWithContent("Pleasanton", trMode);
        }

        public void TestGetWithUrl()
        {
            Console.WriteLine("\nTesting get content using URLs");
            sampleContentManager.CreateDemoObjects();
            try
            {
                ObjectIdentity objIdentity = new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString), this.DefaultRepository);
                FileInfo fi = objectServiceDemo.GetObjectWithUrl(objIdentity);
                if (fi.Equals(null)) { throw new Exception("Failed to get object content using URL."); }
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }

        public void TestGetFilterContentNone()
        {
            Console.WriteLine("\nTesting get object using content filter.");
            DataObject dataObject;
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity objIdentity =
                                new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                                   objectServiceDemo.DefaultRepository);
                dataObject = objectServiceDemo.GetFilterContentNone(objIdentity);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetFilterContentSpecific()
        {
            Console.WriteLine("\nTesting get object filtering for a specific content format.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                DataObject dataObject =
                    objectServiceDemo.GetFilterContentSpecific(SampleContentManager.gifImageQualString);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestGetWithPermissions()
        {
            Console.WriteLine("\nTesting get object with permissions.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity objIdentity =
                    new ObjectIdentity(new Qualification(SampleContentManager.gifImageQualString),
                                       objectServiceDemo.DefaultRepository);
                DataObject dataObject = objectServiceDemo.GetWithPermissions(objIdentity);
                Console.WriteLine("Got object with permissions " + dataObject.Identity.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestObjMove()
        {
            Console.WriteLine("\nTesting moving an object to another folder.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                objectServiceDemo.ObjServiceMove(SampleContentManager.gifImageObjPath,
                                                 SampleContentManager.targetPath,
                                                 SampleContentManager.sourcePath);
                Console.WriteLine("Moved sample file from " + SampleContentManager.sourcePath + " to " + SampleContentManager.targetPath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestUpdateProperties()
        {
            Console.WriteLine("\nTesting update properties.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity objID = new ObjectIdentity();
                objID.RepositoryName = objectServiceDemo.DefaultRepository;
                objID.Value = new Qualification(SampleContentManager.gifImageQualString);

                string newTitle = "updated title " + System.DateTime.Now.Ticks.ToString();
                string newSubject = "update subject " + System.DateTime.Now.Ticks.ToString();
                string[] newKeywords = { "lions", "tigers", "bears" };
                DataPackage dp = objectServiceDemo.UpdateObjectProperties(objID, newTitle, newSubject, newKeywords);

                DataObject dataObject = dp.DataObjects[0];
                string titleVal = dataObject.Properties.Get("title").GetValueAsString();
                string subjectVal = dataObject.Properties.Get("subject").GetValueAsString();

                Console.WriteLine("Updated properties, title is '" + titleVal + "' subject is '" + subjectVal + "'");
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestUpdatePropertiesFetched()
        {
            Console.WriteLine("\nTesting update of object properties first fetched from the repository.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity objID = new ObjectIdentity();
                objID.RepositoryName = objectServiceDemo.DefaultRepository;
                objID.Value = new Qualification(SampleContentManager.gifImageQualString);

                string newTitle = "updated title " + System.DateTime.Now.Ticks.ToString();
                string newSubject = "update subject " + System.DateTime.Now.Ticks.ToString();
                string[] newKeywords = { "lions", "tigers", "bears", "sharks" };
                DataPackage dp = objectServiceDemo.UpdateFetchedObjectProperties(objID, newTitle, newSubject, newKeywords);

                DataObject dataObject = dp.DataObjects[0];
                String titleVal = dataObject.Properties.Get("title").GetValueAsString();
                String subjectVal = dataObject.Properties.Get("subject").GetValueAsString();

                Console.WriteLine("Updated properties, title is '" + titleVal + "' subject is '" + subjectVal + "'");
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestUpdateContent()
        {
            Console.WriteLine("\nTesting update of object content.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity objID = new ObjectIdentity();
                objID.RepositoryName = objectServiceDemo.DefaultRepository;
                objID.Value = new Qualification(SampleContentManager.gifImageQualString);
                string testContentPath = SampleContentManager.gifImage1FilePath;
                String absoluteFilePath = Path.GetFullPath(testContentPath);
                objectServiceDemo.UpdateContent(objID, absoluteFilePath);

                Console.WriteLine("Updated content of object " + objID.GetValueAsString() + " to " + absoluteFilePath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestUpdateRelinkFolder()
        {
            Console.WriteLine("\nTesting unlinking an object from one folder and linking to another.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity documentId = sampleContentManager.GetSampleObjectId(SampleContentManager.gifImageObjectName);
                ObjectPath sourceFolderPath = new ObjectPath(SampleContentManager.sourcePath);
                ObjectPath targetFolderPath = new ObjectPath(SampleContentManager.targetPath);
                ObjectIdentity sourceFolderId = new ObjectIdentity(sourceFolderPath, objectServiceDemo.DefaultRepository);
                ObjectIdentity targetFolderId = new ObjectIdentity(targetFolderPath, objectServiceDemo.DefaultRepository);
                objectServiceDemo.UpdateRelinkFolder(documentId, sourceFolderId, targetFolderId);

                Console.WriteLine("Relinked object " + documentId.GetValueAsString() + " to folder " + targetFolderPath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestUpdateRepeatProperty()
        {
            Console.WriteLine("\nTesting update of repeating property.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                ObjectIdentity docID = new ObjectIdentity();
                docID.RepositoryName = objectServiceDemo.DefaultRepository;
                docID.Value = new Qualification(SampleContentManager.gifImageQualString);
                objectServiceDemo.UpdateRepeatProperty(docID);
                Console.WriteLine("Updated keywords property of " + docID.GetValueAsString());
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }

        public void TestObjCopy()
        {
            Console.WriteLine("\nTesting copying an object to another folder.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                Console.WriteLine("Copying object from " + SampleContentManager.gifImageObjPath + " to " + SampleContentManager.targetPath);
                objectServiceDemo.ObjServiceCopy(SampleContentManager.gifImageObjPath, SampleContentManager.targetPath);
                Console.WriteLine("Object copied.");
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestObjCopyWithMods()
        {
            Console.WriteLine("\nTesting copying an object to another folder while applying modifications.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                Console.WriteLine("Copying object from " + SampleContentManager.gifImageObjPath + " to " + SampleContentManager.targetPath);
                objectServiceDemo.ObjServiceCopyWithMods(SampleContentManager.gifImageObjPath, SampleContentManager.targetPath);
                Console.WriteLine("Copied object.");
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
            }
        }


        public void TestObjCopyAcrossRepositories()
        {
            if (objectServiceDemo.SecondaryRepository == null
                || objectServiceDemo.SecondaryRepository == ""
                || objectServiceDemo.SecondaryRepository == objectServiceDemo.DefaultRepository)
            {
                Console.WriteLine("There is no secondary repository defined in DemoBase, so no test of copy across repositories will be run.");
                return;
            }

            try
            {
                sampleContentManager.CreateDemoObjects();
                objectServiceDemo.ObjServiceCopyAcrossRepositories(SampleContentManager.gifImageObjPath, SampleContentManager.targetPath);
                Console.WriteLine("Copied object from " + SampleContentManager.gifImageObjPath + " to " + SampleContentManager.targetPath);
            }
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
            finally
            {
                sampleContentManager.DeleteTestCabinet();
                sampleContentManager.DeleteSecondaryTestCabinet();
            }
        }


        public SampleContentManager SampleContentManager
        {
            get { return sampleContentManager; }
            set { sampleContentManager = value; }
        }

        private ObjectServiceDemo objectServiceDemo;
        private SampleContentManager sampleContentManager;


    }
}

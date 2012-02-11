using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.DataModel.Core.Lifecycle;
using Emc.Documentum.FS.Services.Core.Lifecycle;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TVirtualDocumentServiceDemo : TestBase
    {
        private VirtualDocumentServiceDemo virtualDocumentServiceDemo;
        private SampleContentManager sampleContentManager;

        public TVirtualDocumentServiceDemo()
        {
            try
            {
                virtualDocumentServiceDemo = new VirtualDocumentServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(virtualDocumentServiceDemo);
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

        private void createTestVirtualDocument()
        {
                // create a new DataObject to use as the parent node
                ObjectIdentity emptyIdentity = new ObjectIdentity(DefaultRepository);
                DataObject parentDO = new DataObject(emptyIdentity);
                parentDO.Type = "dm_document";
                PropertySet parentProperties = new PropertySet();
                parentProperties.Set("object_name", SampleContentManager.testVdmObjectName);
                parentDO.Properties = parentProperties;

                // link into a folder
                ObjectPath objectPath = new ObjectPath(SampleContentManager.sourcePath);
                ObjectIdentity sampleFolderIdentity = new ObjectIdentity(objectPath, DefaultRepository);
                ReferenceRelationship sampleFolderRelationship = new ReferenceRelationship();
                sampleFolderRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
                sampleFolderRelationship.Target = sampleFolderIdentity;
                sampleFolderRelationship.TargetRole = Relationship.ROLE_PARENT;
                parentDO.Relationships.Add(sampleFolderRelationship);

                // get id of document to use for first child node
                ObjectIdentity child0Id = new ObjectIdentity();
                child0Id.RepositoryName = DefaultRepository;
                child0Id.Value = new Qualification(SampleContentManager.gifImageQualString);

                // get id of document to use for second child node
                ObjectIdentity child1Id = new ObjectIdentity();
                child1Id.RepositoryName = DefaultRepository;
                child1Id.Value = new Qualification(SampleContentManager.gifImage1QualString);

                ObjectIdentitySet childNodes = new ObjectIdentitySet();
                childNodes.AddIdentity(child0Id);
                childNodes.AddIdentity(child1Id);

                virtualDocumentServiceDemo.AddChildNodes(parentDO, childNodes);
        }

        public void TestAddChildNodes()
        {
            Console.WriteLine("\nRunning virtual document add child nodes...");
            try
            {
                sampleContentManager.CreateDemoObjects();
                createTestVirtualDocument();
                Console.WriteLine("Completed update of VDM node.");
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

        public void TestRetrieveVdmInfo()
        {
            Console.WriteLine("\nTesting retrieve virtual document info...");
            try
            {
                sampleContentManager.CreateDemoObjects();
                createTestVirtualDocument();

                // create ObjectIdentity of existing virtual document
                ObjectIdentity testVdmId = new ObjectIdentity();
                testVdmId.RepositoryName = DefaultRepository;
                Qualification qual = new Qualification(SampleContentManager.testVdmQualString);
                qual.ObjectType = "dm_document";
                testVdmId.Value = qual;

                virtualDocumentServiceDemo.RetrieveVdmInfo(testVdmId, false);
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

        public void TestCreateSnapshot()
        {
            Console.WriteLine("\nTesting create virtual document snapshot...");
            try
            {
                sampleContentManager.CreateDemoObjects();
                createTestVirtualDocument();

                DataObject DO = virtualDocumentServiceDemo.CreateSnapshotDemo(SampleContentManager.testVdmQualString,
                                                                       SampleContentManager.snapshotObjectName,
                                                                       SampleContentManager.sourcePath);
                virtualDocumentServiceDemo.RetrieveVdmInfo(DO.Identity, true);
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

        public void TestCreateInlineSnapshot()
        {
            Console.WriteLine("\nTesting create virtual document snapshot...");
            try
            {
                sampleContentManager.CreateDemoObjects();
                createTestVirtualDocument();

                DataObject DO = virtualDocumentServiceDemo.CreateInlineSnapshotDemo(SampleContentManager.testVdmQualString,
                                                                             SampleContentManager.sourcePath);
                virtualDocumentServiceDemo.RetrieveVdmInfo(DO.Identity, true);
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

        public void TestRemoveSnapshot()
        {
            Console.WriteLine("\nTesting remove virtual document snapshot...");
            try
            {
                sampleContentManager.CreateDemoObjects();
                createTestVirtualDocument();

                DataObject DO = virtualDocumentServiceDemo.CreateSnapshotDemo(SampleContentManager.testVdmQualString,
                                                       SampleContentManager.snapshotObjectName,
                                                       SampleContentManager.sourcePath);

                virtualDocumentServiceDemo.RemoveSnapshot(SampleContentManager.snapshotQualString);
                Console.WriteLine("Snapshot deleted.");
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
    }
}

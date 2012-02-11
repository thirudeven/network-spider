using System;
using System.Collections.Generic;
using System.Text;

using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Services.Core.Vdm;
using Emc.Documentum.FS.DataModel.Core.Vdm;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class VirtualDocumentServiceDemo : DemoBase
    {
        private IVirtualDocumentService virtualDocumentService;

        public VirtualDocumentServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            virtualDocumentService =
                serviceFactory.GetRemoteService<IVirtualDocumentService>(DemoServiceContext);
        }

        public DataObject AddChildNodes(DataObject parentObject, ObjectIdentitySet childIdentities)
	{
		List<ObjectIdentity> idList = childIdentities.Identities;
		List<VdmChildrenActionInfo> caInfoList = new List<VdmChildrenActionInfo>();
		foreach (ObjectIdentity objIdentity in idList)
		{
			VirtualDocumentNode vdmNode = new VirtualDocumentNode();
			vdmNode.Identity = objIdentity;
			VirtualDocumentInfo vdmInfo = new VirtualDocumentInfo();
			vdmInfo.Binding = VirtualDocumentInfo.BINDING_LATE;
			vdmInfo.CopyBehavior = CopyBehaviorMode.COPY;
			vdmInfo.OverrideLateBinding = false;
			vdmNode.Policy = vdmInfo;
			VdmChildrenActionInfo caInfo = new VdmChildrenActionInfo();
			caInfo.Action = VdmChildrenAction.APPEND;
			caInfo.Node = vdmNode;
			caInfoList.Add(caInfo);
		}
		
        VdmUpdateProfile vdmUpdateProfile = new VdmUpdateProfile();
		List<String> versionLabels = new List<String>();
		versionLabels.Add("testVersionLabel");
		
		// make sure to add the CURRENT label if you
		// want the virtual document to be CURRENT
		versionLabels.Add("CURRENT");
		vdmUpdateProfile.Labels = versionLabels;
		
		OperationOptions options = new OperationOptions();
		options.VdmUpdateProfile = vdmUpdateProfile;
		return virtualDocumentService.Update(parentObject, caInfoList, options);
	}

        public DataObject RetrieveVdmInfo(ObjectIdentity objectIdentity, Boolean isSnapshot)
	{		
		VdmRetrieveProfile retrieveProfile = new VdmRetrieveProfile();
		retrieveProfile.IsShouldFollowAssembly = isSnapshot;
		retrieveProfile.Binding = "CURRENT";
		OperationOptions options = new OperationOptions();
		options.VdmRetrieveProfile = retrieveProfile;
		
		DataObject resultDO = virtualDocumentService.Retrieve(objectIdentity, options);
		List<Relationship> relationships = resultDO.Relationships;
		Console.WriteLine("Total relationships in virtual document = " + relationships.Count);
		
		int i = 0;
		foreach (Relationship r in relationships)
		{
			Console.WriteLine();
			ReferenceRelationship refRel = (ReferenceRelationship)r;
			Console.WriteLine("Child node " + i++ + ": " + refRel.Target.GetValueAsString());
			PropertySet nodeProperties = refRel.RelationshipProperties;

            List<Property> properties = nodeProperties.Properties;
            foreach (Property p in properties)
			{
				Console.Write(p.Name + ": ");
				Console.WriteLine(p.GetValueAsString());
			}
		}
		return resultDO;
	}

        public DataObject CreateSnapshotDemo(String vdmQualString, String snapshotName, String sourcePath)
        {
            // create ObjectIdentity of existing virtual document
            ObjectIdentity testVdmId = new ObjectIdentity();
            testVdmId.RepositoryName = DefaultRepository;
            testVdmId.Value = new Qualification(vdmQualString);

            // create a new DataObject to use for the snapshot
            ObjectIdentity emptyIdentity = new ObjectIdentity(DefaultRepository);
            DataObject snapshotDO = new DataObject(emptyIdentity);
            snapshotDO.Type = "dm_document";
            PropertySet parentProperties = new PropertySet();
            parentProperties.Set("object_name", snapshotName);
            snapshotDO.Properties = parentProperties;

            // link into a folder
            ObjectPath objectPath = new ObjectPath(sourcePath);
            ObjectIdentity sampleFolderIdentity = new ObjectIdentity(objectPath, DefaultRepository);
            ReferenceRelationship sampleFolderRelationship = new ReferenceRelationship();
            sampleFolderRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            sampleFolderRelationship.Target = sampleFolderIdentity;
            sampleFolderRelationship.TargetRole = Relationship.ROLE_PARENT;
            snapshotDO.Relationships.Add(sampleFolderRelationship);

            // options are reserved for future use so just pass null
            OperationOptions options = null;
            return virtualDocumentService.CreateSnapshot(testVdmId, snapshotDO, options);
        }

        public DataObject CreateInlineSnapshotDemo(String vdmQualString, String sourcePath)
        {
            // create ObjectIdentity of existing virtual document
            ObjectIdentity testVdmId = new ObjectIdentity();
            testVdmId.RepositoryName = DefaultRepository;
            testVdmId.Value = new Qualification(vdmQualString);

            // create a new DataObject to use for the snapshot
            DataObject snapshotDO = new DataObject(testVdmId);

            // link into a folder
            ObjectPath objectPath = new ObjectPath(sourcePath);
            ObjectIdentity sampleFolderIdentity = new ObjectIdentity(
                    objectPath, DefaultRepository);
            ReferenceRelationship sampleFolderRelationship = new ReferenceRelationship();
            sampleFolderRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            sampleFolderRelationship.Target = sampleFolderIdentity;
            sampleFolderRelationship.TargetRole = Relationship.ROLE_PARENT;
            snapshotDO.Relationships.Add(sampleFolderRelationship);

            // options are reserved for future use so just pass null
            OperationOptions options = null;
            return virtualDocumentService.CreateSnapshot(testVdmId, snapshotDO, options);
        }

        public void RemoveSnapshot(String snapshotQualString)
        {
            // create ObjectIdentity of existing snapshot
            ObjectIdentity testSnapshotId = new ObjectIdentity();
            testSnapshotId.RepositoryName = DefaultRepository;
            testSnapshotId.Value = new Qualification(snapshotQualString);

            // remove snapshot
            virtualDocumentService.RemoveSnapshot(testSnapshotId, null);
        }

    }
}

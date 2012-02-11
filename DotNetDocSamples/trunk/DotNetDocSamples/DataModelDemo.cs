using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Schema;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class DataModelDemo
    {
        public void ShowDataPackage()
        {
            DataObject dataObject = new DataObject(new ObjectIdentity("myRepository"));
            DataPackage dataPackage = new DataPackage(dataObject);

            DataObject dataObject1 = new DataObject(new ObjectIdentity("myRepository"));
            dataPackage.AddDataObject(dataObject1);

            foreach (DataObject dataObject2 in dataPackage.DataObjects)
            {
                Console.WriteLine("Data Object: " + dataObject2);
            }
        }

        public void ShowObjectIdentity()
        {
            String repName = "MyRepositoryName";
            ObjectIdentity[] objectIdentities = new ObjectIdentity[4];

            // repository only is required to represent an object that has not been created
            objectIdentities[0] = new ObjectIdentity(repName);

            // show each form of unique identifier
            ObjectId objId = new ObjectId("090007d280075180");
            objectIdentities[1] = new ObjectIdentity(objId, repName);
            Qualification qualification = new Qualification("dm_document where r_object_id = '090007d280075180'");
            objectIdentities[2] = new ObjectIdentity(qualification, repName);

            ObjectPath objPath = new ObjectPath("/testCabinet/testFolder/testDoc");
            objectIdentities[3] = new ObjectIdentity(objPath, repName);

            foreach (ObjectIdentity identity in objectIdentities)
            {
                Console.WriteLine(identity.GetValueAsString());
            }
        }

        public void ShowObjectIdentitySet()
    {
        String repName = "MyRepositoryName";
        ObjectIdentitySet objIdSet = new ObjectIdentitySet();
        ObjectIdentity[] objectIdentities = new ObjectIdentity[4];

        // add some ObjectIdentity instances
        ObjectId objId = new ObjectId("090007d280075180");
        objIdSet.AddIdentity(new ObjectIdentity(objId, repName));

        Qualification qualification = new Qualification("dm_document where object_name = 'bl_upwind.gif'");
        objIdSet.AddIdentity(new ObjectIdentity(qualification, repName));

        ObjectPath objPath = new ObjectPath("/testCabinet/testFolder/testDoc");
        objIdSet.AddIdentity(new ObjectIdentity(objPath, repName));

        // walk through and see what we have
        IEnumerator<ObjectIdentity> identityEnumerator = objIdSet.Identities.GetEnumerator();
        while (identityEnumerator.MoveNext())
        {
            Console.WriteLine("Object Identity: " + identityEnumerator.Current);
        }
    }

        public void ShowPermissions(DataObject dataObject)
    {
        List<Permission> permissions = dataObject.Permissions;
        foreach (Permission permission in permissions)
        {
            Console.WriteLine(permission.Name);
        }
    }

        public void ConstructDataObject(String repositoryName, String objName, String objTitle)
        {
            ObjectIdentity objIdentity = new ObjectIdentity(repositoryName);
            DataObject dataObject = new DataObject(objIdentity, "dm_document");

            PropertySet properties = dataObject.Properties;
            properties.Set("object_name", objName);
            properties.Set("title", objTitle);
            properties.Set("a_content_type", "gif");

            dataObject.Contents.Add(new FileContent("c:/temp/MyImage.gif", "gif"));

            DataPackage dataPackage = new DataPackage(dataObject);
        }

    }

}

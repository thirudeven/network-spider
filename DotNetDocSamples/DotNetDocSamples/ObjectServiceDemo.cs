using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Acl;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Services.Core;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class ObjectServiceDemo : DemoBase
    {
        private IObjectService objectService;

        public ObjectServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            objectService =
                serviceFactory.GetRemoteService<IObjectService>(DemoServiceContext);
        }

        public void ObjServiceCopy(String sourceObjectPathString, String targetLocPathString)
        {
            // identify the object to copy
            ObjectPath objPath = new ObjectPath(sourceObjectPathString);
            ObjectIdentity docToCopy = new ObjectIdentity();
            docToCopy.Value = objPath;
            docToCopy.RepositoryName = DefaultRepository;

            // identify the folder to copy to
            ObjectPath folderPath = new ObjectPath(targetLocPathString);
            ObjectIdentity toFolderIdentity = new ObjectIdentity();
            toFolderIdentity.Value = folderPath;
            toFolderIdentity.RepositoryName = DefaultRepository;
            ObjectLocation toLocation = new ObjectLocation();
            toLocation.Identity = toFolderIdentity;

            OperationOptions operationOptions = null;
            objectService.Copy(new ObjectIdentitySet(docToCopy),
                               toLocation,
                               new DataPackage(),
                               operationOptions);
        }

        public void ObjServiceMove(String sourceObjectPathString,
                                   String targetLocPathString,
                                   String sourceLocPathString)
        {
            // identify the object to move
            ObjectPath objPath = new ObjectPath(sourceObjectPathString);
            ObjectIdentity docToCopy = new ObjectIdentity();
            docToCopy.Value = objPath;
            docToCopy.RepositoryName = DefaultRepository;

            // identify the folder to move from
            ObjectPath fromFolderPath = new ObjectPath();
            fromFolderPath.Path = sourceLocPathString;
            ObjectIdentity fromFolderIdentity = new ObjectIdentity();
            fromFolderIdentity.Value = fromFolderPath;
            fromFolderIdentity.RepositoryName = DefaultRepository;
            ObjectLocation fromLocation = new ObjectLocation();
            fromLocation.Identity = fromFolderIdentity;

            // identify the folder to move to
            ObjectPath folderPath = new ObjectPath(targetLocPathString);
            ObjectIdentity toFolderIdentity = new ObjectIdentity();
            toFolderIdentity.Value = folderPath;
            toFolderIdentity.RepositoryName = DefaultRepository;
            ObjectLocation toLocation = new ObjectLocation();
            toLocation.Identity = toFolderIdentity;

            OperationOptions operationOptions = null;
            objectService.Move(new ObjectIdentitySet(docToCopy),
                               fromLocation,
                               toLocation,
                               new DataPackage(),
                               operationOptions);
        }

        public void ObjServiceCopyAcrossRepositories(String sourceObjectPathString,
                                                 String targetLocPathString)
        {
            // identify the object to copy
            ObjectPath objPath = new ObjectPath(sourceObjectPathString);
            ObjectIdentity docToCopy = new ObjectIdentity();
            docToCopy.Value = objPath;
            docToCopy.RepositoryName = DefaultRepository;

            // identify the folder to copy to
            ObjectPath folderPath = new ObjectPath();
            folderPath.Path = targetLocPathString;
            ObjectIdentity toFolderIdentity = new ObjectIdentity();
            toFolderIdentity.Value = folderPath;
            toFolderIdentity.RepositoryName = SecondaryRepository;
            ObjectLocation toLocation = new ObjectLocation();
            toLocation.Identity = toFolderIdentity;

            OperationOptions operationOptions = null;
            objectService.Copy(new ObjectIdentitySet(docToCopy), toLocation, null, operationOptions);
        }

        public void ObjServiceCopyWithMods(String sourceObjectPathString,
                                      String targetLocPathString)
        {
            // identify the object to copy
            ObjectPath objPath = new ObjectPath(sourceObjectPathString);
            ObjectIdentity docToCopy = new ObjectIdentity();
            docToCopy.Value = objPath;
            docToCopy.RepositoryName = DefaultRepository;

            // identify the folder to copy to
            ObjectPath folderPath = new ObjectPath();
            folderPath.Path = targetLocPathString;
            ObjectIdentity toFolderIdentity = new ObjectIdentity();
            toFolderIdentity.Value = folderPath;
            toFolderIdentity.RepositoryName = DefaultRepository;
            ObjectLocation toLocation = new ObjectLocation();
            toLocation.Identity = toFolderIdentity;

            // specify changes to make when copying
            DataObject modDataObject = new DataObject(docToCopy);
            modDataObject.Type = "dm_document";
            PropertySet modProperties = modDataObject.Properties;
            String newObjectName = "copiedDocument-" + System.DateTime.Now.Ticks;
            modProperties.Set("object_name", newObjectName);
            Console.WriteLine("Modified object by changing object name to " + newObjectName); 
            DataPackage dataPackage = new DataPackage(modDataObject);

            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(docToCopy);
            OperationOptions operationOptions = null;
            objectService.Copy(objIdSet, toLocation, dataPackage, operationOptions);
        }

        public DataPackage CreateContentlessDocument()
        {
            ObjectIdentity objectIdentity = new ObjectIdentity(DefaultRepository);
            DataObject dataObject = new DataObject(objectIdentity, "dm_document");
            PropertySet properties = new PropertySet();
            properties.Set("object_name", "contentless-" + System.DateTime.Now.Ticks);
            dataObject.Properties = properties;
            DataPackage dataPackage = new DataPackage(dataObject);

            OperationOptions operationOptions = null;
            return objectService.Create(dataPackage, operationOptions);
        }

        public DataPackage CreateWithContentDefaultContext(String filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Test file: " + filePath + " does not exist");
            }
            ObjectIdentity objIdentity = new ObjectIdentity(DefaultRepository);
            DataObject dataObject = new DataObject(objIdentity, "dm_document");
            PropertySet properties = dataObject.Properties;
            properties.Set("object_name", "MyImage" + System.DateTime.Now.Ticks);
            properties.Set("title", "MyImage");
            properties.Set("a_content_type", "gif");
            dataObject.Contents.Add(new FileContent(Path.GetFullPath(filePath), "gif"));

            OperationOptions operationOptions = null;
            return objectService.Create(new DataPackage(dataObject), operationOptions);
        }

        public DataPackage CreateWithContentModifyContext(String filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Test file: " + filePath + " does not exist");
            }

            ContentTransferProfile transferProfile = new ContentTransferProfile();
            transferProfile.TransferMode = ContentTransferMode.BASE64;
            Console.WriteLine("Modified service context: changed content transfer mode to BASE64 and Geolocation to 'Pleasanton'");
            transferProfile.Geolocation = "Pleasanton";
            DemoServiceContext.SetProfile(transferProfile);

            ObjectIdentity objIdentity = new ObjectIdentity(DefaultRepository);
            DataObject dataObject = new DataObject(objIdentity, "dm_document");
            PropertySet properties = dataObject.Properties;
            properties.Set("object_name", "MyImage");
            properties.Set("title", "MyImage");
            properties.Set("a_content_type", "gif");
            dataObject.Contents.Add(new FileContent(Path.GetFullPath(filePath), "gif"));

            OperationOptions operationOptions = null;
            return objectService.Create(new DataPackage(dataObject), operationOptions);
        }

        public DataPackage CreateNewFolder()
        {
            ObjectIdentity folderIdentity = new ObjectIdentity();
            folderIdentity.RepositoryName = DefaultRepository;
            DataObject dataObject = new DataObject(folderIdentity, "dm_folder");
            PropertySet properties = new PropertySet();
            String folderName = "aTestFolder-" + System.DateTime.Now.Ticks;
            properties.Set("object_name", folderName);
            dataObject.Properties = properties;

            DataPackage dataPackage = new DataPackage(dataObject);

            OperationOptions operationOptions = null;
            return objectService.Create(dataPackage, operationOptions);
        }

        public DataPackage CreateFolderAndLinkedDoc()
        {
            // create a folder data object
            String folderName = "a-test-folder-" + System.DateTime.Now.Ticks;
            DataObject folderDataObj = new DataObject(new ObjectIdentity(DefaultRepository), "dm_folder");
            PropertySet folderDataObjProperties = new PropertySet();
            folderDataObjProperties.Set("object_name", folderName);
            folderDataObj.Properties = folderDataObjProperties;

            // create a contentless document DataObject
            String doc1Name = "a-test-doc-" + System.DateTime.Now.Ticks;
            DataObject docDataObj = new DataObject(new ObjectIdentity(DefaultRepository), "dm_document");
            PropertySet properties = new PropertySet();
            properties.Set("object_name", doc1Name);
            docDataObj.Properties = properties;

            // add the folder as a parent of the folder
            ObjectRelationship objRelationship = new ObjectRelationship();
            objRelationship.Target = folderDataObj;
            objRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            objRelationship.TargetRole = Relationship.ROLE_PARENT;
            docDataObj.Relationships.Add(new ObjectRelationship(objRelationship));

            // set up the relationship filter to return the doc and folder
            RelationshipProfile relationProfile = new RelationshipProfile();
            relationProfile.ResultDataMode = ResultDataMode.REFERENCE;
            relationProfile.TargetRoleFilter = TargetRoleFilter.ANY;
            relationProfile.NameFilter = RelationshipNameFilter.ANY;
            relationProfile.DepthFilter = DepthFilter.SPECIFIED;
            relationProfile.Depth = 2;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.RelationshipProfile = relationProfile;

            // create the folder and linked document
            DataPackage dataPackage = new DataPackage();
            dataPackage.AddDataObject(docDataObj);
            return objectService.Create(dataPackage, operationOptions);
        }

        public DataObject CreateAndLinkToFolder(String folderPath)
        {
            // create a contentless document to link into folder
            String objectName = "linkedDocument" + System.DateTime.Now.Ticks;
            Console.WriteLine("Constructed document " + objectName);
            String repositoryName = DefaultRepository;
            ObjectIdentity sampleObjId = new ObjectIdentity(repositoryName);
            DataObject sampleDataObject = new DataObject(sampleObjId, "dm_document");
            sampleDataObject.Properties.Set("object_name", objectName);

            // add the folder to link to as a ReferenceRelationship
            ObjectPath objectPath = new ObjectPath(folderPath);
            ObjectIdentity sampleFolderIdentity = new ObjectIdentity(objectPath, DefaultRepository);
            ReferenceRelationship sampleFolderRelationship = new ReferenceRelationship();
            sampleFolderRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            sampleFolderRelationship.Target = sampleFolderIdentity;
            sampleFolderRelationship.TargetRole = Relationship.ROLE_PARENT;
            sampleDataObject.Relationships.Add(sampleFolderRelationship);

            // create a new document linked into parent folder
            OperationOptions operationOptions = null;
            DataPackage dataPackage = new DataPackage(sampleDataObject);
            DataPackage resultPackage = objectService.Create(dataPackage, operationOptions);
            DataObject resultDataObject = resultPackage.DataObjects[0];

            return resultDataObject;
        }

        public DataPackage CreateDocumentInFolder(ObjectIdentity objectIdentity)
        {
            DataPackage dataPackage = new DataPackage(new DataObject(objectIdentity));
            OperationOptions operationOptions = null;
            return objectService.Create(dataPackage, operationOptions);
        }

        public ObjectIdentity CreateFolderInCabinet(String path)
        {
            ObjectPath objPath = new ObjectPath(path);
            return objectService.CreatePath(objPath, DefaultRepository);
        }

        public void ObjServiceDelete(String path)
        {
            ObjectPath docPath = new ObjectPath(path);
            ObjectIdentity objIdentity = new ObjectIdentity();
            objIdentity.Value = docPath;
            objIdentity.RepositoryName = DefaultRepository;
            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objIdentity);

            DeleteProfile deleteProfile = new DeleteProfile();
            deleteProfile.IsDeepDeleteFolders = true;
            deleteProfile.IsDeepDeleteChildrenInFolders = true;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.DeleteProfile = deleteProfile;

            objectService.Delete(objectIdSet, operationOptions);
        }

        public DataObject GetObjectWithDefaults(ObjectIdentity objIdentity)
        {
            objIdentity.RepositoryName = DefaultRepository;
            ObjectIdentitySet objectIdSet = new ObjectIdentitySet();
            List<ObjectIdentity> objIdList = objectIdSet.Identities;
            objIdList.Add(objIdentity);

            OperationOptions operationOptions = null;
            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);

            return dataPackage.DataObjects[0];
        }

        public DataObject GetObjectFilterProperties(ObjectIdentity objIdentity)
        {
            PropertyProfile propertyProfile = new PropertyProfile();
            propertyProfile.FilterMode = PropertyFilterMode.ALL_NON_SYSTEM;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.PropertyProfile = propertyProfile;
            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objIdentity);
            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);
            return dataPackage.DataObjects[0];
        }

        public DataObject GetObjectFilterPropertiesInclude(ObjectIdentity objIdentity)
        {
            PropertyProfile propertyProfile = new PropertyProfile();
            propertyProfile.FilterMode = PropertyFilterMode.SPECIFIED_BY_INCLUDE;
            List<string> includeProperties = new List<string>();
            includeProperties.Add("title");
            includeProperties.Add("object_name");
            includeProperties.Add("r_object_type");
            Console.WriteLine("Explicitly filtering all properties to only included properties: title, object_name, r_object_type.");
            propertyProfile.IncludeProperties = includeProperties;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.PropertyProfile = propertyProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objIdentity);

            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);
            return dataPackage.DataObjects[0];
        }

        public DataObject GetObjectFilterPropertiesExclude(ObjectIdentity objIdentity)
        {
            PropertyProfile propertyProfile = new PropertyProfile();
            propertyProfile.FilterMode = PropertyFilterMode.SPECIFIED_BY_EXCLUDE;
            List<string> excludeProperties = new List<string>();
            excludeProperties.Add("title");
            excludeProperties.Add("object_name");
            excludeProperties.Add("r_object_type");
            Console.WriteLine("Explicitly filtering all properties by excluding title, object_name, r_object_type.");
            propertyProfile.ExcludeProperties = excludeProperties;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.PropertyProfile = propertyProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objIdentity);
            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);

            return dataPackage.DataObjects[0];
        }

        public DataObject GetObjectFilterRelationsParentOnly(ObjectIdentity objIdentity)
        {
            // set up relation profile to return only immediate parent
            RelationshipProfile relationProfile = new RelationshipProfile();
            relationProfile.ResultDataMode = ResultDataMode.REFERENCE;
            relationProfile.TargetRoleFilter = TargetRoleFilter.SPECIFIED;
            relationProfile.TargetRole = Relationship.ROLE_PARENT;
            relationProfile.NameFilter = RelationshipNameFilter.ANY;
            relationProfile.DepthFilter = DepthFilter.SPECIFIED;
            relationProfile.Depth = 1;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.RelationshipProfile = relationProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objIdentity);

            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);

            return dataPackage.DataObjects[0];
        }

        public DataObject GetWithPermissions(ObjectIdentity objectIdentity)
        {
            PermissionProfile permissionProfile = new PermissionProfile();
            permissionProfile.PermissionTypeFilter = PermissionTypeFilter.ANY;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.PermissionProfile = permissionProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet();
            List<ObjectIdentity> objIdList = objectIdSet.Identities;
            objIdList.Add(objectIdentity);

            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);
            return dataPackage.DataObjects[0];
        }

        public DataObject GetWithContent(ObjectIdentity objectIdentity, String geoLoc, ContentTransferMode transferMode)
        {
            ContentTransferProfile transferProfile = new ContentTransferProfile();
            transferProfile.Geolocation = geoLoc;
            transferProfile.TransferMode = transferMode;
            DemoServiceContext.SetProfile(transferProfile);

            ContentProfile contentProfile = new ContentProfile();
            contentProfile.FormatFilter = FormatFilter.ANY;
            contentProfile.UrlReturnPolicy = UrlReturnPolicy.PREFER;

            OperationOptions operationOptions = new OperationOptions();
            operationOptions.ContentProfile = contentProfile;
            operationOptions.SetProfile(contentProfile);

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet();
            List<ObjectIdentity> objIdList = objectIdSet.Identities;
            objIdList.Add(objectIdentity);

            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);
            DataObject dataObject = dataPackage.DataObjects[0];

            Content resultContent = dataObject.Contents[0];
            String contentClassName = resultContent.GetType().FullName;
            Console.WriteLine("Returned content as type " + contentClassName);
            if (contentClassName.Equals("Emc.Documentum.FS.DataModel.Core.Content.UrlContent"))
            {
                UrlContent urlContent = (UrlContent)resultContent;
                Console.WriteLine("Content ACS URL is: " + (urlContent.Url));
            }
            if (resultContent.CanGetAsFile())
            {
                FileInfo fileInfo = resultContent.GetAsFile();
                Console.WriteLine("Got content as file " + fileInfo.FullName);
            }
            else if (contentClassName.Equals("Emc.Documentum.FS.DataModel.Core.Content.UcfContent"))
            {
                UcfContent ucfContent = (UcfContent)resultContent;
                Console.WriteLine("Got content as file " + ucfContent.LocalFilePath);
            }
            return dataObject;
        }

        public DataObject GetFilterContentNone(ObjectIdentity objectIdentity)
        {
            ContentProfile contentProfile = new ContentProfile();
            contentProfile.FormatFilter = FormatFilter.NONE;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.ContentProfile = contentProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet();
            List<ObjectIdentity> objIdList = objectIdSet.Identities;
            objIdList.Add(objectIdentity);

            DataPackage dataPackage = objectService.Get(objectIdSet, operationOptions);
            return dataPackage.DataObjects[0];
        }

        public FileInfo GetObjectWithUrl(ObjectIdentity objIdentity)
        {
            objIdentity.RepositoryName = this.DefaultRepository;
            ObjectIdentitySet objectIdSet = new ObjectIdentitySet();
            List<ObjectIdentity> objIdList = objectIdSet.Identities;
            objIdList.Add(objIdentity);
            List<ObjectContentSet> urlList = objectService.GetObjectContentUrls(objectIdSet);
            ObjectContentSet objectContentSet = (ObjectContentSet)urlList[0];
            UrlContent urlContent =  (UrlContent)objectContentSet.Contents[0];
            if (urlContent.CanGetAsFile())
            {
                // downloads the file using the ACS URL
                FileInfo file = urlContent.GetAsFile();
                Console.WriteLine("File exists: " + file.Exists);
                Console.WriteLine("File full name: " + file.FullName);
                return file;
            }
            return null;
        }

        public DataObject GetFilterContentSpecific(String qualificationString)
        {
            ContentProfile contentProfile = new ContentProfile();
            contentProfile.FormatFilter = FormatFilter.SPECIFIED;
            contentProfile.Format = "gif";
            contentProfile.UrlReturnPolicy = UrlReturnPolicy.PREFER;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.ContentProfile = contentProfile;

            ObjectIdentity objectIdentity =
                new ObjectIdentity
                    (new Qualification(qualificationString), DefaultRepository);
            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objectIdentity);
            DataPackage dataPackage = null;

            dataPackage = objectService.Get(objectIdSet, operationOptions);

            DataObject dataObject = dataPackage.DataObjects[0];

            Content resultContent = dataObject.Contents[0];
            String contentClassName = resultContent.GetType().FullName;
            Console.WriteLine("Returned content as type " + contentClassName);
            if (contentClassName.Equals("Emc.Documentum.FS.DataModel.Core.Content.UrlContent"))
            {
                UrlContent urlContent = (UrlContent)resultContent;
                Console.WriteLine("Content ACS URL is: " + (urlContent.Url));
            }
            if (resultContent.CanGetAsFile())
            {
                FileInfo fileInfo = resultContent.GetAsFile();
                Console.WriteLine("Returned content format is " + resultContent.Format);
                Console.WriteLine("Got content as file " + fileInfo.FullName);
            }
            return dataObject;
        }

        public DataObject UpdateObject(DataObject dataObject)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.DataObjects.Add(dataObject);
            OperationOptions operationOptions = null;
            DataPackage resultDataPackage = objectService.Update(dataPackage, operationOptions);
            return resultDataPackage.DataObjects[0];
        }

        public DataPackage UpdateObjectProperties(ObjectIdentity objectIdentity,
                                              String newTitle,
                                              String newSubject,
                                              String[] newKeywords)
        {
            PropertyProfile propertyProfile = new PropertyProfile();

            // Setting the filter to ALL can cause errors if the DataObject
            // passed to the operation contains system properties, so to be safe 
            // set the filter to ALL_NON_SYSTEM unless you explicitly want to update
            // a system property
            propertyProfile.FilterMode = PropertyFilterMode.ALL_NON_SYSTEM;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.SetProfile(propertyProfile);

            DataObject dataObject = new DataObject(objectIdentity);

            PropertySet properties = new PropertySet();
            properties.Set("title", newTitle);
            properties.Set("subject", newSubject);
            properties.Set("keywords", newKeywords);
            dataObject.Properties = properties;

            return objectService.Update(new DataPackage(dataObject), operationOptions);
        }

        public DataPackage UpdateFetchedObjectProperties(ObjectIdentity objectIdentity,
                                                         String newTitle,
                                                         String newSubject,
                                                         String[] newKeywords)
        {
            PropertyProfile propertyProfile = new PropertyProfile();

            // Setting the filter to ALL can cause errors if the DataObject
            // passed to the operation contains system properties, so to be safe 
            // set the filter to ALL_NON_SYSTEM unless you explicitly want to update
            // a system property
            propertyProfile.FilterMode = PropertyFilterMode.ALL_NON_SYSTEM;
            DemoServiceContext.SetProfile(propertyProfile);

            OperationOptions operationOptions = null;
            DataPackage dP = objectService.Get(new ObjectIdentitySet(objectIdentity), operationOptions);
            DataObject dataObject = dP.DataObjects[0];

            PropertySet properties = new PropertySet();
            properties.Set("title", newTitle);
            properties.Set("subject", newSubject);
            properties.Set("keywords", newKeywords);
            dataObject.Properties = properties;

            return objectService.Update(new DataPackage(dataObject), operationOptions);
        }

        public void UpdateContent(ObjectIdentity objectIdentity, String newContentPath)
        {
            DataObject dataObject = new DataObject(objectIdentity, "dm_document");

            dataObject.Contents.Add(new FileContent(newContentPath, "gif"));

            OperationOptions operationOptions = null;
            objectService.Update(new DataPackage(dataObject), operationOptions);
        }

        public void UpdateProperties(ObjectIdentity objectIdentity, String newPropVal)
        {
            DataObject dataObject = new DataObject(objectIdentity, "dm_document");

            dataObject.Properties.Set("subject", newPropVal);

            OperationOptions operationOptions = null;
            objectService.Update(new DataPackage(dataObject), operationOptions);
        }

        public DataPackage UpdateRelinkFolder(ObjectIdentity docId,
                                              ObjectIdentity sourceFolderId,
                                              ObjectIdentity targetFolderId)
        {
            DataObject docDataObj = new DataObject(docId, "dm_document");

            // add the source folder as a parent relationship of the document
            ReferenceRelationship removeRelationship = new ReferenceRelationship();
            removeRelationship.TargetRole = Relationship.ROLE_PARENT;
            removeRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            removeRelationship.Target = sourceFolderId;
            docDataObj.Relationships.Add(removeRelationship);

            // specify that the folder is to be unlinked
            removeRelationship.IntentModifier = RelationshipIntentModifier.REMOVE;
            Console.WriteLine("Set to remove relationship from parent folder " + sourceFolderId.GetValueAsString());

            // add the folder into which to link document
            ReferenceRelationship addRelationship = new ReferenceRelationship();
            addRelationship.TargetRole = Relationship.ROLE_PARENT;
            addRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            addRelationship.Target = targetFolderId;
            docDataObj.Relationships.Add(addRelationship);
            Console.WriteLine("Set relationship to parent folder " + targetFolderId.GetValueAsString());

            OperationOptions operationOptions = null;
            return objectService.Update(new DataPackage(docDataObj), operationOptions);
        }

        public DataPackage UpdateRepeatProperty(ObjectIdentity objectIdentity)
        {
            PropertyProfile propertyProfile = new PropertyProfile();

            // Setting the filter to ALL can cause errors if the DataObject
            // passed to the operation contains system properties, so to be safe 
            // set the filter to ALL_NON_SYSTEM unless you explicitly want to update
            // a system property
            propertyProfile.FilterMode = PropertyFilterMode.ALL_NON_SYSTEM;
            DemoServiceContext.SetProfile(propertyProfile);

            DataObject dataObject = new DataObject(objectIdentity);

            String[] moreDangers = { "snakes", "sharks" };
            ArrayProperty<string> keywordProperty = new StringArrayProperty("keywords", moreDangers);
            Console.WriteLine("Appending additional strings 'snakes' and 'sharks' to object keywords.");

            ValueAction appendAction = new ValueAction(ValueActionType.INSERT, 0);
            ValueAction deleteAction = new ValueAction(ValueActionType.APPEND, 1);
            keywordProperty.SetValueActions(appendAction, deleteAction);
            PropertySet properties = new PropertySet();
            properties.Set(keywordProperty);
            dataObject.Properties = properties;

            OperationOptions operationOptions = null;
            return objectService.Update(new DataPackage(dataObject), operationOptions);
        }
    }
}

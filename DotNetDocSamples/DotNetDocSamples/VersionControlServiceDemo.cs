using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Services.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class VersionControlServiceDemo : DemoBase
    {
        private IVersionControlService versionControlService;

        public VersionControlServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password) 
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            versionControlService =
                serviceFactory.GetRemoteService<IVersionControlService>(DemoServiceContext);
        }

        public CheckoutInfo CheckoutInfo(ObjectIdentity objIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);
            List<CheckoutInfo> objList;
            OperationOptions operationOptions = null;
            versionControlService.Checkout(objIdSet, operationOptions);
            objList = versionControlService.GetCheckoutInfo(objIdSet);
            CheckoutInfo checkoutInfo = objList[0];

            if (checkoutInfo.IsCheckedOut)
            {
                Console.WriteLine("Object "
                                   + checkoutInfo.Identity
                                   + " is checked out.");
                Console.WriteLine("Lock owner is " + checkoutInfo.UserName);
            }
            else
            {
                Console.WriteLine("Object "
                                   + checkoutInfo.Identity
                                   + " is not checked out.");
            }
            versionControlService.CancelCheckout(objIdSet);
            return checkoutInfo;
        }

        public DataPackage Checkout(ObjectIdentity objIdentity)
        {

            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            OperationOptions operationOptions = null;
            DataPackage resultDp;
            
            resultDp = versionControlService.Checkout(objIdSet, operationOptions);
            Console.WriteLine("Checkout successful");

            List<VersionInfo> vInfo = versionControlService.GetVersionInfo(objIdSet);
            VersionInfo versionInfo = vInfo[0];

            Console.WriteLine("Printing version info for " + versionInfo.Identity);
            Console.WriteLine("IsCurrent is " + versionInfo.IsCurrent);
            Console.WriteLine("Version is " + versionInfo.Version);
            Console.WriteLine("Symbolic labels are: ");
            foreach (String label in versionInfo.SymbolicLabels)
            {
                Console.WriteLine(label);
            }

            versionControlService.CancelCheckout(objIdSet);
            Console.WriteLine("Checkout cancelled");
            return resultDp;
        }

        public DataPackage Checkin(ObjectIdentity objIdentity, String newContentPath)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            OperationOptions operationOptions = new OperationOptions();
            ContentProfile contentProfile = new ContentProfile(FormatFilter.ANY, null,
                                                               PageFilter.ANY,
                                                               -1,
                                                               PageModifierFilter.ANY, null);
            operationOptions.ContentProfile = contentProfile;

            DataPackage checkinPackage = versionControlService.Checkout(objIdSet, operationOptions);

            DataObject checkinObj = checkinPackage.DataObjects[0];

            checkinObj.Contents = null;
            FileContent newContent = new FileContent();
            newContent.LocalPath = newContentPath;
            newContent.RenditionType = RenditionType.PRIMARY;
            newContent.Format = "gif";
            checkinObj.Contents.Add(newContent);

            bool retainLock = false;
            List<String> labels = new List<String>();
            labels.Add("test_version");
            DataPackage resultDp;
            try
            {
                resultDp = versionControlService.Checkin(checkinPackage,
                                              VersionStrategy.NEXT_MINOR,
                                              retainLock,
                                              labels,
                                              operationOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw new Exception(e.Message);
            }
            return resultDp;
        }

        public void CancelCheckout(ObjectIdentity objIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            versionControlService.CancelCheckout(objIdSet);
            Console.WriteLine("Checkout cancelled.");
        }

        public DataObject GetCurrentDemo(ObjectIdentity objIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            OperationOptions operationOptions = null;
            DataPackage resultDataPackage = versionControlService.GetCurrent(objIdSet, operationOptions);
            return resultDataPackage.DataObjects[0];
        }

        public void DeleteVersionDemo(ObjectIdentity objIdentity)
        {
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            versionControlService.DeleteVersion(objIdSet);
        }

        public void DeleteAllVersionsDemoQual()
        {
            string nonCurrentQual = "dm_document (ALL) " +
                                    "where object_name = 'DFS_sample_image' " +
                                    "and ANY r_version_label = 'test_version'";
            Qualification qual = new Qualification(nonCurrentQual);
            ObjectIdentity objIdentity = new ObjectIdentity();
            objIdentity.Value = qual;
            objIdentity.RepositoryName = DefaultRepository;
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            versionControlService.DeleteAllVersions(objIdSet);
        }

        public void VersionInfoDemoQual(String nonCurrentQual)
        {
            Qualification qual = new Qualification(nonCurrentQual);
            ObjectIdentity objIdentity = new ObjectIdentity();
            objIdentity.Value = qual;
            objIdentity.RepositoryName = DefaultRepository;
            ObjectIdentitySet objIdSet = new ObjectIdentitySet();
            objIdSet.Identities.Add(objIdentity);

            List<VersionInfo> vInfo = versionControlService.GetVersionInfo(objIdSet);
            VersionInfo versionInfo = vInfo[0];

            Console.WriteLine("Printing version info for " + versionInfo.Identity);
            Console.WriteLine("isCurrent is " + versionInfo.IsCurrent);
            Console.WriteLine("Version is " + versionInfo.Version);

            Console.WriteLine("Symbolic labels are: ");
            foreach (string label in versionInfo.SymbolicLabels)
            {
                Console.WriteLine(label);
            }
        }

    }
}

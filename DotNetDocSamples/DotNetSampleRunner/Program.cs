using System;
using System.Collections.Generic;

namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            RunQueryServiceDemos();
            RunObjectServiceDemos();
            RunVersionControlServiceDemos();
            RunSchemaServiceDemos();
            RunSearchServiceDemos();
            RunWorkflowServiceDemos();
            RunAccessControlServiceDemos();
            RunLifecycleServiceDemos();
            RunVirtualDocumentServiceDemos();
        }

        private static void RunQueryServiceDemos()
        {
            TQueryServiceDemo demoRunner = new TQueryServiceDemo();
            demoRunner.TestBasicPassthroughQuery();
            demoRunner.TestCacheQuery();
        }

        private static void RunObjectServiceDemos()
        {
            TObjectServiceDemo demoRunner = new TObjectServiceDemo();
            demoRunner.TestCreateAndLinkToFolder();
            demoRunner.TestCreateContentless();
            demoRunner.TestCreateFolderAndLinkedDoc();
            demoRunner.TestCreateNewFolder();
            demoRunner.TestCreatePath();
            demoRunner.TestCreateWithContentDefaultContext();
            demoRunner.TestCreateWithContentModifyContext();
            demoRunner.TestGetFilterContentNone();
            demoRunner.TestGetFilterContentSpecific();
            demoRunner.TestGetFilterProperties();
            demoRunner.TestGetFilterPropertiesExclude();
            demoRunner.TestGetFilterPropertiesInclude();
            demoRunner.TestGetObjectDefaultsObjPath();
            demoRunner.TestGetObjectFilterRelationParentOnly();
            demoRunner.TestGetWithPermissions();
            demoRunner.TestMTOM();
            demoRunner.TestBase64();
            demoRunner.TestGetWithUrl();
            demoRunner.TestObjCopy();
            demoRunner.TestObjCopyAcrossRepositories();
            demoRunner.TestObjCopyWithMods();
            demoRunner.TestObjDelete();
            demoRunner.TestObjMove();
            demoRunner.TestUpdateContent();
            demoRunner.TestUpdateProperties();
            demoRunner.TestUpdatePropertiesFetched();
            demoRunner.TestUpdateRelinkFolder();
            demoRunner.TestUpdateRepeatProperty();

        }

        private static void RunSchemaServiceDemos()
        {
            TSchemaServiceDemo demoRunner = new TSchemaServiceDemo();
            demoRunner.TestTypeInfo();
            demoRunner.TestDemoPropertyInfo();
            demoRunner.TestRepositoryInfo();
        }

        private static void RunSearchServiceDemos()
        {
            TSearchServiceDemo demoRunner = new TSearchServiceDemo();
            demoRunner.TestGetRepositoryList();
            demoRunner.TestPassthroughQuery();
            demoRunner.TestStructuredQuery();
        }

        private static void RunVersionControlServiceDemos()
        {
            TVersionControlServiceDemo demoRunner = new TVersionControlServiceDemo();
            demoRunner.TestCancelCheckout();
            demoRunner.TestCheckin();
            demoRunner.TestCheckoutInfo();
            demoRunner.TestCheckoutInfoNonCurrent();
            demoRunner.TestGetCurrent();
            demoRunner.TestVersionInfoQual();
        }

        // the Workflow demos are dependent on a Workflow SBO that must be available from the global registry
        // the global registry must be referenced in the dfc.properties used by DFS
        private static void RunWorkflowServiceDemos()
        {
            TWorkflowServiceDemo demoRunner = new TWorkflowServiceDemo();
            demoRunner.TestGetProcessInfo();
            demoRunner.TestGetProcessTemplates();
        }

        private static void RunAccessControlServiceDemos()
        {
            TAccessControlServiceDemo demoRunner = new TAccessControlServiceDemo();
            demoRunner.testAclCreate();
            demoRunner.testAclDelete();
            demoRunner.testAclGet();
            demoRunner.testAclQuery();
            demoRunner.testAclUpdate();
        }

        private static void RunLifecycleServiceDemos()
        {
            TLifecycleServiceDemo demoRunner = new TLifecycleServiceDemo();
            demoRunner.TestGetLifecycles();
            demoRunner.TestAttachLifecycle();
            demoRunner.TestShowLifecycle();
            demoRunner.TestPromote();
            demoRunner.TestDemoteToBase();
            demoRunner.TestDetachLifecycle();
        }

        private static void RunVirtualDocumentServiceDemos()
        {
            TVirtualDocumentServiceDemo demoRunner = new TVirtualDocumentServiceDemo();
            demoRunner.TestAddChildNodes();
            demoRunner.TestRetrieveVdmInfo();
            demoRunner.TestCreateSnapshot();
            demoRunner.TestCreateInlineSnapshot();
            demoRunner.TestRemoveSnapshot();
        }
    }
}
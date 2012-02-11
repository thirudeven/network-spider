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
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Properties;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TLifecycleServiceDemo : TestBase
    {
        private LifecycleServiceDemo lifecycleServiceDemo;
        private SampleContentManager sampleContentManager;

        public TLifecycleServiceDemo()
        {
            try
            {
                lifecycleServiceDemo = new LifecycleServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                Console.WriteLine("Lifecycle Service Test"
                + "You must install the BusinessPolicy1 lifecycle (you can use Composer) "
                + "before this sample will work. The lifecycle owner_name must be the login client.");

                sampleContentManager = new SampleContentManager(lifecycleServiceDemo);
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

        public void TestGetLifecycles()
        {
            try
            {
                Console.WriteLine("\nTesting get lifecycles...");

                DataPackage dp = lifecycleServiceDemo.GetLifecycles(DefaultRepository);
                Console.WriteLine("Returned " + dp.DataObjects.Count + " lifecycles.");
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

        public void TestAttachLifecycle()
        {
            try
            {
                Console.WriteLine("\nTesting lifecycle attach...");
                sampleContentManager.CreateDemoObjects();

                attachTestLifecycle();

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

        public void TestShowLifecycle()
        {
            try
            {
                Console.WriteLine("\nTesting lifecycle info...");
                sampleContentManager.CreateDemoObjects();
                attachTestLifecycle();

                ObjectIdentity objId = new ObjectIdentity();
                objId.RepositoryName = DefaultRepository;
                objId.Value = new Qualification(SampleContentManager.gifImageQualString);

                lifecycleServiceDemo.ShowLifecycleInfo(objId);

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

        public void TestPromote()
        {
            try
            {
                Console.WriteLine("\nTesting lifecycle promote...");
                sampleContentManager.CreateDemoObjects();
                attachTestLifecycle();

                ObjectIdentity objId = new ObjectIdentity();
                objId.RepositoryName = DefaultRepository;
                objId.Value = new Qualification(
                        SampleContentManager.gifImageQualString);
                lifecycleServiceDemo.PromoteLifecycle(objId);
                lifecycleServiceDemo.ShowLifecycleInfo(objId);

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

        public void TestDemoteToBase()
        {
            try
            {
                Console.WriteLine("\nTesting lifecycle execute demote to base...");
                sampleContentManager.CreateDemoObjects();
                attachTestLifecycle();

                ObjectIdentity objId = new ObjectIdentity();
                objId.RepositoryName = DefaultRepository;
                objId.Value = new Qualification(
                        SampleContentManager.gifImageQualString);

                lifecycleServiceDemo.PromoteLifecycle(objId);
                lifecycleServiceDemo.PromoteLifecycle(objId);
                lifecycleServiceDemo.DemoteLifecycleToBase(objId);
                lifecycleServiceDemo.ShowLifecycleInfo(objId);

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

        public void TestDetachLifecycle()
        {
            try
            {
                Console.WriteLine("\nTesting lifecycle detach...");
                sampleContentManager.CreateDemoObjects();
                attachTestLifecycle();

                ObjectIdentity objId = new ObjectIdentity();
                objId.RepositoryName = DefaultRepository;
                objId.Value = new Qualification(
                        SampleContentManager.gifImageQualString);

                lifecycleServiceDemo.DetachLifecycle(objId);
                Console.WriteLine("Detached lifecycle from document: "
                        + SampleContentManager.gifImageQualString);
                lifecycleServiceDemo.ShowLifecycleInfo(objId);

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

        private void attachTestLifecycle()
        {
            ObjectIdentity objId = new ObjectIdentity();
            objId.RepositoryName = DefaultRepository;
            objId.Value = new Qualification(SampleContentManager.gifImageQualString);

            ObjectIdentity policyId = new ObjectIdentity();
            policyId.RepositoryName = DefaultRepository;
            String lifecycleQualString = "dm_policy where object_name='BusinessPolicy1' and owner_name='"
                    + sampleContentManager.GetUserNameFromRepository() + "'";
            policyId.Value = new Qualification(lifecycleQualString);

            lifecycleServiceDemo.AttachLifecycle(objId, policyId, null);
            Console.WriteLine("Completed attach of lifecycle.");
            Console.WriteLine("Lifecycle: " + lifecycleQualString);
            Console.WriteLine("Attached to document: "
                    + SampleContentManager.gifImageQualString);
        }
    }
}

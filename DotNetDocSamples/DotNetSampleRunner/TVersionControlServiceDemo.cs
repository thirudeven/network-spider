using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TVersionControlServiceDemo : TestBase
    {

        public TVersionControlServiceDemo()
        {
            try
            {
                versionControlServiceDemo = new VersionControlServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(versionControlServiceDemo);
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
       
        
        public void TestCheckout()
        {
            Console.WriteLine("\nRunning version control checkout sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity();
                objIdentity.RepositoryName = versionControlServiceDemo.DefaultRepository;
                objIdentity.Value = new ObjectPath(SampleContentManager.gifImage1ObjPath);
                DataPackage resultDataPackage = versionControlServiceDemo.Checkout(objIdentity);
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

        
        public void TestCancelCheckout()
        {
            Console.WriteLine("\nRunning version control cancel checkout sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity();
                objIdentity.RepositoryName = versionControlServiceDemo.DefaultRepository;
                objIdentity.Value = new ObjectPath(SampleContentManager.gifImage1ObjPath);
                versionControlServiceDemo.CancelCheckout(objIdentity);
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

        
        public void TestCheckoutInfo()
        {
            Console.WriteLine("\nRunning version control checkout info sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity();
                objIdentity.RepositoryName = versionControlServiceDemo.DefaultRepository;
                objIdentity.Value = new ObjectPath(SampleContentManager.gifImage1ObjPath);
                versionControlServiceDemo.CheckoutInfo(objIdentity);
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

        
        public void TestGetCurrent()
        {
            Console.WriteLine("\nRunning version get current version sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity();
                objIdentity.RepositoryName = versionControlServiceDemo.DefaultRepository;
                objIdentity.Value = new ObjectPath(SampleContentManager.gifImage1ObjPath);
                versionControlServiceDemo.GetCurrentDemo(objIdentity);
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

        
        public void TestCheckin()
        {
            Console.WriteLine("\nRunning version control checkin sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity();
                objIdentity.RepositoryName = versionControlServiceDemo.DefaultRepository;
                objIdentity.Value = new ObjectPath(SampleContentManager.gifImage1ObjPath);
                versionControlServiceDemo.GetCurrentDemo(objIdentity);
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

        
        public void TestCheckoutNonCurrent()
        {
            Console.WriteLine("\nRunning version control checkout of non-current version sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity(new Qualification(nonCurrentQual), 
                                                                versionControlServiceDemo.DefaultRepository);
                DataPackage dp = versionControlServiceDemo.Checkout(objIdentity);
                DataObject dO = dp.DataObjects[0];
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

        
        public void TestCheckoutInfoNonCurrent()
        {
            Console.WriteLine("\nRunning version control non-current version info sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                ObjectIdentity objIdentity = new ObjectIdentity(new Qualification(nonCurrentQual),
                                                               versionControlServiceDemo.DefaultRepository);
                versionControlServiceDemo.CheckoutInfo(objIdentity);
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

        
        public void TestVersionInfoQual()
        {
            Console.WriteLine("\nRunning version control version info from qualification sample.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                sampleContentManager.VersionSampleObjects();
                versionControlServiceDemo.VersionInfoDemoQual(nonCurrentQual);
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

 
        public SampleContentManager SampleContentManager
        {
            get { return sampleContentManager; }
            set { sampleContentManager = value; }
        }

        private VersionControlServiceDemo versionControlServiceDemo;
        private SampleContentManager sampleContentManager;

        private string nonCurrentQual = "dm_document (ALL) " +
                                 "where object_name = 'DFS_sample_image' " +
                                 "and ANY r_version_label = 'test_version'";
    }
}

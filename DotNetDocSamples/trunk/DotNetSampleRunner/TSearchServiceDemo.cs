using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.Runtime;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TSearchServiceDemo : TestBase
    {

        public TSearchServiceDemo()
        {
            try
            {
                searchServiceDemo = new SearchServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(searchServiceDemo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Failed with exception " + e.Message);
            }
        }   
        
        public void TestGetRepositoryList()
        {
            Console.WriteLine("\nRunning get repository list.");
            try
            {
                searchServiceDemo.RepositoryList();
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

        
        public void TestPassthroughQuery()
        {
            Console.WriteLine("\nRunning search service passthrough query.");
            try
            {
                searchServiceDemo.SimplePassthroughQuery();
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

        
        public void TestStructuredQuery()
        {
            Console.WriteLine("\nRunning search service structured query.");
            try
            {
                sampleContentManager.CreateDemoObjects();
                searchServiceDemo.SimpleStructuredQuery(SampleContentManager.gifImage1ObjectName);
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

            private SearchServiceDemo searchServiceDemo;
            private SampleContentManager sampleContentManager;
        } 
}

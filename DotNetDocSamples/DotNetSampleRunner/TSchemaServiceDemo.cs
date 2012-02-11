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
    public class TSchemaServiceDemo : TestBase
    {
        public TSchemaServiceDemo()
        {
            try
            {
                schemaServiceDemo = new SchemaServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(schemaServiceDemo);
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

        public void TestDemoPropertyInfo()
        {
            Console.WriteLine("\nTesting SchemaService property info.");
            try
            {
                schemaServiceDemo.PropertyInfoDemo();
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

        
        public void TestRepositoryInfo()
        {
            Console.WriteLine("\nTesting SchemaService repository info.");
            try
            {
                schemaServiceDemo.RepositoryInfoDemo();
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

        public void TestTypeInfo()
        {
            Console.WriteLine("\nTesting SchemaService type info.");
            try
            {
                schemaServiceDemo.TypeInfoDemo();
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

        
        public void TestValueInfo()
        {
            Console.WriteLine("\nTesting SchemaService value info.");
            try
            {
                schemaServiceDemo.ValueInfoDemo();
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

        private SampleContentManager sampleContentManager;
        private SchemaServiceDemo schemaServiceDemo;
    }
}

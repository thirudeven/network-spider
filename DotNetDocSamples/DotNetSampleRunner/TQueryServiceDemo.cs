using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Doc.Samples.Client;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TQueryServiceDemo : TestBase
    {     
        public TQueryServiceDemo()
        {
            try
            {
                queryServiceDemo = new QueryServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
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
        
        public void TestBasicPassthroughQuery()
        {
            try
            {
                Console.WriteLine("\nBasic passthrough query demo.");
                queryServiceDemo.BasicPassthroughQuery();
                Console.WriteLine("Passthrough query demo completed successfully.");
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
    
        public void TestCacheQuery()
        {
            try
            {
                Console.WriteLine("\nCache query demo.");
               queryServiceDemo.CachedPassthroughQuery();
               Console.WriteLine("Cached passthrough query demo completed successfully.");
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

        private QueryServiceDemo queryServiceDemo;
    }
}

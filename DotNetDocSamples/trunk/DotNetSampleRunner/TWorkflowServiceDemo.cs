using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Doc.Samples.Client;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TWorkflowServiceDemo : TestBase
    {
        public TWorkflowServiceDemo()
        {
            try
            {
                workflowServiceDemo = new WorkflowServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
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
       
        public void TestGetProcessTemplates()
        {
            Console.WriteLine("\nRunning workflow get process templates.");
            try
            {
                workflowServiceDemo.processTemplates();
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

        public void TestGetProcessInfo()
        {
            Console.WriteLine("\nRunning workflow get process info.");
            try
            {
                DataPackage resultDp = workflowServiceDemo.processTemplates();
                DataObject dO = resultDp.DataObjects[0];
                ObjectIdentity objIdentity = dO.Identity;
                workflowServiceDemo.processInfo(objIdentity);
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

            private WorkflowServiceDemo workflowServiceDemo;
        } 
}

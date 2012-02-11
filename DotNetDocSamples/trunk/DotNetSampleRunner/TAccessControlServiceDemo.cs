using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;

using Emc.Documentum.FS.Doc.Samples.Client;
using Emc.Documentum.FS.DataModel.Core.Acl;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Runtime;


namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TAccessControlServiceDemo : TestBase
    {
        public TAccessControlServiceDemo()
        {
            try
            {
                accessControlServiceDemo = new AccessControlServiceDemo(DefaultRepository, SecondaryRepository, UserLoginName, Password);
                sampleContentManager = new SampleContentManager(accessControlServiceDemo);
                userName = sampleContentManager.GetUserNameFromRepository();
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

        public void testAclCreate()
        {
            Console.WriteLine("\nTesting Acl create...");
            AclIdentity aclIdentity = null;
            try
            {
                
                AclPackage aclPackage = accessControlServiceDemo.CreateAcl(
                        DefaultRepository, userName, "MyAcl"
                                + System.DateTime.Now.Ticks);
                aclIdentity = aclPackage.Acls[0].Identity;
                Console.WriteLine("Created acl " + aclIdentity.Name);
                Console.WriteLine("testAclCreate completed");
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
                if (aclIdentity != null)
                {
                    sampleContentManager.DeleteSampleAcl(aclIdentity);
                }
            }
        }

        public void testAclGet()
        {
            Console.WriteLine("\nTesting Acl get...");
            AclIdentity aclIdentity = null;
            try
            {       
                String aclName = "MyAcl" + System.DateTime.Now.Ticks;
                AclPackage aclPackage = accessControlServiceDemo.CreateAcl(
                        DefaultRepository, userName, aclName);
                aclIdentity = aclPackage.Acls[0].Identity;
                Console.WriteLine("Created acl: "
                        + aclIdentity.Name);

                aclPackage = accessControlServiceDemo.GetAcl(DefaultRepository,
                        userName, aclName);
                Console.WriteLine("Got ACL: "
                        + aclPackage.Acls[0].Identity.Name);
                Console.WriteLine("testAclGet completed");
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
                if (aclIdentity != null)
                {
                    sampleContentManager.DeleteSampleAcl(aclIdentity);
                }
            }
        }

        public void testAclUpdate()
        {
            Console.WriteLine("\nTesting Acl update...");
            AclIdentity aclIdentity = null;
            try
            {  
                aclIdentity = new AclIdentity();
                aclIdentity.RepositoryName = DefaultRepository;
                aclIdentity.Domain = userName;
                aclIdentity.Name = "MyAcl" + System.DateTime.Now.Ticks;
                AclPackage aclPackage = accessControlServiceDemo.CreateAcl(
                        DefaultRepository, userName, "MyAcl"
                                + System.DateTime.Now.Ticks);
                aclIdentity = aclPackage.Acls[0].Identity;
                Console.WriteLine("Created acl: " + aclIdentity.Name);

                aclPackage = accessControlServiceDemo
                        .UpdateAcl(aclIdentity, userName);
                Console.WriteLine("Updated ACL: "
                        + aclPackage.Acls[0].Identity.Name);
                Console.WriteLine("testAclUpdate completed");
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
                if (aclIdentity != null)
                {
                    sampleContentManager.DeleteSampleAcl(aclIdentity);
                }
            }
        }

        public void testAclDelete()
        {
            Console.WriteLine("\nTesting Acl delete...");
            AclIdentity aclIdentity = null;
            try
            {
                String aclName = "MyAcl" + System.DateTime.Now.Ticks;
                AclPackage aclPackage = accessControlServiceDemo.CreateAcl(
                        DefaultRepository, userName, aclName);
                aclIdentity = aclPackage.Acls[0].Identity;
                Console.WriteLine("Created acl: "
                        + aclIdentity.Name);

                accessControlServiceDemo.DeleteAcl(DefaultRepository, userName, aclName);
                Console.WriteLine("Deleted ACL " + aclName);
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
                if (aclIdentity != null)
                {
                    sampleContentManager.DeleteSampleAcl(aclIdentity);
                }
            }
        }

        public void testAclQuery()
	    {
            Console.WriteLine("\nTesting Acl query...");
            AclIdentity aclIdentity = null;
		    try
		    {
                String aclName = "MyAcl" + System.DateTime.Now.Ticks;
                AclPackage createdAclPackage = accessControlServiceDemo.CreateAcl(
                        DefaultRepository, userName, aclName);
                aclIdentity = createdAclPackage.Acls[0].Identity;
                Console.WriteLine("Created acl: "
                        + aclIdentity.Name);
			    AclPackage aclPackage = accessControlServiceDemo.GetOwnerPrivateAcls(
			            userName, DefaultRepository);
			    List<Acl> aclList = aclPackage.Acls;
			    if (aclList==null)
			    {
				    Console.WriteLine("Query returned no results.");
				    return;
			    }
			    Console.WriteLine("Contents of AclPackage: ");
			    foreach (Acl acl in aclList)
			    {
                    Console.WriteLine(aclPackage.Acls[0].Identity.Name);
			    }
			    Console.WriteLine("testAclDelete completed");
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
                if (aclIdentity != null)
                {
                    sampleContentManager.DeleteSampleAcl(aclIdentity);
                }
            }
	    }


        private AccessControlServiceDemo accessControlServiceDemo;
        private SampleContentManager sampleContentManager;
        private String userName;
    }
}

using System;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.Runtime;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string moduleName = "example";
            string contextRoot = "http://192.168.200.9:9080/services";

            ContextFactory contextFactory = ContextFactory.Instance;
            IServiceContext context = contextFactory.NewContext();
            RepositoryIdentity repoId = new RepositoryIdentity();
            repoId.RepositoryName = "MOH_TST";
            repoId.UserName = "dmadmin";
            repoId.Password = "dm.admin";


            context.AddIdentity(repoId);

            IHelloWorldService mySvc;
            try
            {
                context = contextFactory.Register(context, moduleName, contextRoot);
                ServiceFactory serviceFactory = ServiceFactory.Instance;
                mySvc = serviceFactory.GetRemoteService<IHelloWorldService>(context, moduleName, contextRoot);
                string response = mySvc.SayHello("friend");
                Console.WriteLine("response = " + response);
            }
           // catch (FaultException<SerializableException> ex)
            //{
            //    Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            //}
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
        }
    }
}

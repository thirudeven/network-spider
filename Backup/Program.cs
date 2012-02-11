using System;
using Example.Hello;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.Runtime.Context;
using System.ServiceModel;
using Emc.Documentum.FS.Runtime;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string moduleName = "example";
            string contextRoot = "http://localhost:8080/services";

            ContextFactory contextFactory = ContextFactory.Instance;
            IServiceContext context = contextFactory.NewContext();
            RepositoryIdentity repoId = new RepositoryIdentity();
            repoId.RepositoryName = "YOUR_REPOSITORY_NAME";
            repoId.UserName = "YOUR_USER_NAME";
            repoId.Password = "YOUR_PASSWORD";


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
            catch (FaultException<SerializableException> ex)
            {
                Console.WriteLine(String.Format("Got FaultException[{0}] with message: {1}\n", ex.Detail, ex.Message));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.StackTrace);
            }
        }
    }
}

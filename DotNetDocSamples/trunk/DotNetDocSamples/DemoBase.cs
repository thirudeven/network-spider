using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public abstract class DemoBase     
    {

        public DemoBase(String defaultRepository, String secondaryRepository, String userName, String password)
        {
            DefaultRepository = defaultRepository;
            SecondaryRepository = secondaryRepository;
            UserName = userName;
            Password = password;

            initializeContext();
        }

        public string DefaultRepository
        {
            get { return this.defaultDocbase; }
            set { this.defaultDocbase = value; }
        }

        public string SecondaryRepository
        {
            get { return this.secondaryDocbase; }
            set { this.secondaryDocbase = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public IServiceContext DemoServiceContext
        {
            get { return this.serviceContext; }
        }

        private void initializeContext()
        {
            ContextFactory contextFactory = ContextFactory.Instance;
            serviceContext = contextFactory.NewContext();

            RepositoryIdentity repoId = new RepositoryIdentity();
            RepositoryIdentity repositoryIdentity = 
                new RepositoryIdentity(DefaultRepository, UserName, Password, "");
            serviceContext.AddIdentity(repositoryIdentity);

            ContentTransferProfile contentTransferProfile = new ContentTransferProfile();
            contentTransferProfile.TransferMode = ContentTransferMode.MTOM;
            contentTransferProfile.Geolocation = "Pleasanton";
            serviceContext.SetProfile(contentTransferProfile);

            PropertyProfile propertyProfile = new PropertyProfile();
            propertyProfile.FilterMode = PropertyFilterMode.ALL_NON_SYSTEM;
            serviceContext.SetProfile(propertyProfile);

            if (SecondaryRepository != null)
            {
                RepositoryIdentity repoId1 = new RepositoryIdentity();
                repoId1.RepositoryName = SecondaryRepository;
                repoId1.UserName = UserName;
                repoId1.Password = Password;
                serviceContext.AddIdentity(repoId1);
            }
        }

        private string defaultDocbase;
        private string secondaryDocbase;
        private string userName;
        private string password;
        private IServiceContext serviceContext;

    }
}

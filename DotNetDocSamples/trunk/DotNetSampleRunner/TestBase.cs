using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Emc.Documentum.FS.Doc.Samples.Client.Runner
{
    public class TestBase
    {
        // TODO: You must supply valid values for the following variables
        private string defaultRepository = "MOH_TST";

        // set this to null if you are not using a second repository
        // a second repository is required only  to demonstrate copying an object across repositories
        // the samples expect login to both repositories using the same credentials
        private string secondaryRepository = null;

        private string userLoginName = "dmadmin";
        private string password = "dm.admin";


        public string UserLoginName
        {
            get { return userLoginName; }
            set { userLoginName = value; }
        }
       

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        

        public string DefaultRepository
        {
            get { return defaultRepository; }
            set { defaultRepository = value; }
        }
        

        public string SecondaryRepository
        {
            get { return secondaryRepository; }
            set { secondaryRepository = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Services.Core.Acl;
using Emc.Documentum.FS.DataModel.Core.Acl;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime;
using Emc.Documentum.FS.Runtime.Context;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class AccessControlServiceDemo : DemoBase
    {
        private IAccessControlService accessControlService;

        public AccessControlServiceDemo(String defaultRepository, String secondaryRepository, String userName, String password)
            : base(defaultRepository, secondaryRepository, userName, password)
        {
            ServiceFactory serviceFactory = ServiceFactory.Instance;
            accessControlService =
                serviceFactory.GetRemoteService<IAccessControlService>(DemoServiceContext);
        }

       	public AclPackage CreateAcl(String repoName, String userName, String aclName)
	{
		AclIdentity aclIdentity = new AclIdentity();
		aclIdentity.RepositoryName = repoName;
		aclIdentity.Domain = userName;
		aclIdentity.Name = aclName;

		Permission basicReadPermission = new Permission();
		basicReadPermission.Name = Permission.READ;
		basicReadPermission.Type = PermissionType.BASIC;

		Permission basicDeletePermission = new Permission();
		basicDeletePermission.Name = Permission.DELETE;
		basicDeletePermission.Type = PermissionType.BASIC;

		List<AclEntry> entryList = new List<AclEntry>();

		AclEntry aclEntry = new AclEntry();
		aclEntry.AccessType = AccessType.PERMIT;
		aclEntry.Accessor = "dm_world";
		List<Permission> permissionList = new List<Permission>();
		permissionList.Add(basicReadPermission);
		aclEntry.Permissions = permissionList;

		AclEntry aclEntry1 = new AclEntry();
		aclEntry1.AccessType = AccessType.PERMIT;
		aclEntry1.Accessor = "dm_owner";
		List<Permission> permissionList1 = new List<Permission>();
		permissionList1.Add(basicDeletePermission);
		aclEntry1.Permissions = permissionList1;

		entryList.Add(aclEntry);
		entryList.Add(aclEntry1);

		Acl acl = new Acl();
		acl.Identity = aclIdentity;
		acl.Description = "a test acl" + System.DateTime.Now.Ticks;
		// acl.setType(AclType.REGULAR); // defaults to REGULAR
		// acl.setVisibility(AclVisibility.PRIVATE); // defaults to PRIVATE
		acl.Entries = entryList;

		AclPackage aclPackage = new AclPackage();
		List<Acl> aclList = new List<Acl>();
		aclList.Add(acl);
		aclPackage.Acls = aclList;

		return accessControlService.Create(aclPackage);
	}

	public AclPackage GetAcl(String repository, String domain, String name)
	{
		AclIdentity aclIdentity = new AclIdentity();
		aclIdentity.RepositoryName = repository;
		aclIdentity.Domain = domain;
		aclIdentity.Name = name;
		List<AclIdentity> aclIdentityList = new List<AclIdentity>();
		aclIdentityList.Add(aclIdentity);
		return accessControlService.Get(aclIdentityList);
	}

	public AclPackage UpdateAcl(AclIdentity aclIdentity, String userName)
	{
		List<AclIdentity> idList = new List<AclIdentity>();
		idList.Add(aclIdentity);
		AclPackage aclPackage = accessControlService.Get(idList);

		AclEntry aclEntry = new AclEntry();
		aclEntry.Accessor = userName;
		Permission basicDeletePermission = new Permission();
		basicDeletePermission.Name = Permission.DELETE;
		basicDeletePermission.Type = PermissionType.BASIC;
		aclEntry.AccessType = AccessType.PERMIT;
		List<Permission> permissionList = new List<Permission>();
		permissionList.Add(basicDeletePermission);
		aclEntry.Permissions = permissionList;

		Acl acl = aclPackage.Acls[0];
		acl.Entries.Add(aclEntry);

		return accessControlService.Update(aclPackage);
	}

	public void DeleteAcl(String repository, String domain, String name)
	{
		AclIdentity aclIdentity = new AclIdentity();
		aclIdentity.RepositoryName = repository;
		aclIdentity.Domain = domain;
		aclIdentity.Name = name;
		List<AclIdentity> aclIdentityList = new List<AclIdentity>();
		aclIdentityList.Add(aclIdentity);
		accessControlService.Delete(aclIdentityList);
	}

	public AclPackage GetOwnerPrivateAcls(String ownerName, String repository)
    {
		// instantiate a Query service proxy
		ServiceFactory serviceFactory = ServiceFactory.Instance;
		IQueryService queryService = null;
		
	    queryService = serviceFactory.GetRemoteService<IQueryService>(accessControlService.GetServiceContext());
		
		// build and run the query
		PassthroughQuery query = new PassthroughQuery();
		query.QueryString = "select owner_name, object_name from dm_acl " +
			"where r_is_internal='0' and acl_class='0' and owner_name='" + ownerName + "'";
		query.AddRepository(DefaultRepository);
		QueryExecution queryEx = new QueryExecution();
		queryEx.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;
		queryEx.MaxResultCount = -1; // no limit
		OperationOptions operationOptions = null;
		QueryResult queryResult = queryService.Execute(query, queryEx,
		        operationOptions);
		DataPackage resultDp = queryResult.DataPackage;
		List<DataObject> dataObjects = resultDp.DataObjects;
		Console.WriteLine("Total objects returned is: " + dataObjects.Count);
		
        //convert the results into a List<AclIdentity>
		List<AclIdentity> identityList = new List<AclIdentity>();
		foreach (DataObject dObj in dataObjects)
		{
			PropertySet docProperties = dObj.Properties;
			AclIdentity aclIdentity = new AclIdentity();
			aclIdentity.Domain = docProperties.Get("owner_name").GetValueAsString();
			aclIdentity.Name = docProperties.Get("object_name").GetValueAsString();
			aclIdentity.RepositoryName = repository;
			identityList.Add(aclIdentity);
		}
		
		// get and return the AclPackage
		return accessControlService.Get(identityList);
	}
    }
}

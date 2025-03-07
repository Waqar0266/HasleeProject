﻿using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static System.String;

namespace Hasslefree.Services.Security
{
	public class SecurityService : ISecurityService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IUserContext UserContext { get; }

		#endregion

		#region Constructor

		public SecurityService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			ICacheManager cacheManager,
			IUserContext userContext
		)
		{
			// Repos
			LoginRepo = loginRepo;

			// Other
			CacheManager = cacheManager;
			UserContext = userContext;
		}

		#endregion

		#region ISecurityService

		public virtual bool IsSystemAdmin(int loginId)
		{
			if (loginId <= 0) return false;

			var groups = GetGroups(loginId);
			var permissions = GetPermissions(loginId);

			return groups.FirstOrDefault(i => i.SecurityGroupLogins.Any(y => y.LoginId == loginId)) != null
					  || permissions.Any(p => p.PermissionUniqueName.Contains("SysAdmin"));
		}

		/// <summary>
		/// Determine if the person is in one or more of the specified security groups
		/// </summary>
		/// <param name="loginId"></param>
		/// <param name="securityGroups"></param>
		/// <returns></returns>
		public virtual bool IsInSecurityGroup(int loginId, List<string> securityGroups)
		{
			if (loginId <= 0 || (!securityGroups.Any())) return false;

			var groups = GetGroups(loginId);

			if (groups.Any(a => a.SecurityGroupName == "Admin")) return true;

			return groups.Any(sg => securityGroups.Contains(sg.SecurityGroupName));
		}

		public virtual bool HasPermission(int loginId, string permission)
		{
			if (IsNullOrWhiteSpace(permission)) throw new ArgumentException("Permission");

			if (UserContext.IsSystemAdmin) return true;

			return permission.Contains("Any") ? UserContext.SecurityGroups.Any() : UserContext.Permissions.Any(p => p.PermissionUniqueName.Contains(permission));
		}

		#endregion

		#region Private Methods

		private List<SecurityGroup> GetGroups(int loginId)
		{
			return LoginRepo.Table
					   .Include(l => l.SecurityGroupLogins.Select(sgl => sgl.SecurityGroup.Permissions))
					   .FirstOrDefault(p => p.LoginId == loginId)
					   ?.SecurityGroupLogins
					   ?.Select(r => r.SecurityGroup)
					   .ToList() ?? new List<SecurityGroup>();
		}

		private List<Permission> GetPermissions(int loginId)
		{
			return GetGroups(loginId).SelectMany(sg => sg.Permissions).Distinct().ToList();
		}

		#endregion
	}
}

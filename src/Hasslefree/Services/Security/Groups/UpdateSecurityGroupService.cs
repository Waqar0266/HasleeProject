using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Helpers;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Helpers;
using Hasslefree.Services.Security.Validation;
using Hasslefree.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Security.Groups
{
	public class UpdateSecurityGroupService : IUpdateSecurityGroupService
	{
		#region Constants

		private readonly string[] _restrictedParameters = { "SecurityGroupId", "CreatedOnUtc", "IsSystemSecurityGroup" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Permission> PermissionRepo { get; }
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private SecurityGroup _group;

		private bool _attemptedSystemGroupEdit;

		private readonly Dictionary<CrudAction, HashSet<int>> _permissions = new Dictionary<CrudAction, HashSet<int>>
		{
			{ CrudAction.Create, new HashSet<int>()},
			{ CrudAction.Remove, new HashSet<int>()}
		};

		private readonly Dictionary<CrudAction, HashSet<int>> _users = new Dictionary<CrudAction, HashSet<int>>
		{
			{ CrudAction.Create, new HashSet<int>()},
			{ CrudAction.Remove, new HashSet<int>()}
		};

		#endregion

		#region Constructor

		public UpdateSecurityGroupService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Permission> permissionRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			ICacheManager cacheManager,
			IDataContext database,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo)
		{
			// Repos
			LoginRepo = loginRepo;
			PermissionRepo = permissionRepo;
			SecurityGroupRepo = securityGroupRepo;

			// Other
			CacheManager = cacheManager;
			Database = database;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IEditSecurityGroupService

		public bool HasWarnings
		{
			get
			{
				if (_group == null)
					return Warnings.Any();

				Warnings.Clear();
				return !(ValidateEntity() && ValidateDatabase());
			}
		}

		public List<SecurityGroupWarning> Warnings { get; } = new List<SecurityGroupWarning>();

		public IUpdateSecurityGroupService this[int securityGroupId]
		{
			get
			{
				if (securityGroupId <= 0) return this;

				_group = SecurityGroupQuery(securityGroupId);

				return this;
			}
		}

		public IUpdateSecurityGroupService WithSecurityGroupId(int securityGroupId) => this[securityGroupId];

		public IUpdateSecurityGroupService Set<T>(Expression<Func<SecurityGroup, T>> lambda, object value)
		{
			if (_group == null) return this;

			if (_group.IsSystemSecurityGroup)
			{
				_attemptedSystemGroupEdit = true;
				return this;
			}

			_group.SetPropertyValue(lambda, value, _restrictedParameters);

			return this;
		}

		public IUpdateSecurityGroupService WithPermission(int permissionId)
		{
			if (_group == null) return this;

			if (permissionId <= 0) return this;

			if (_permissions[CrudAction.Create].Contains(permissionId)) return this;

			_permissions[CrudAction.Create].Add(permissionId);

			return this;
		}

		public IUpdateSecurityGroupService RemovePermission(int permissionId)
		{
			if (_group?.Permissions == null) return this;

			if (permissionId <= 0) return this;

			if (_permissions[CrudAction.Remove].Contains(permissionId)) return this;

			_permissions[CrudAction.Remove].Add(permissionId);

			return this;
		}

		public IUpdateSecurityGroupService WithUser(int loginId)
		{
			if (_group == null) return this;

			if (loginId <= 0) return this;

			if (_users[CrudAction.Create].Contains(loginId)) return this;

			_users[CrudAction.Create].Add(loginId);

			return this;
		}

		public IUpdateSecurityGroupService RemoveUser(int loginId)
		{
			if (_group?.SecurityGroupLogins == null) return this;

			if (loginId <= 0) return this;

			if (_users[CrudAction.Remove].Contains(loginId)) return this;

			_users[CrudAction.Remove].Add(loginId);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings) return Clear(false);

			UpdatePermissions();
			UpdateUsers();

			_group.ModifiedOn = DateTime.Now;

			if (!saveChanges) return Clear(true);

			Database.SaveChanges();

			return Clear(true);
		}

		#endregion

		#region Private Methods

		private SecurityGroup SecurityGroupQuery(int groupId)
		{
			var sgFuture = (from sg in SecurityGroupRepo.Table.Include(sg => sg.Permissions) where sg.SecurityGroupId == groupId select sg).DeferredFirstOrDefault().FutureValue();

			// ReSharper disable UnusedVariable
			var sglFuture = (from sg in SecurityGroupRepo.Table
							 where sg.SecurityGroupId == groupId
							 join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
							 select sgl).Future();

			var lFuture = (from sg in SecurityGroupRepo.Table
						   where sg.SecurityGroupId == groupId
						   join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
						   join l in LoginRepo.Table on sgl.LoginId equals l.LoginId
						   select l).Future();
			// ReSharper enable UnusedVariable

			var group = sgFuture.Value;

			if (group == null) return null;

			group.SecurityGroupLogins = group.SecurityGroupLogins ?? sglFuture.ToList();

			return group;
		}

		#region Validation

		private bool ValidateEntity()
		{
			var results = new SecurityGroupValidator(true).Validate(_group);

			if (results.IsValid) return true;

			Warnings.AddRange(results.Errors.Select(error => new SecurityGroupWarning
			(
				error.ErrorCode.EnumTryParse<SecurityGroupWarningCode>(out var @enum) ? @enum : SecurityGroupWarningCode.PropertyNotValid,
				$"[{error.PropertyName}]: {error.ErrorMessage}"
			)));

			return false;
		}

		private bool ValidateDatabase()
		{
			if (_attemptedSystemGroupEdit)
				Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.CannotEditSystemSecurityGroup));

			if (SecurityGroupRepo.Table.Any(sg => sg.SecurityGroupId != _group.SecurityGroupId && sg.SecurityGroupName.Equals(_group.SecurityGroupName, StringComparison.CurrentCulture)))
				Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.DuplicateGroupName, _group.SecurityGroupName));

			if (_permissions[CrudAction.Create].Any())
			{
				var permissionsNotFound = _permissions[CrudAction.Create].Except(PermissionRepo.Table.Select(p => p.PermissionId).AsEnumerable()).ToHashSet();

				if (permissionsNotFound.Any())
					foreach (var id in permissionsNotFound)
						Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.PermissionNotFound, id.ToString()));
			}

			if (!_users[CrudAction.Create].Any()) return !Warnings.Any();

			var usersNotFound = _users[CrudAction.Create].Except(LoginRepo.Table.Select(l => l.LoginId).AsEnumerable()).ToHashSet();

			if (!usersNotFound.Any()) return !Warnings.Any();

			foreach (var id in usersNotFound)
				Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.UserNotFound, id.ToString()));

			return !Warnings.Any();
		}

		#endregion

		#region Permissions

		private void UpdatePermissions()
		{
			RemovePermissions();
			AddPermissions();
		}

		private void RemovePermissions()
		{
			if (!_permissions[CrudAction.Remove].Any()) return;

			var removePermissions = _group.Permissions.Where(p => _permissions[CrudAction.Remove].Contains(p.PermissionId)).ToList();

			removePermissions.ForEach(p => _group.Permissions.Remove(p));
		}

		private void AddPermissions()
		{
			if (!_permissions[CrudAction.Create].Any()) return;

			var currentPermissionIds = _group.Permissions.Select(p => p.PermissionId);
			var ids = _permissions[CrudAction.Create].AsEnumerable();

			var permissions = PermissionRepo.Table.Where(p => !currentPermissionIds.Contains(p.PermissionId) && ids.Contains(p.PermissionId)).ToList();

			permissions.ForEach(p => _group.Permissions.Add(p));
		}

		#endregion

		#region Users

		private void UpdateUsers()
		{
			RemoveUsers();
			AddUsers();
		}

		private void RemoveUsers()
		{
			if (!_users[CrudAction.Remove].Any()) return;

			var ids = _users[CrudAction.Remove].ToList();
			var securityGroupLogins = _group.SecurityGroupLogins.Where(sgl => ids.Contains(sgl.LoginId)).ToList();

			securityGroupLogins.ForEach(sgl => _group.SecurityGroupLogins.Remove(sgl));
		}

		private void AddUsers()
		{
			if (!_users[CrudAction.Create].Any()) return;

			var currentUsers = _group.SecurityGroupLogins.Select(sgl => sgl.LoginId);
			var ids = _users[CrudAction.Create].AsEnumerable();

			var users = LoginRepo.Table.Where(l => !currentUsers.Contains(l.LoginId) && ids.Contains(l.LoginId)).ToList();

			users.ForEach(l => _group.SecurityGroupLogins.Add(new SecurityGroupLogin
			{
				LoginId = l.LoginId
			}));
		}


		#endregion

		private bool Clear(bool success)
		{
			if (success) ClearCache();

			_group = null;
			_attemptedSystemGroupEdit = false;

			_permissions[CrudAction.Create].Clear();
			_permissions[CrudAction.Remove].Clear();

			_users[CrudAction.Create].Clear();
			_users[CrudAction.Remove].Clear();

			return success;
		}

		private void ClearCache()
		{
			var logins = _users[CrudAction.Create].Union(_users[CrudAction.Remove]).ToList();

			if (!logins.Any()) return;

			logins.ForEach(m =>
			{
				CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.IsSystemAdmin(0, m));
				CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.Permissions(0, m));
				CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.SecurityGroups(0, m));
			});
		}

		#endregion
	}
}
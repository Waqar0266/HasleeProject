using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Common;
using Hasslefree.Web.Models.Security.SecurityGroups;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class UpdateAuthorizedUserService : IUpdateAuthorizedUserService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Core.Domain.Security.Login _login;

		// Security Groups
		private List<int> _addToSecurityGroups;
		private List<int> _removeFromSecurityGroups;

		#endregion

		#region Constructor

		public UpdateAuthorizedUserService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			ICacheManager cacheManager,
			IDataContext database
		)
		{
			LoginRepo = loginRepo;
			SecurityGroupRepo = securityGroupRepo;
			CacheManager = cacheManager;
			Database = database;
		}

		#endregion

		#region IEditAuthorizedUserService

		public bool HasWarnings => Warnings.Any();

		public List<AuthorizedUserWarning> Warnings { get; } = new List<AuthorizedUserWarning>();

		public IUpdateAuthorizedUserService WithId(int loginId)
		{
			if (loginId <= 0) return this;

			_login = LoginRepo.Table
								.Include(l => l.SecurityGroupLogins)
								.FirstOrDefault(l => l.LoginId == loginId);

			return this;
		}

		public IUpdateAuthorizedUserService WithSecurityGroups(List<SecurityGroupBaseModel> securityGroups)
		{
			if (securityGroups == null || securityGroups.Count <= 0) return this;

			_addToSecurityGroups = new List<int>();
			_removeFromSecurityGroups = new List<int>();

			securityGroups.ForEach(sg =>
			{
				switch (sg.Action)
				{
					case CrudAction.Create:
						_addToSecurityGroups.Add(sg.SecurityGroupId);
						break;
					case CrudAction.Remove:
						_removeFromSecurityGroups.Add(sg.SecurityGroupId);
						break;
				}
			});

			return this;
		}

		public bool Edit()
		{
			if (!EditValid()) return false;

			if (_addToSecurityGroups != null && _addToSecurityGroups.Count > 0) AddToSecurityGroups();
			if (_removeFromSecurityGroups != null && _removeFromSecurityGroups.Count > 0) RemoveFromSecurityGroups();

			Database.SaveChanges();
			ClearCache();

			return true;
		}

		#endregion

		#region Private Methods

		private bool EditValid()
		{
			if (_login == null) Warnings.Add(new AuthorizedUserWarning(AuthorizedUserWarningCode.UserNotFound));

			return !HasWarnings;
		}

		private void AddToSecurityGroups()
		{
			var addToSecurityGroups = _addToSecurityGroups.Except(_login.SecurityGroupLogins.Select(sgl => sgl.SecurityGroupId));
			var securityGroups = SecurityGroupRepo.Table.Where(sg => addToSecurityGroups.Contains(sg.SecurityGroupId)).ToList();
			securityGroups.ForEach(sg =>
			{
				_login.SecurityGroupLogins.Add(new SecurityGroupLogin
				{
					SecurityGroupId = sg.SecurityGroupId
				});
			});
		}

		private void RemoveFromSecurityGroups()
		{
			var securityGroupLogins = _login.SecurityGroupLogins.Where(sgl => _removeFromSecurityGroups.Contains(sgl.SecurityGroupId)).ToList();
			securityGroupLogins.ForEach(sgl => _login.SecurityGroupLogins.Remove(sgl));
		}

		private void ClearCache()
		{
			CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.IsSystemAdmin(0, _login.LoginId));
			CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.Permissions(0, _login.LoginId));
			CacheManager.RemoveByPattern(CacheKeys.Store.Account.Login.SecurityGroups(0, _login.LoginId));
		}

		#endregion
	}
}

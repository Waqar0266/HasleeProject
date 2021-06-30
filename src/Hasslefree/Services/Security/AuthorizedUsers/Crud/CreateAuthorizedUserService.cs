using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.SecurityGroups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class CreateAuthorizedUserService : ICreateAuthorizedUserService
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
		private List<SecurityGroup> _securityGroups;

		#endregion

		#region Constructor

		public CreateAuthorizedUserService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			ICacheManager cacheManager,
			IDataContext database
		)
		{
			// Repos
			LoginRepo = loginRepo;
			SecurityGroupRepo = securityGroupRepo;

			// Other
			CacheManager = cacheManager;
			Database = database;
		}

		#endregion

		#region ICreateAuthorizedUserService

		public bool HasWarnings => Warnings.Any();

		public List<AuthorizedUserWarning> Warnings { get; } = new List<AuthorizedUserWarning>();

		public ICreateAuthorizedUserService WithModel(AuthorizedUserFullModel baseModel)
		{
			if (baseModel == null) return this;

			_login = LoginRepo.Table.FirstOrDefault(l => l.Email == baseModel.Email);

			if (_login == null) return this;

			if (!(baseModel.SecurityGroups?.Any()?? false)) return this;

			var securityGroupIds = baseModel.SecurityGroups.Select(sg => sg.SecurityGroupId);

			_securityGroups = SecurityGroupRepo.Table.Where(sg => securityGroupIds.Contains(sg.SecurityGroupId)).ToList();

			if (_securityGroups == null || _securityGroups.Count <= 0) return this;

			_securityGroups.ForEach(sg =>
			{
				_login.SecurityGroupLogins.Add(new SecurityGroupLogin
				{
					SecurityGroup = sg
				});
			});

			return this;
		}

		public ICreateAuthorizedUserService WithModel(string email, List<SecurityGroupBaseModel> securityGroups)
		{
			if (String.IsNullOrWhiteSpace(email)) return this;

			_login = LoginRepo.Table.FirstOrDefault(l => l.Email == email);

			if (_login == null) return this;

			if (securityGroups == null || securityGroups.Count <= 0) return this;

			var securityGroupIds = securityGroups.Select(sg => sg.SecurityGroupId);

			_securityGroups = SecurityGroupRepo.Table.Where(sg => securityGroupIds.Contains(sg.SecurityGroupId)).ToList();

			if (_securityGroups == null || _securityGroups.Count <= 0) return this;

			_securityGroups.ForEach(sg =>
			{
				_login.SecurityGroupLogins.Add(new SecurityGroupLogin
				{
					SecurityGroup = sg
				});
			});

			return this;
		}

		public bool Create()
		{
			if (!CreateValid()) return false;

			Database.SaveChanges();
			ClearCache();

			return true;
		}

		#endregion

		#region Private Methods

		private bool CreateValid()
		{
			if (_login == null)
				Warnings.Add(new AuthorizedUserWarning(AuthorizedUserWarningCode.UserNotFound));

			if (_securityGroups == null || _securityGroups.Count <= 0)
				Warnings.Add(new AuthorizedUserWarning(AuthorizedUserWarningCode.SecurityGroupsNotFound));

			return !HasWarnings;
		}

		private void ClearCache()
		{
		}

		#endregion
	}
}

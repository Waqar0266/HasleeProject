using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Cache;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class DeleteAuthorizedUserService : IDeleteAuthorizedUserService
	{
		#region Private Properties

		// Repos
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private List<SecurityGroupLogin> _securityGroupLogins;

		#endregion

		#region Constructor

		public DeleteAuthorizedUserService
		(
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo,
			ICacheManager cacheManager,
			IDataContext database
		)
		{
			// Repos
			SecurityGroupLoginRepo = securityGroupLoginRepo;

			// Other
			CacheManager = cacheManager;
			Database = database;
		}

		#endregion

		#region IDeleteAuthorizedUserService

		public bool HasWarnings => Warnings.Any();

		public List<AuthorizedUserWarning> Warnings { get; } = new List<AuthorizedUserWarning>();

		public IDeleteAuthorizedUserService WithId(int loginId)
		{
			if (loginId <= 0) return this;

			_securityGroupLogins = SecurityGroupLoginRepo.Table
															.Where(sgl => sgl.LoginId == loginId)
															.ToList();

			return this;
		}

		public IDeleteAuthorizedUserService WithIds(List<int> loginIds)
		{
			if (loginIds == null || loginIds.Count <= 0) return this;

			_securityGroupLogins = SecurityGroupLoginRepo.Table
															.Where(l => loginIds.Contains(l.LoginId))
															.ToList();

			return this;
		}

		/// <inheritdoc />
		public bool Delete(bool saveChanges = true)
		{
			if (!DeleteValid()) return false;

			SecurityGroupLoginRepo.Remove(_securityGroupLogins);

			if (!saveChanges) return true;

			Database.SaveChanges();
			_securityGroupLogins.Select(sgl => sgl.LoginId).ToList().ForEach(ClearCache);

			return true;
		}

		#endregion

		#region Private Methods

		private bool DeleteValid()
		{

			if (_securityGroupLogins == null || _securityGroupLogins.Count <= 0)
				Warnings.Add(new AuthorizedUserWarning(AuthorizedUserWarningCode.UsersNotFound));

			return !HasWarnings;
		}

		private void ClearCache(int loginId)
		{
		}

		#endregion
	}
}

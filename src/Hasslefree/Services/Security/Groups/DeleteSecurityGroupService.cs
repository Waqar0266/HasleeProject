using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Cache;

namespace Hasslefree.Services.Security.Groups
{
	public class DeleteSecurityGroupService : IDeleteSecurityGroupService
	{
		#region Private Properties

		// Repos
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private List<SecurityGroup> _securityGroups;

		#endregion

		#region Constructor

		public DeleteSecurityGroupService
		(
			IDataRepository<SecurityGroup> securityGroupRepo,
			ICacheManager cacheManager,
			IDataContext database
		)
		{
			// Repos
			SecurityGroupRepo = securityGroupRepo;

			// Other
			CacheManager = cacheManager;
			Database = database;
		}

		#endregion

		#region IDeleteSecurityGroupService

		public bool HasWarnings => Warnings.Any();

		public List<SecurityGroupWarning> Warnings { get; } = new List<SecurityGroupWarning>();

		public IDeleteSecurityGroupService this[int securityGroupId]
		{
			get
			{
				if (securityGroupId <= 0) return this;

				var securityGroup = SecurityGroupRepo.Table
					.Include(sg => sg.SecurityGroupLogins)
					.FirstOrDefault(sg => sg.SecurityGroupId == securityGroupId);

				if (securityGroup == null) return this;

				_securityGroups = _securityGroups ?? new List<SecurityGroup>();

				_securityGroups.Add(securityGroup);

				return this;
			}
		}

		public IDeleteSecurityGroupService this[List<int> securityGroupIds]
		{
			get
			{
				if (!(_securityGroups?.Any() ?? false)) return this;

				_securityGroups = SecurityGroupRepo.Table
					.Include(sg => sg.SecurityGroupLogins)
					.Where(sg => securityGroupIds.Contains(sg.SecurityGroupId))
					.ToList();

				return this;
			}
		}

		/// <inheritdoc />
		public bool Remove(bool saveChanges = true)
		{
			if (!RemoveValid()) return false;

			SecurityGroupRepo.Remove(_securityGroups);

			if (!saveChanges) return true;

			Database.SaveChanges();
			_securityGroups.ForEach(ClearCache);

			return true;
		}

		#endregion

		#region Private Methods

		private bool RemoveValid()
		{
			if (!(_securityGroups?.Any() ?? false))
				Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.GroupsNotFound));

			if (_securityGroups?.Any(sg => sg.IsSystemSecurityGroup) ?? false)
				Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.CannotDeleteSystemSecurityGroup));

			return !HasWarnings;
		}

		private void ClearCache(SecurityGroup securityGroup)
		{
		}

		#endregion
	}
}

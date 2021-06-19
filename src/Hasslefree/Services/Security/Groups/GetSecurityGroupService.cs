using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Web.Models.Security.SecurityGroups.Get;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Security.Groups
{
	public class GetSecurityGroupService : IGetSecurityGroupService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Constructor

		public GetSecurityGroupService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			// Repos
			LoginRepo = loginRepo;
			SecurityGroupRepo = securityGroupRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
			PersonRepo = personRepo;
		}

		#endregion

		#region ISecurityGroupsService

		public SecurityGroupWarning Warning { get; private set; }

		public SecurityGroupGet this[int id, bool includeDates = true]
		{
			get
			{
				if (id <= 0) return GroupNotFound();

				var securityGroup = GroupQuery(id);

				if (securityGroup == null) return GroupNotFound();

				return new SecurityGroupGet
				{
					SecurityGroupId = securityGroup.SecurityGroupId,
					CreatedOn = includeDates ? securityGroup.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? securityGroup.ModifiedOn : (DateTime?)null,
					Name = securityGroup.SecurityGroupName,
					Description = securityGroup.SecurityGroupDesc,
					IsSystemSecurityGroup = securityGroup.IsSystemSecurityGroup,
					Users = securityGroup.SecurityGroupLogins?.Select(sgl => new SecurityGroupUser
					{
						LoginId = sgl.LoginId,
						CreatedOn = includeDates ? sgl.Login?.CreatedOn : (DateTime?)null,
						Email = sgl.Login?.Email,
						FullName = !string.IsNullOrWhiteSpace(sgl.Login?.Salutation) ? sgl.Login?.Salutation : sgl.Login?.Person?.FirstName + " " + sgl.Login?.Person?.Surname,
					}).ToList(),
					Permissions = securityGroup.Permissions?.Select(p => new SecurityGroupPermission
					{
						PermissionId = p.PermissionId,
						DisplayName = p.PermissionDisplayName,
						Description = p.PermissionDescription,
						Group = p.PermissionGroupName
					}).ToList()
				};
			}
		}

		#endregion

		#region Private Methods

		private dynamic GroupNotFound()
		{
			Warning = new SecurityGroupWarning(SecurityGroupWarningCode.GroupNotFound);
			return null;
		}

		private SecurityGroup GroupQuery(int id)
		{
			var sgFuture = (from sg in SecurityGroupRepo.Table.Include(sg => sg.Permissions) where sg.SecurityGroupId == id select sg).DeferredFirstOrDefault().FutureValue();

			// ReSharper disable UnusedVariable
			var sglFuture = (from sg in SecurityGroupRepo.Table
							 where sg.SecurityGroupId == id
							 join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
							 select sgl).Future();

			var lFuture = (from sg in SecurityGroupRepo.Table
						   where sg.SecurityGroupId == id
						   join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
						   join l in LoginRepo.Table on sgl.LoginId equals l.LoginId
						   select l).Future();

			var pFuture = (from sg in SecurityGroupRepo.Table
						   where sg.SecurityGroupId == id
						   join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
						   join l in LoginRepo.Table on sgl.LoginId equals l.LoginId
						   join p in PersonRepo.Table on l.PersonId equals p.PersonId
						   select p).Future();
			// ReSharper enable UnusedVariable

			var group = sgFuture.Value;

			if (group == null) return null;
			
			group.SecurityGroupLogins = group.SecurityGroupLogins ?? sglFuture.ToList();

			return group;
		}

		#endregion
	}
}

using Hasslefree.Data;
using Hasslefree.Web.Models.Common;
using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.SecurityGroups;
using System;
using System.Data.Entity;
using System.Linq;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class GetAuthorizedUsersService : IGetAuthorizedUsersService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		#endregion

		#region Fields

		private int _authorizedUserLoginId;

		#endregion

		#region Constructor

		public GetAuthorizedUsersService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo
		)
		{
			LoginRepo = loginRepo;
		}

		#endregion

		#region IGetAuthorizedUsersService

		public IGetAuthorizedUsersService WithId(int authorizedUserId)
		{
			if (authorizedUserId <= 0) return this;

			_authorizedUserLoginId = authorizedUserId;

			return this;
		}

		public AuthorizedUserFullModel Get()
		{
			if (_authorizedUserLoginId <= 0) return null;

			var authorizedUser = LoginRepo.Table
										.Include(l => l.Person)
										.Include(l => l.SecurityGroupLogins.Select(sgl => sgl.SecurityGroup))
										.FirstOrDefault(l => l.LoginId == _authorizedUserLoginId);

			if (authorizedUser == null) return null;

			return new AuthorizedUserFullModel
			{
				LoginId = authorizedUser.LoginId,
				Email = authorizedUser.Email,
				FullName = !String.IsNullOrWhiteSpace(authorizedUser.Salutation)
					? authorizedUser.Salutation
					: authorizedUser.Person.FirstName + " " + authorizedUser.Person.Surname,
				SecurityGroups = authorizedUser.SecurityGroupLogins.Select(sgl => new SecurityGroupBaseModel
				{
					Action = CrudAction.None,
					SecurityGroupId = sgl.SecurityGroupId,
					SecurityGroupName = sgl.SecurityGroup.SecurityGroupName,
					SecurityGroupDesc = sgl.SecurityGroup.SecurityGroupDesc,
					IsSystemSecurityGroup = sgl.SecurityGroup.IsSystemSecurityGroup
				}).ToList()
			};
		}
		
		#endregion
	}
}

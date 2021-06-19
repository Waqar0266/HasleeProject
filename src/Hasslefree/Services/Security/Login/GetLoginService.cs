using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Web.Models.Security.Login.Get;
using Hasslefree.Web.Models.Security.Login.Get.Person;
using Hasslefree.Web.Models.Security.Login.Get.SecurityGroups;
using System;
using System.Linq;
using Z.EntityFramework.Plus;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Login
{
	public class GetLoginService : IGetLoginService
	{
		#region Private Properties

		private IDataRepository<LoginDb> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Constructor

		public GetLoginService
		(
			IDataRepository<LoginDb> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			// Repos
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SecurityGroupRepo = securityGroupRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IGetLoginDetailsService

		public LoginWarning Warning { get; private set; }

		public LoginGet this[int loginId, bool includeDates = true]
		{
			get
			{
				if (loginId <= 0) return LoginNotFound();

				var login = LoginQuery(loginId);

				if (login == null) return LoginNotFound();

				return new LoginGet
				{
					LoginId = login.LoginId,
					CreatedOn = includeDates ? login.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? login.ModifiedOn : (DateTime?)null,
					Email = login.Email,
					Salutation = login.Salutation,
					Active = login.Active,
					Person = new PersonGet
					{
						PersonId = login.PersonId,
						FirstName = login.Person?.FirstName,
						Surname = login.Person?.Surname,
						PersonStatus = login.Person?.PersonStatus ?? PersonStatus.Pending
					},
					SecurityGroups = login.SecurityGroupLogins?.Select(sgl => sgl.SecurityGroup).Select(sg => new SecurityGroupGet
					{
						SecurityGroupId = sg.SecurityGroupId,
						Name = sg.SecurityGroupName,
						Description = sg.SecurityGroupDesc,
						IsSystemGroup = sg.IsSystemSecurityGroup
					}).ToList()
				};
			}
		}

		#endregion

		#region Private Methods

		private LoginDb LoginQuery(int loginId)
		{
			var lFuture = LoginRepo.Table.DeferredFirstOrDefault(l => l.LoginId == loginId).FutureValue();

			var pFuture = (from l in LoginRepo.Table
						   where l.LoginId == loginId
						   join p in PersonRepo.Table on l.PersonId equals p.PersonId
						   select p).FutureValue();

			var sglFuture = (from sgl in SecurityGroupLoginRepo.Table
							 where sgl.LoginId == loginId
							 select sgl).Future();

			var sgFuture = (from sgl in SecurityGroupLoginRepo.Table
							where sgl.LoginId == loginId
							join sg in SecurityGroupRepo.Table on sgl.SecurityGroupId equals sg.SecurityGroupId
							select sg).Future();

			var login = lFuture.Value;

			if (login == null) return null;

			login.Person = login.Person ?? pFuture.Value;
			login.SecurityGroupLogins = login.SecurityGroupLogins ?? sglFuture.Select(sgl =>
										{
											sgl.SecurityGroup = sgl.SecurityGroup ?? sgFuture.FirstOrDefault(sg => sgl.SecurityGroupId == sg.SecurityGroupId);
											return sgl;
										}).ToList();

			return login;
		}

		private dynamic LoginNotFound()
		{
			Warning = new LoginWarning(LoginWarningCode.LoginNotFound);
			return null;
		}

		#endregion
	}
}
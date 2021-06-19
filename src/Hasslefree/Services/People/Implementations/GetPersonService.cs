using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.People.Warnings;
using Hasslefree.Web.Models.People.Get;
using Hasslefree.Web.Models.People.Get.Account;
using Hasslefree.Web.Models.People.Get.Login;
using System;
using System.Linq;
using Z.EntityFramework.Plus;
using DbLogin = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.People.Implementations
{
	public class GetPersonService : IGetPersonService
	{
		#region Private Properties

		private IDataRepository<DbLogin> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Constructor

		public GetPersonService
		(
			IDataRepository<DbLogin> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IGetPeopleService

		public PersonWarning Warning { get; private set; }

		public PersonGet this[int personId, bool includeDates]
		{
			get
			{
				if (personId <= 0) return NotFound();

				var person = PersonQuery(personId);

				if (person == null) return NotFound();

				return Get(person, includeDates);
			}
		}

		public PersonGet this[string email, bool includeDates]
		{
			get
			{
				var person = PersonQuery(0, email);

				if (person == null) return NotFound();

				return Get(person,includeDates);
			}
		}

		#endregion

		#region Private Methods

		private PersonGet Get(Person person,bool includeDates)
		{
			var login = person.Logins.FirstOrDefault();
			return new PersonGet
			{
				PersonId = person.PersonId,
				CreatedOn = includeDates ? person.CreatedOn : (DateTime?)null,
				ModifiedOn = includeDates ? person.ModifiedOn : (DateTime?)null,
				PersonGuid = person.PersonGuid,
				Title = person.Title,
				FirstName = person.FirstName,
				Surname = person.Surname,
				Birthday = person.Birthday,
				Phone = person.Phone,
				Fax = person.Fax,
				Mobile = person.Mobile,
				Email = person.Email,
				Gender = person.Gender,
				PersonStatus = person.PersonStatus,
				HasAdminPrivileges = person.Logins.Any(l => l.SecurityGroupLogins.Any()),
				Login = login != null ? new LoginGet
				{
					LoginId = login.LoginId,
					Email = login.Email,
					Salutation = login.Salutation,
					Active = login.Active
				} : null
			};
		}

		private PersonGet NotFound()
		{
			Warning = new PersonWarning(PersonWarningCode.PersonNotFound);
			return null;
		}

		private Person PersonQuery(int personId,string email = "")
		{
			var personType = EntityType.Person.ToString();

			var pFuture = String.IsNullOrWhiteSpace(email)
				? PersonRepo.Table.DeferredFirstOrDefault(p => p.PersonId == personId).FutureValue()
				: PersonRepo.Table.DeferredFirstOrDefault(p => p.Email == email).FutureValue();

			var lFuture = (from l in LoginRepo.Table
						   where l.PersonId == personId
						   select l).Future();

			var sglFuture = (from l in LoginRepo.Table
							 where l.PersonId == personId
							 join sgl in SecurityGroupLoginRepo.Table on l.LoginId equals sgl.LoginId
							 select sgl).Future();

			return pFuture.Value;
		}

		#endregion
	}
}

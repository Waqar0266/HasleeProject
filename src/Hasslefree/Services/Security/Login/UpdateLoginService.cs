using Hasslefree.Core.Crypto;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Helpers;
using Hasslefree.Data;
using Hasslefree.Services.Helpers;
using Hasslefree.Services.Security.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;
using static System.String;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Login
{
	public class UpdateLoginService : IUpdateLoginService
	{
		#region Private Properties

		// Repos
		private IDataRepository<LoginDb> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private LoginDb _login;

		#endregion

		#region Constructor

		public UpdateLoginService
		(
			IDataRepository<LoginDb> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo,
			IDataContext database
		)
		{
			// Repos
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;

			// Other
			Database = database;
		}

		#endregion

		#region IUpdateLoginService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !(ValidateEntity() && ValidateDatabase());
			}
		}

		public List<LoginWarning> Warnings { get; } = new List<LoginWarning>();

		public IUpdateLoginService this[int loginId]
		{
			get
			{
				if (loginId <= 0) return this;

				_login = LoginQuery(loginId);

				return this;
			}
		}

		public IUpdateLoginService WithLoginId(int loginId) => this[loginId];

		public IUpdateLoginService WithPersonId(int personId)
		{
			if (personId <= 0) return this;

			_login = LoginRepo.Table.FirstOrDefault(l => l.PersonId == personId);

			return this;
		}

		public IUpdateLoginService WithLogin(LoginDb login)
		{
			_login = login;
			return this;
		}

		public IUpdateLoginService WithEmail(string email)
		{
			if (IsNullOrWhiteSpace(email)) return this;

			_login = LoginRepo.Table.FirstOrDefault(l => l.Email == email);

			return this;
		}

		public IUpdateLoginService SetEmail(string email, bool updatePerson = false)
		{
			if (_login == null) return this;

			_login.Email = email;

			if (updatePerson && _login.Person != null) _login.Person.Email = email;

			return this;
		}

		public IUpdateLoginService Set<T>(Expression<Func<LoginDb, T>> lambda, object value)
		{
			_login?.SetPropertyValue(lambda, value);
			return this;
		}

		public IUpdateLoginService WithSecurityGroup(int securityGroupId)
		{
			if (_login == null) return this;

			_login.SecurityGroupLogins = _login.SecurityGroupLogins ?? new List<SecurityGroupLogin>();

			_login.SecurityGroupLogins.Add(new SecurityGroupLogin
			{
				SecurityGroupId = securityGroupId
			});

			return this;
		}

		public IUpdateLoginService RemoveSecurityGroup(int securityGroupId)
		{
			if (_login?.SecurityGroupLogins == null) return this;

			var sgl = _login.SecurityGroupLogins.FirstOrDefault(g => g.SecurityGroupId == securityGroupId);

			if (sgl == null) return this;

			_login.SecurityGroupLogins.Remove(sgl);

			return this;
		}


		public IUpdateLoginService SetPassword(string password, string salt = null)
		{
			if (_login == null) return this;

			if (IsNullOrWhiteSpace(salt)) salt = Hash.GetSalt();
			if (IsNullOrWhiteSpace(password)) password = BaseX.GenerateString();

			_login.Password = Hash.GetHashBase64(password, salt); ;
			_login.PasswordSalt = salt;

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings) return false;

			_login.ModifiedOn = DateTime.Now;

			LoginRepo.Attach(_login);

			if (saveChanges) Database.SaveChanges();

			return true;
		}

		#endregion

		#region Private Methods

		private LoginDb LoginQuery(int loginId)
		{
			var lFuture = (from l in LoginRepo.Table
						   where l.LoginId == loginId
						   select l).DeferredFirstOrDefault().FutureValue();

			var pFuture = (from l in LoginRepo.Table
						   where l.LoginId == loginId
						   join p in PersonRepo.Table on l.PersonId equals p.PersonId
						   select p).DeferredFirstOrDefault().FutureValue();

			var sglFuture = (from sgl in SecurityGroupLoginRepo.Table
							 where sgl.LoginId == loginId
							 select sgl).Future();

			var login = lFuture.Value;

			if (login == null) return null;

			login.Person = login.Person ?? pFuture.Value;
			login.SecurityGroupLogins = login.SecurityGroupLogins ?? sglFuture.ToList();

			return login;
		}

		#region Validation

		private bool ValidateEntity()
		{
			var results = new LoginValidator().Validate(_login);

			if (results.IsValid) return true;

			Warnings.AddRange(results.Errors.Select(error => new LoginWarning
			(
				error.ErrorCode.EnumTryParse<LoginWarningCode>(out var @enum) ? @enum : LoginWarningCode.PropertyNotValid,
				$"[{error.PropertyName}]: {error.ErrorMessage}"
			)));

			return false;
		}

		private bool ValidateDatabase()
		{
			if (LoginRepo.Table.Any(l => _login.LoginId != l.LoginId && _login.Email == l.Email))
				Warnings.Add(new LoginWarning(LoginWarningCode.DuplicateLoginEmail));
			else if (PersonRepo.Table.Any(p => _login.PersonId != p.PersonId && _login.Email == p.Email))
				Warnings.Add(new LoginWarning(LoginWarningCode.DuplicatePersonEmail));

			return !Warnings.Any();
		}

		#endregion

		#endregion
	}
}

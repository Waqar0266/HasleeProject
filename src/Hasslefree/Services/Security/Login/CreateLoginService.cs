using Hasslefree.Core;
using Hasslefree.Core.Crypto;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Helpers;
using Hasslefree.Data;
using Hasslefree.Services.Security.Validation;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Login
{
	public class CreateLoginService : ICreateLoginService
	{
		#region Private Properties

		// Repos
		private IDataRepository<LoginDb> LoginRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<SecurityGroup> SecurityGroupRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private LoginDb _login;
		private bool _passwordSet;

		#endregion

		#region Constructor

		public CreateLoginService
		(
			IDataRepository<LoginDb> loginRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<SecurityGroup> securityGroupRepo,
			IDataContext database
		)
		{
			// Repos
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SecurityGroupRepo = securityGroupRepo;

			// Other
			Database = database;
		}

		#endregion

		#region ICreateLoginDetailsService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !(ValidateEntity() && ValidateDatabase());
			}
		}

		public List<LoginWarning> Warnings { get; } = new List<LoginWarning>();

		public int LoginId { get; private set; }

		public ICreateLoginService New(int personId, string email, string salutation, bool active)
		{
			_login = new LoginDb
			{
				PersonId = personId,
				Email = email,
				Salutation = salutation,
				Active = active
			};

			return this;
		}

		public ICreateLoginService WithPassword(string password, string passwordSalt)
		{
			if (_login == null) return this;

			_passwordSet = true;

			_login.Password = password;
			_login.PasswordSalt = passwordSalt;

			return this;
		}

		public ICreateLoginService WithSecurityGroup(int securityGroupId)
		{
			if (_login == null || securityGroupId <= 0) return this;

			_login.SecurityGroupLogins = _login.SecurityGroupLogins ?? new List<SecurityGroupLogin>();

			_login.SecurityGroupLogins.Add(new SecurityGroupLogin
			{
				SecurityGroupId = securityGroupId
			});

			return this;
		}

		public ICreateLoginService WithSecurityGroups(IEnumerable<int> securityGroupIds)
		{
			var groupIds = securityGroupIds?.ToList();

			if (_login == null || (!groupIds?.Any() ?? true)) return this;

			_login.SecurityGroupLogins = _login.SecurityGroupLogins ?? new List<SecurityGroupLogin>();

			foreach (var id in groupIds.Where(id => _login.SecurityGroupLogins.All(sgl => sgl.SecurityGroupId != id)))
			{
				_login.SecurityGroupLogins.Add(new SecurityGroupLogin
				{
					SecurityGroupId = id
				});
			}

			return this;
		}

		public bool Create(bool saveChanges = true)
		{
			if (!_passwordSet) GeneratePassword();

			if (HasWarnings) return Clear(false);

			LoginRepo.Add(_login);

			if (!saveChanges) return Clear(true);

			Database.SaveChanges();
			LoginId = _login.LoginId;

			return Clear(true);
		}

		#endregion

		#region Private Methods

		private void GeneratePassword()
		{
			if (_login == null) return;

			_login.Password = !IsNullOrWhiteSpace(_login.Password) ? _login.Password : BaseX.GenerateString();
			_login.PasswordSalt = Hash.GetSalt();
			_login.Password = Hash.GetHashBase64(_login.Password, _login.PasswordSalt);
		}

		#region Validation

		private bool ValidateEntity()
		{
			var results = new LoginValidator(true).Validate(_login);

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
			var logins = LoginRepo.Table.ToList();

			if (_login.PersonId <= 0 || PersonRepo.Table.All(p => _login.PersonId != p.PersonId))
				Warnings.Add(new LoginWarning(LoginWarningCode.PersonNotFound));
			else if (logins.Any(l => _login.PersonId == l.PersonId))
				Warnings.Add(new LoginWarning(LoginWarningCode.ExistingLoginRecord));

			if (logins.Any(l => _login.Email == l.Email))
				Warnings.Add(new LoginWarning(LoginWarningCode.DuplicateLoginEmail, _login.Email));

			if (!_login.SecurityGroupLogins?.Any() ?? true) return !Warnings.Any();

			var ids = SecurityGroupRepo.Table.Select(sg => sg.SecurityGroupId).ToList();
			if (_login.SecurityGroupLogins.Select(sgl => sgl.SecurityGroupId).Any(id => !ids.Contains(id)))
				Warnings.Add(new LoginWarning(LoginWarningCode.SecurityGroupsNotFound));

			return !Warnings.Any();
		}

		#endregion

		private bool Clear(bool success)
		{
			_login = null;
			_passwordSet = false;

			return success;
		}

		#endregion
	}
}

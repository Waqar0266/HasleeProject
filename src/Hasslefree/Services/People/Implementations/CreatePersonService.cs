using Hasslefree.Core.Crypto;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.People.Validators;
using Hasslefree.Services.People.Warnings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.People.Implementations
{
	public class CreatePersonService : ICreatePersonService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Person> PersonRepo { get; }

		private IReadOnlyRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Core.Domain.Security.SecurityGroup> SecurityGroupRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Person _person;
		private readonly Dictionary<int, HashSet<int>> _attributeValues = new Dictionary<int, HashSet<int>>();

		private string _password;
		private string _passwordSalt;

		private int? _securityGroupId;

		#endregion

		#region Constructor

		public CreatePersonService
		(
			IDataRepository<Person> personRepo,
			IReadOnlyRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Core.Domain.Security.SecurityGroup> securityGroupRepo,
			IDataContext database
		)
		{
			// Repos
			PersonRepo = personRepo;
			SecurityGroupRepo = securityGroupRepo;

			// Other
			Database = database;
			LoginRepo = loginRepo;
		}

		#endregion

		#region ICreatePersonService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !(ValidateEntity() && ValidateDatabase());
			}
		}

		public List<PersonWarning> Warnings { get; } = new List<PersonWarning>();

		public int PersonId { get; set; }
		public int LoginId { get; set; }
		public Guid PersonGuid { get; set; }

		public ICreatePersonService WithSecurityGroup(string securityGroup)
		{
			var sg = SecurityGroupRepo.Table.FirstOrDefault(s => s.SecurityGroupName == securityGroup);
			if (sg == null)
			{
				sg = new Core.Domain.Security.SecurityGroup()
				{
					SecurityGroupDesc = securityGroup,
					SecurityGroupName = securityGroup
				};

				SecurityGroupRepo.Insert(sg);
			}

			_securityGroupId = sg.SecurityGroupId;

			return this;
		}

		public ICreatePersonService New
		(
			string firstName,
			string middleNames,
			string surname,
			string email,
			Titles title = Titles.Mr,
			string alias = null,
			Gender gender = Gender.Male,
			DateTime? birthday = null,
			PersonStatus status = PersonStatus.Enabled,
			string tag = null
		)
		{
			_person = new Person
			{
				Title = title.ToString(),
				FirstName = firstName,
				Surname = surname,
				Email = email,
				Birthday = birthday,
				Gender = gender,
				PersonStatus = status
			};

			return this;
		}

		public ICreatePersonService WithContactDetails
		(
			string phone = null,
			string fax = null,
			string mobile = null
		)
		{
			if (_person == null) return this;

			_person.Phone = phone;
			_person.Fax = fax;
			_person.Mobile = mobile;

			return this;
		}

		public ICreatePersonService WithKeyValue(string key, string value)
		{
			if (_person == null) return this;

			return this;
		}

		public ICreatePersonService WithAttribute(int attributeId, int attributeValueId)
		{
			if (_person == null) return this;

			if (_attributeValues.ContainsKey(attributeId))
			{
				if (!_attributeValues[attributeId].Contains(attributeValueId))
					_attributeValues[attributeId].Add(attributeValueId);
			}
			else
			{
				_attributeValues.Add(attributeId, new HashSet<int> { attributeValueId });
			}

			return this;
		}

		public ICreatePersonService WithPassword(string password, string passwordSalt)
		{
			if (!string.IsNullOrWhiteSpace(password)) _password = password;

			if (!string.IsNullOrWhiteSpace(passwordSalt)) _passwordSalt = passwordSalt;

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return Clear(false);

			CreateLogin();
			AddAttributes();

			PersonRepo.Add(_person);

			Database.SaveChanges();

			PersonId = _person.PersonId;
			LoginId = _person.Logins.FirstOrDefault().LoginId;
			PersonGuid = _person.PersonGuid;

			return Clear(true);
		}

		#endregion

		#region Private Methods

		#region Validation

		private bool ValidateEntity()
		{
			var results = new PersonValidator(true).Validate(_person);

			if (results.IsValid) return true;

			Warnings.AddRange(results.Errors.Select(error => new PersonWarning
			(
				TryParse(error.ErrorCode) ?? PersonWarningCode.PropertyNotValid,
				$"[{error.PropertyName}]: {error.ErrorMessage}"
			)));

			return false;
		}

		private bool ValidateDatabase()
		{
			if (PersonRepo.Table.Any(p => _person.Email == p.Email))
				Warnings.Add(new PersonWarning(PersonWarningCode.DuplicatePersonEmail, _person.Email));
			else if (LoginRepo.Table.Any(l => _person.Email == l.Email))
				Warnings.Add(new PersonWarning(PersonWarningCode.DuplicateLoginEmail, _person.Email));

			return !Warnings.Any();
		}

		private static PersonWarningCode? TryParse(string errorCode)
			=> Enum.TryParse(errorCode, out PersonWarningCode code) ? code : (PersonWarningCode?)null;

		#endregion

		private void CreateLogin()
		{
			_password = _password ?? BaseX.GenerateString();
			_passwordSalt = _passwordSalt ?? Hash.GetSalt();
			_password = Hash.GetHashBase64(_password, _passwordSalt);

			var newLogin = new Core.Domain.Security.Login
			{
				Email = _person.Email,
				Password = _password,
				PasswordSalt = _passwordSalt,
				Salutation = _person.Surname + ", " + _person.FirstName,
				Active = true,
			};

			if (_securityGroupId.HasValue)
				newLogin.SecurityGroupLogins.Add(new Core.Domain.Security.SecurityGroupLogin()
				{
					SecurityGroupId = _securityGroupId.Value
				});

			_person.Logins.Add(newLogin);

		}

		private void AddAttributes()
		{
			if (!(_attributeValues?.Any() ?? false)) return;
		}

		private bool Clear(bool success)
		{
			_person = null;
			_attributeValues.Clear();

			return success;
		}

		#endregion
	}
}

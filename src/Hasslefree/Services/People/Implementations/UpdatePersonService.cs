using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.Helpers;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.People.Validators;
using Hasslefree.Services.People.Warnings;
using Hasslefree.Services.Security.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.People.Implementations
{
	public class UpdatePersonService : IUpdatePersonService
	{
		#region Constants

		private readonly string[] _restrictedPersonProperties = { "PersonId", "CreatedOnUtc", "Email" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Person> PersonRepo { get; }

		// Services
		private IUpdateLoginService UpdateLoginService { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Person _person;
		private bool _loginUpdated;

		private readonly List<int> _addAttributeValues = new List<int>();

		#endregion

		#region Constructor

		public UpdatePersonService
		(
			IDataRepository<Person> personRepo,
			IUpdateLoginService updateLoginService,
			IDataContext database
		)
		{
			// Repos
			PersonRepo = personRepo;

			// Services
			UpdateLoginService = updateLoginService;

			// Other
			Database = database;
		}

		#endregion

		#region IUpdatePersonService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !(ValidateEntity() && ValidateDatabase());
			}
		}

		public List<PersonWarning> Warnings { get; } = new List<PersonWarning>();

		public IUpdatePersonService this[int personId]
		{
			get
			{
				if (personId <= 0) return this;

				_person = PersonQuery(personId);

				return this;
			}
		}

		public IUpdatePersonService WithPersonId(int personId) => this[personId];

		public IUpdatePersonService SetEmail(string email)
		{
			if (_person == null || IsNullOrWhiteSpace(email)) return this;

			_person.Email = email;

			var login = _person.Logins?.FirstOrDefault();

			if (login == null) UpdateLoginService.WithPersonId(_person.PersonId);
			else UpdateLoginService.WithLogin(login);

			UpdateLoginService.SetEmail(email);

			_loginUpdated = true;

			return this;
		}

		public IUpdatePersonService Set<T>(Expression<Func<Person, T>> lambda, object value)
		{
			_person?.SetPropertyValue(lambda, value, _restrictedPersonProperties);

			return this;
		}

		public IUpdatePersonService SetKeyValue(string key, string value)
		{
			if (_person == null || IsNullOrWhiteSpace(key)) return this;

			return this;
		}

		public IUpdatePersonService RemoveKeyValue(string key)
		{
			return this;
		}

		public IUpdatePersonService SetAttribute(int attributeValueId)
		{
			if (_person == null || attributeValueId <= 0) return this;

			return this;
		}

		public IUpdatePersonService RemoveAttribute(int attributeValueId)
		{
			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings) return false;

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				AddAttributes();

				if (_loginUpdated) UpdateLoginService.Update(false);

				_person.ModifiedOn = DateTime.Now;
				Database.SaveChanges();

				scope.Complete();
			}

			return true;
		}

		#endregion

		#region Private Methods

		private Person PersonQuery(int personId)
		{
			var personType = EntityType.Person.ToString();

			var pFuture = PersonRepo.Table.DeferredFirstOrDefault(p => p.PersonId == personId).FutureValue();

			var person = pFuture.Value;

			if (person == null) return null;


			return person;
		}

		#region Validation

		private bool ValidateEntity()
		{
			var results = new PersonValidator().Validate(_person);

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
			if (PersonRepo.Table.Any(p => p.PersonId != _person.PersonId && p.Email == _person.Email))
				Warnings.Add(new PersonWarning(PersonWarningCode.DuplicatePersonEmail, _person.Email));

			else if (_loginUpdated)
			{
				if (UpdateLoginService.HasWarnings)
					UpdateLoginService.Warnings.ForEach(w => Warnings.Add(new PersonWarning(PersonWarningCode.UpdateLoginServiceError, $"[{w.Code}] {w.Message}")));
			}

			return !Warnings.Any();
		}

		private static PersonWarningCode? TryParse(string errorCode) => Enum.TryParse(errorCode, out PersonWarningCode code) ? code : (PersonWarningCode?)null;

		#endregion

		private void AddAttributes()
		{
			if (!(_addAttributeValues?.Any() ?? false)) return;

			var personType = EntityType.Person.ToString();
		}

		#endregion
	}
}

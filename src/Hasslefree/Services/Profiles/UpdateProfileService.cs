using Hasslefree.Core.Configuration;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Common;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.People;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hasslefree.Services.Emails;
using static System.String;

namespace Hasslefree.Services.Profiles
{
	public class UpdateProfileService : IUpdateProfileService
	{
		#region Constants

		private const string RestrictedAccountProperties = "AccountId, AccountNo, CreatedOnUtc, AccountStatus";
		private const string RestrictedAddressProperties = "AddressId";
		private const string RestrictedPersonProperties = "PersonId";

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Person> PersonRepo { get; }

		// Services
		private IUpdateLoginProfileService UpdateLoginProfileService { get; }

		// Other
		private IDataContext Database { get; }
		private ISendMail SendMail { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Fields

		private readonly List<AddressBuilder> _addresses = new List<AddressBuilder>();
		private bool _usePerson;

		#endregion

		#region Constructor

		public UpdateProfileService
		(
			IDataRepository<Person> personRepo,
			IUpdateLoginProfileService updateLoginProfileService,
			IDataContext database,
			ISendMail sendMail,
			ISettingsService settings,
			ISessionManager sessionManager
		)
		{
			// Repos
			PersonRepo = personRepo;

			// Services
			UpdateLoginProfileService = updateLoginProfileService;

			// Other
			Database = database;
			SendMail = sendMail;
			SessionManager = sessionManager;
		}

		#endregion

		#region IUpdateProfileService

		public bool HasWarnings => Warnings.Any();

		public List<UpdateProfileWarning> Warnings { get; set; } = new List<UpdateProfileWarning>();

		/* New Address */
		public AddressBuilder AddAddress(string address1, string address2, string suburb, string town, string country, string region, string code)
		{
			var address = new AddressBuilder(address1, address2, suburb, town, country, region, code);
			_addresses.Add(address);
			return address;
		}

		public IUpdateProfileService SetPerson<T>(Expression<Func<Person, T>> lambda, object value)
		{
			// Get the person
			var personId = SessionManager.Login.PersonId;
			var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == personId);

			if (person == null)
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.PersonNotFound));
				return this;
			}

			// Get the property selector
			var selector = lambda.Body as MemberExpression;
			if (selector == null) return this;

			// Identify the property
			var property = selector.Member as PropertyInfo;

			// Unknown property
			if (property == null)
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.PropertyNotFound));
				return this;
			}

			//Check for duplicate email
			if (property.Name == "Email")
			{
				var emailChanged = (string)property.GetValue(person) != (string)value;
				if (!emailChanged) return this;
				var dbEmails = PersonRepo.Table.Any(p => p.Email == value.ToString());

				if (dbEmails)
				{
					Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.DuplicateEmail));
					return this;
				}

				var loginId = person.Logins.FirstOrDefault(p => p.Email == person.Email)?.LoginId ?? SessionManager.Login.LoginId;
				UpdateLoginProfileService.WithLoginId(loginId).SetLogin(l => l.Email, value);

				if (UpdateLoginProfileService.HasWarnings)
				{
					Warnings.AddRange(UpdateLoginProfileService.Warnings);
					return this;
				}
			}

			// Is the property restricted
			if (RestrictedPersonProperties.Contains(property.Name))
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.Restricted));
				return this;
			}

			// Update
			property.SetValue(person, value, null);

			return this;
		}

		public IUpdateProfileService SetAddress<T>(int addressId, Expression<Func<Address, T>> lambda, object value)
		{
			//// Get the address
			//var address = _account.Addresses.FirstOrDefault(a => a.AddressId == addressId).Address;
			//if (address == null)
			//{
			//	Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.AddressNotFound));
			//	return this;
			//}

			//// Get the property selector
			//if (!(lambda.Body is MemberExpression selector)) return this;

			//// Identify the property
			//var property = selector.Member as PropertyInfo;

			//// Unknown property
			//if (property == null)
			//{
			//	Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.PropertyNotFound));
			//	return this;
			//}

			//// Is the property restricted
			//if (RestrictedAddressProperties.Contains(property.Name))
			//{
			//	Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.Restricted));
			//	return this;
			//}

			//// Update
			//property.SetValue(address, value, null);

			return this;
		}

		public IUpdateProfileService DeleteAddress(int addressId)
		{
			//// Get the account
			//_account = _account ?? GetAccount();
			//if (_account == null)
			//{
			//	Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.AccountNotFound));
			//	return this;
			//}

			//// Get the address
			//var address = _account.Addresses.FirstOrDefault(a => a.AddressId == addressId);
			//if (address == null)
			//{
			//	Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.AddressNotFound));
			//	return this;
			//}

			//// Delete the address
			//_account.Addresses.Remove(address);

			return this;
		}

		public IUpdateProfileService UsePerson()
		{
			_usePerson = true;
			return this;
		}

		/* Update */
		public bool Update()
		{
			if (HasWarnings) return false;

			//get the account
			var personId = SessionManager.Login.PersonId;
			var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == personId);

			//Commit changes
			Database.SaveChanges();

			//Clear Account cache
			SessionManager.ClearAccountCache();
			SessionManager.ClearLoginCache();

			return true;
		}

		#endregion
	}
}

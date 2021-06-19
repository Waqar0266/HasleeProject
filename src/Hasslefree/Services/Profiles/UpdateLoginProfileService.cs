using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Services.Emails;
using Hasslefree.Services.People;

namespace Hasslefree.Services.Profiles
{
	public class UpdateLoginProfileService : IUpdateLoginProfileService
	{
		#region Constants

		private const string RestrictedPersonProperties = "LoginId";

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		// Services
		private ISendMail SendMail { get; }
		private ISettingsService SettingsService { get; }

		// Other
		private IDataContext Database { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Fields

		private Core.Domain.Security.Login _login;


		#endregion

		#region Constructor

		public UpdateLoginProfileService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			ISendMail sendMail,
			ISettingsService settingsService,
			IDataContext database,
			ISessionManager sessionManager
		)
		{
			// Repos
			LoginRepo = loginRepo;

			// Services
			SendMail = sendMail;
			SettingsService = settingsService;

			// Other
			Database = database;
			SessionManager = sessionManager;
			
		}

		#endregion

		#region IUpdateLoginService

		public bool HasWarnings => Warnings.Any();

		public List<UpdateProfileWarning> Warnings { get; } = new List<UpdateProfileWarning>();

		public IUpdateLoginProfileService WithLoginId(int loginId)
		{
			_login = LoginRepo.GetById(loginId);

			return this;
		}

		public IUpdateLoginProfileService WithLogin(Core.Domain.Security.Login login)
		{
			_login = login;

			return this;
		}

		public IUpdateLoginProfileService SetLogin<T>(Expression<Func<Core.Domain.Security.Login, T>> lambda, object value)
		{
			_login = _login ?? SessionManager.Login;

			if (_login == null)
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.LoginNotFound));
				return this;
			}

			var selector = lambda.Body as MemberExpression;
			if (selector == null) return this;

			var property = selector.Member as PropertyInfo;
			if (property == null)
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.PropertyNotFound));
				return this;
			}

			if (property.Name == "Email")
			{
				var emailChanged = (string)property.GetValue(_login) != (string)value;
				if (!emailChanged) return this;

				var dbEmails = LoginRepo.Table.Any(l => l.Email == value.ToString());

				if (dbEmails)
				{
					Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.DuplicateEmail));
					return this;
				}
			}

			if (RestrictedPersonProperties.Contains(property.Name))
			{
				Warnings.Add(new UpdateProfileWarning(UpdateProfileWarningCode.Restricted));
				return this;
			}

			property.SetValue(_login, value, null);

			return this;
		}

		public bool Update()
		{
			if (HasWarnings) return false;

			Database.SaveChanges();

			SessionManager.ClearAccountCache();
			SessionManager.ClearLoginCache();

			return true;
		}

		#endregion
	}
}

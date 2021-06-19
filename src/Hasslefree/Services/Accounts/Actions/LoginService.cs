using Hasslefree.Core;
using Hasslefree.Core.Configuration.Session;
using Hasslefree.Core.Crypto;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core.Configuration;
using Z.EntityFramework.Plus;
using Hasslefree.Web.Models.Accounts;

namespace Hasslefree.Services.Accounts.Actions
{
	public class LoginService : ILoginService
	{
		#region Constants

		private const string CookieSession = "Session";

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Session> SessionRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }

		// Services
		private ISettingsService SettingsService { get; }

		// Other
		private IDataContext Database { get; }
		private ISessionManager SessionManager { get; }
		private IWebHelper WebHelper { get; }

		#endregion

		#region Fields

		private string _email;
		private string _password;
		private bool _remember;
		private Guid _guid;

		#endregion

		#region Constructor

		public LoginService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Session> sessionRepo,
			ISettingsService settingsService,
			IDataContext database,
			IWebHelper webHelper,
			IDataRepository<Person> personRepo,
			ISessionManager sessionManager
		)
		{
			// Repos
			LoginRepo = loginRepo;
			SessionRepo = sessionRepo;
			PersonRepo = personRepo;

			// Services
			SettingsService = settingsService;

			// Other
			Database = database;
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region ILoginService 

		public bool HasWarnings => Warnings.Any();

		public List<LoginWarning> Warnings { get; } = new List<LoginWarning>();

		/// <summary>
		/// Set the local email
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public ILoginService WithEmail(string email)
		{
			// Validate the input
			if (String.IsNullOrWhiteSpace(email)) Warnings.Add(new LoginWarning(LoginWarningCode.InvalidCredentials));

			_email = email;
			return this;
		}

		/// <summary>
		/// Set the password
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public ILoginService WithPassword(string password)
		{
			// Validate the input
			if (String.IsNullOrWhiteSpace(password)) Warnings.Add(new LoginWarning(LoginWarningCode.InvalidCredentials));

			_password = password;
			return this;
		}

		/// <summary>
		/// Set the remember me
		/// </summary>
		/// <param name="remember"></param>
		/// <returns></returns>
		public ILoginService Remember(bool remember)
		{
			_remember = remember;
			return this;
		}

		/// <summary>
		/// Set the local person guid
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public ILoginService WithGuid(Guid guid)
		{
			_guid = guid;
			return this;
		}

		/// <summary>
		/// Use the provided credentials to update the current Session & Cart
		/// </summary>
		/// <returns></returns>
		public bool Login()
		{
			// Check for warnings before accessing the database
			if (HasWarnings) return false;

			// Get some ids
			var sessionId = SessionManager?.Session?.SessionId ?? 0;

			// Prepare some queries
			GetPreparedQueries(sessionId, out var queryLogin, out var querySession);

			// Execute the queries
			var loginObject = queryLogin.Value;
			var login = loginObject?.GetLogin();

			var session = querySession?.Value;

			// Check if the login is valid
			if (!ValidateLogin(login)) return false;

			// Get the session settings
			var settings = SettingsService.LoadSetting<SessionSettings>();

			// Calculate the date when the session cookie should expire
			var expiresOn = settings.SessionExpires ? DateTime.UtcNow.AddMonths(settings.SessionExpireMonths) : DateTime.UtcNow.AddMonths(6);

			// Update the session record in the database
			if (session != null)
			{
				session.LoginId = login?.LoginId;
				session.ModifiedOn = DateTime.Now;
				session.ExpiresOn = _remember ? (DateTime?)null : expiresOn;
			}

			// Save Changes
			Database.SaveChanges();

			// Set the expiry date on the cookie
			WebHelper.SetCookieExpireDate(CookieSession, expiresOn);

			// Clear the session cache
			SessionManager?.ClearSessionCache();

			return true;
		}

		public bool ValidateLogin()
		{
			var sessionId = SessionManager?.Session?.SessionId ?? 0;
			GetPreparedQueries(sessionId, out var queryLogin, out var querySession, true);
			var loginObject = queryLogin.Value;
			var login = loginObject?.GetLogin();

			var session = querySession?.Value;

			// Check if the login is valid
			if (ValidateLogin(login)) return true;
			return false;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Get prepared queries
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="documentId"></param>
		/// <param name="login"></param> 
		/// <param name="session"></param>
		/// <param name="document"></param>
		private void GetPreparedQueries(int sessionId, out QueryFutureValue<LoginObject> login, out QueryFutureValue<Session> session, bool anysite = false)
		{
			login = (from l in LoginRepo.Table
					 join p in PersonRepo.Table on l.PersonId equals p.PersonId
					 where p.PersonGuid == _guid || l.Email == _email
					 select new LoginObject()
					 {
						 LoginId = l.LoginId,
						 PersonId = p.PersonId,
						 CreatedOn = l.CreatedOn,
						 ModifiedOn = l.ModifiedOn,
						 Email = l.Email,
						 Password = l.Password,
						 PasswordSalt = l.PasswordSalt,
						 Salutation = l.Salutation,
						 Active = l.Active,
						 Person = p
					 }).FutureValue();

			// To get the session
			session = sessionId > 0 ? SessionRepo.Table
					.Where(s => s.SessionId == sessionId)
					.DeferredFirstOrDefault()
					.FutureValue()
				: null;
		}

		/// <summary>
		/// Validate the login
		/// </summary>
		private bool ValidateLogin(Core.Domain.Security.Login login)
		{
			if (login?.Person == null)
			{
				Warnings.Add(new LoginWarning(LoginWarningCode.NotFound));
				return false;
			}

			if (!login.Active)
			{
				Warnings.Add(new LoginWarning(LoginWarningCode.LoginDeactivate));
				return false;
			}

			var person = login.Person;

			// Check if the person is enabled
			if (person.PersonStatus != PersonStatus.Enabled)
			{
				Warnings.Add(new LoginWarning(LoginWarningCode.AccountDisabled));
				return false;
			}

			// Got guid? No authentication needed.
			if (_guid != Guid.Empty) return true;

			// Hash the provided password
			var hash = Hash.GetHashBase64(_password, login.PasswordSalt);

			// Validate the credentials
			if (hash == login.Password) return true;

			Warnings.Add(new LoginWarning(LoginWarningCode.InvalidCredentials));
			return false;
		}

		#endregion

		#region Private Classes
		public class LoginObject
		{
			public int LoginId { get; set; }
			public int PersonId { get; set; }
			public DateTime CreatedOn { get; set; }
			public DateTime ModifiedOn { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
			public string PasswordSalt { get; set; }
			public string Salutation { get; set; }
			public bool Active { get; set; }
			public Person Person { get; set; }

			public Core.Domain.Security.Login GetLogin()
			{
				return new Core.Domain.Security.Login()
				{
					LoginId = LoginId,
					PersonId = PersonId,
					CreatedOn = CreatedOn,
					ModifiedOn = ModifiedOn,
					Email = Email,
					Password = Password,
					PasswordSalt = PasswordSalt,
					Salutation = Salutation,
					Active = Active,
					Person = Person,
				};
			}
		}

		#endregion
	}

}






using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Web.Framework
{
	public class SessionManager : ISessionManager
	{
		#region Constants

		private const string BotSessionReference = "BOT-SESSION";
		private const string DefaultReference = "DEFAULT-SESSION";

		#endregion

		#region Private Variables

		private Session _session;
		private Login _login;
		private string _reference;

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Login> LoginRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }
		private IDataRepository<Session> SessionRepo { get; }

		// Other
		private ICacheManager CacheManager { get; }
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public SessionManager
		(
			IDataRepository<Login> loginRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo,
			IDataRepository<Session> sessionRepo,
			ICacheManager cacheManager,
			IWebHelper webHelper
		)
		{
			// Repos
			LoginRepo = loginRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
			SessionRepo = sessionRepo;

			// Other
			CacheManager = cacheManager;
			WebHelper = webHelper;
		}

		#endregion

		/// <summary>
		/// Gets an instance of the SessionManager from the engine context
		/// </summary>
		public static ISessionManager Current => EngineContext.Current.Resolve<ISessionManager>();

		#region ISessionManager

		public ISessionManager WithReference(string reference)
		{
			_reference = reference;
			return this;
		}

		/// <summary>
		/// Get the current session
		/// </summary>
		public Session Session
		{
			get
			{
				// If the session has already been loaded then return it
				if (_session != null) return _session;

				// Determine how to load the session
				string reference = _reference;
				if (String.IsNullOrEmpty(reference))
				{
					// Get the session reference from the cookie
					var sessionCookie = WebHelper.GetCookie("Session");
					reference = sessionCookie?.Value ?? "";
				}

				// Check if this is a bot. If it is a Bot, use the fixed reference
				if (WebHelper.IsBotRequest())
					reference = $"{BotSessionReference}";

				if (String.IsNullOrWhiteSpace(reference)) reference = DefaultReference;

				// Get the session and load it into cache since it gets used a lot
				_session = CacheManager.Get(CacheKeys.Store.Session.PatternSession(reference), CacheKeys.Time.LongTime, () =>
				{
					if (reference.Equals(DefaultReference))
						return GenerateDefaultSession();

					return SessionRepo.Table
										.AsNoTracking()
										.FirstOrDefault(a => a.Reference == reference);
				});

				if (_session != null) return _session;

				_session = GenerateDefaultSession();
				WebHelper.DeleteCookie("Session");
				CacheManager.Remove(CacheKeys.Store.Session.PatternSession(reference));

				// Return the session
				return _session;
			}
		}

		/// <summary>
		/// Get the person logged in for the current session
		/// </summary>
		public Login Login
		{
			get
			{
				// If the person has already been loaded then use it
				if (_login != null) return _login;

				// Check if someone is logged in
				if (Session?.LoginId == null) return null;

				// ReSharper disable UnusedVariable
				// Get the logged in user's account information and permissions
				_login = CacheManager.Get(CacheKeys.Store.Session.Login(Session.Reference), CacheKeys.Time.LongTime, () =>
				{
					var loginId = Session.LoginId.Value;
					var accountFuture = LoginRepo.Table
													.Include(l => l.Person)
													.Where(l => l.LoginId == loginId)
													.Future();

					var permissionFuture = SecurityGroupLoginRepo.Table
																	.Include(sgl => sgl.SecurityGroup)
																	.Include(sgl => sgl.SecurityGroup.Permissions)
																	.Where(sgl => sgl.LoginId == loginId)
																	.Future();

					return accountFuture.FirstOrDefault();
				});

				return _login;
			}
		}

		/// <summary>
		/// Get the Location from the current session
		/// </summary>
		public LocationModel Location => new LocationModel
		{
			IP = Session.IpAddress,
			Latitude = Session.Latitude,
			Longtitude = Session.Longitude
		};


		#region Public Methods

		/// <summary>
		/// Clear all cache for the session
		/// </summary>
		public void ClearSessionCache()
		{
			//check if there is an existing cookie (API)
			if (_session == null) return;

			// Remove all cached items for the session
			CacheManager.RemoveByPattern(CacheKeys.Store.Session.PatternSession(_session.Reference ?? ""));

			// Empty all related objects so that they reload
			_session = null;
			_login = null;
		}

		/// <summary>
		/// Remove the session's cart
		/// </summary>
		public void ClearCartCache()
		{
			// Remove cache
			CacheManager.Remove(CacheKeys.Store.Session.Cart(Session.Reference));
		}

		/// <summary>
		/// Remove the session's login
		/// </summary>
		public void ClearLoginCache()
		{
			// Remove cache
			CacheManager.Remove(CacheKeys.Store.Session.Login(Session.Reference));

			// Empty all related objects so that they reload
			_login = null;
		}

		/// <summary>
		/// Remove the session's person
		/// </summary>
		public void ClearAccountCache()
		{
			// Remove cache
			CacheManager.Remove(CacheKeys.Store.Session.Account(Session.Reference));
		}

		/// <summary>
		/// Check if someone is logged in for the session
		/// </summary>
		/// <returns></returns>
		public bool IsLoggedIn() => Session?.LoginId != null;

		#endregion

		#endregion

		#region Private Methods

		private Session GenerateDefaultSession()
		{
			return new Session()
			{
				CreatedOn = DateTime.Now,
				ModifiedOn = DateTime.Now,
				Reference = DefaultReference,
				SessionId = 0,
				IpAddress = "",
				Latitude = 0,
				Longitude = 0
			};
		}

		#endregion
	}
}

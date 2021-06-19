using Hasslefree.Core;
using Hasslefree.Core.Configuration.Session;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using static System.String;

namespace Hasslefree.Web.Framework.Filters
{
	public class SessionFilter : ActionFilterAttribute
	{
		/* Cache Keys */
		private const string BotSessionReference = "BOT-SESSION";

		/* Fields */
		private HttpCookie _sessionCookie;
		private string _sessionReference;
		private bool _isBot;

		/* Dependencies */
		public IDataRepository<Session> SessionRepo { get; set; }
		public ISettingsService SettingsService { get; set; }
		public ICacheManager CacheManager { get; set; }
		public IWebHelper WebHelper { get; set; }
		public ISessionManager SessionManager { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// Do nothing for child actions
			if (filterContext.IsChildAction) return;

			_isBot = WebHelper.IsBotRequest();

			// Initialize the filter
			Init();

			// Get the current session from the cache
			var cached = TryGetSessionFromCache(out var session);

			// If it is not in the cache then check the database
			if (!cached)
			{
				var exists = TryGetSessionFromDatabase(out session);

				// If it does not exist then create it
				if (!exists) session = CreateNewSession();

				// If it exists then check for any changes
				else UpdateSession(session);

				// Make sure the session gets cleared
				SessionManager.ClearSessionCache();

				// Cache the session for the next action
				CacheSession(session);
			}
		}

		#region Private Methods

		/// <summary>
		/// Set some frequently used objects
		/// </summary>
		private void Init()
		{
			// If this is a bot, don't even use the WebHelper
			if (_isBot)
			{
				_sessionReference = $"{BotSessionReference}";

				// Create the session cookie to make sure it is always there
				WebHelper.SetCookieValue("Session", _sessionReference, DateTime.UtcNow.AddMonths(6));

				return;
			}

			_sessionCookie = WebHelper.GetCookie("Session");
			_sessionReference = _sessionCookie?.Value ?? "";
		}

		/// <summary>
		/// Get the Geo Origin data
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <returns></returns>
		private GeoLocationModel GetGeoOrigin(string ipAddress)
		{
			return new GeoLocationModel()
			{
				IP = ipAddress,
				City = "",
				Country_Name = "",
				Latitude = 0,
				Longitude = 0,
				Region_Code = "",
				Region_Name = ""
			};
			//var trackingDetails = GeoService.GetLocation(ipAddress);
			//var geoData = trackingDetails.Content.ReadAsStringAsync().Result;
			//return JsonConvert.DeserializeObject<GeoLocationModel>(geoData.Replace("%0a", ""));
		}

		/// <summary>
		/// Get the session from the cache without the assistance of the database
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		private bool TryGetSessionFromCache(out Session session)
		{
			// Get the session from cache
			session = CacheManager.Get<Session>(CacheKeys.Server.Framework.Filter.Session(_sessionReference));

			// Return true if the session exists in the cache
			return session != null;
		}

		/// <summary>
		/// Get the session from the database
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		private bool TryGetSessionFromDatabase(out Session session)
		{
			// Get the session from the database
			session = SessionRepo.Table.FirstOrDefault(s => s.Reference == _sessionReference);

			// Return true if the session exists in the cache
			return session != null && (session.ExpiresOn == null || session.ExpiresOn > DateTime.UtcNow);
		}

		/// <summary>
		/// Create a new instance
		/// </summary>
		/// <returns></returns>
		private Session CreateNewSession()
		{
			// Get the GeoOrigin
			var originIp = WebHelper.GetRequestIp();

			// New Session model
			var session = new Session();

			// Get the current geo origin
			var geoOrigin = GetGeoOrigin(originIp);

			// Set the origin details for the session
			session.IpAddress = geoOrigin.IP;
			session.Latitude = geoOrigin.Latitude;
			session.Longitude = geoOrigin.Longitude;

			// Setting to check if GeoOrigin should be loaded
			var sessionSettings = SettingsService.LoadSetting<SessionSettings>();

			// Check if the session expires
			if (sessionSettings.SessionExpires)
				session.ExpiresOn = DateTime.UtcNow.AddMonths(sessionSettings.SessionExpireMonths);

			// If this is a bot, create a fixed reference.
			if (_isBot)
			{
				session.Reference = _sessionReference;

				// Bot sessions cannot expire. Ever.
				session.ExpiresOn = null;
			}

			// Insert the session
			SessionRepo.Insert(session);

			// Create cookie for the session
			WebHelper.SetCookieValue("Session", session.Reference, session.ExpiresOn ?? DateTime.UtcNow.AddMonths(3));
			_sessionReference = session.Reference;

			// Return the created session
			return session;
		}

		/// <summary>
		/// Update the existing session
		/// </summary>
		/// <param name="session"></param>
		private void UpdateSession(Session session)
		{
			// Track any changes made
			bool applyUpdate = false;

			// Get the GeoOrigin
			var originIp = WebHelper.GetRequestIp();

			// If the origin IP is still the same as in the past then make no changes to the session
			if (session.IpAddress != originIp)
			{
				// An update is required since the IP has changed
				applyUpdate = true;

				// Get the current geo origin
				var geoOrigin = GetGeoOrigin(originIp);

				// Minor fix-up for the geoOrigin data
				geoOrigin.Region_Name = geoOrigin.Region_Name.Replace("Province of the", "");
				session.IpAddress = geoOrigin.IP;
				session.Latitude = geoOrigin.Latitude;
				session.Longitude = geoOrigin.Longitude;
			}

			// Load the GeoOrigin settings
			var sessionSettings = SettingsService.LoadSetting<SessionSettings>();

			// If the session does have an expiry date then increase the lifespan of the session
			if (sessionSettings.SessionExpires)
			{
				session.ExpiresOn = DateTime.UtcNow.AddMonths(sessionSettings.SessionExpireMonths);
				applyUpdate = true;
			}
			else if (!sessionSettings.SessionExpires && session.ExpiresOn != null)
			{
				session.ExpiresOn = null;
				applyUpdate = true;
			}

			// If any changes were made made then apply them
			if (applyUpdate)
			{
				// Update the session
				SessionRepo.Update(session);

				// Update the cookie expire date
				WebHelper.SetCookieExpireDate("Session", session.ExpiresOn ?? DateTime.UtcNow.AddMonths(3));
			}

			// ... else, if the session has no expire date, extend the cookie lifetime
			else if (session.ExpiresOn == null)
			{
				WebHelper.SetCookieExpireDate("Session", DateTime.UtcNow.AddMonths(3));
			}
		}

		/// <summary>
		/// Add the current session to the cache
		/// </summary>
		/// <param name="session"></param>
		private void CacheSession(Session session)
		{
			// Build the key for the cache
			var key = CacheKeys.Server.Framework.Filter.Session(_sessionReference);

			// If the session does not have an expire date then cache it for 12 hours
			if (!session.ExpiresOn.HasValue)
			{
				CacheManager.Set(key, session, 720);
			}
			// Else try to align the expiration dates
			else
			{
				var dateDif = session.ExpiresOn - DateTime.UtcNow;
				CacheManager.Set(key, session, dateDif.Value.Hours);
			}
		}

		#endregion

		#region Private Classes

		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private class GeoLocationModel
		{
			public string IP { get; set; }
			public string Country_Name { get; set; }
			public string Region_Name { get; set; }
			public string Region_Code { get; set; }
			public string City { get; set; }
			public double Latitude { get; set; }
			public double Longitude { get; set; }
		}

		#endregion
	}
}

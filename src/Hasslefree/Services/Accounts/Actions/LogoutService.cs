using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using System.Linq;

namespace Hasslefree.Services.Accounts.Actions
{
	public class LogoutService : ILogoutService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Session> SessionRepo { get; }

		// Other
		private ISessionManager SessionManager { get; }
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public LogoutService
		(
			IDataRepository<Session> sessionRepo,
			ISessionManager sessionManager,
			IWebHelper webHelper
		)
		{
			// Repos
			SessionRepo = sessionRepo;

			// Other
			SessionManager = sessionManager;
			WebHelper = webHelper;
		}

		#endregion

		#region ILogoutService

		/// <summary>
		/// Log Out!.
		/// Deletes all cookies, updates the session and cart
		/// </summary>
		public void Logout()
		{
			// Ignore unless someone is logged in
			if (!SessionManager.IsLoggedIn()) return;

			// Get some ids
			var sessionId = SessionManager?.Session?.SessionId ?? 0;

			// delete the session
			if (sessionId > 0)
			{
				var session = SessionRepo.Table.FirstOrDefault(s => s.SessionId == sessionId);
				SessionRepo.Delete(session);
			}

			// Delete any 3rd party cookies
			WebHelper.DeleteCookie("Session");

			// Clear the session cache
			SessionManager?.ClearLoginCache();
			SessionManager?.ClearSessionCache();
		}

		#endregion
	}
}

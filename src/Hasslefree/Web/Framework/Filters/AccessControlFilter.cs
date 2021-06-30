using Hasslefree.Core.Sessions;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Filters
{
	/// <inheritdoc />
	/// <summary>
	/// Only allow authenticated users to access this page.
	/// Optionally check if the user has a specifix role
	/// </summary>
	public class AccessControlFilter : AuthorizeAttribute
	{
		/// <summary>
		/// The default login url
		/// </summary>
		private const string DefaultLoginUrl = "/account/login";

		public string Permission { get; set; }

		/// <summary>
		/// The security service
		/// </summary>
		public Services.Security.ISecurityService SecurityService { get; set; }

		/// <summary>
		/// Session manager
		/// </summary>
		public ISessionManager SessionManager { get; set; }

		/// <summary>
		/// The settings service
		/// </summary>
		public Services.Configuration.ISettingsService SettingsService { get; set; }

		/// <summary>
		/// The login url to use (instead of the default login url)
		/// </summary>
		public string LoginUrl { get; set; }

		#region Methods

		/// <inheritdoc />
		/// <summary>
		/// Perform a access control check on the currently logged in user
		/// </summary>
		/// <param name="filterContext"></param>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			// Do nothing when headers already sent
			if (filterContext.HttpContext.Response.HeadersWritten) return;

			// Ignore child actions
			if (filterContext.Controller.ControllerContext.IsChildAction) return;

			//Check if anonymous access is allowed on this action
			if (filterContext.ActionDescriptor.GetCustomAttributes(true).Any(a => a is AllowAnonymousAttribute)) return;

			//Get the request object
			var request = filterContext.HttpContext.Request;

			//Get the details about this request
			var url = request.Url;
			var path = url?.PathAndQuery; //e.g. /backoffice/products/edit?id=1

			//Generate the login url
			var loginUrl = String.IsNullOrEmpty(LoginUrl) ? DefaultLoginUrl : LoginUrl;

			//Add the redirect to the login url
			loginUrl += "?redirectTo=" + HttpUtility.UrlEncode(path);

			// Check for login
			if (!SessionManager.IsLoggedIn() || SessionManager.Login?.Person == null)
			{
				filterContext.Result = new RedirectResult(loginUrl);
				return;
			}

			// Check if permission is set on the wrong paramter
			if (!String.IsNullOrWhiteSpace(Roles) && String.IsNullOrWhiteSpace(Permission)) Permission = Roles;

			// No roles ... no check needed
			if (String.IsNullOrWhiteSpace(Permission)) return;

			if (!SecurityService.IsInSecurityGroup(SessionManager.Login.LoginId, Permission)) filterContext.Result = new RedirectResult("/error/403");
		}
		#endregion
	}
}

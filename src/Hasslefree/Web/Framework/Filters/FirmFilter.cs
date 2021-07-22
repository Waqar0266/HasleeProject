using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Common;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Filters
{
	public class FirmFilter : AuthorizeAttribute
	{
		/// <summary>
		/// Session manager
		/// </summary>
		public ISessionManager SessionManager { get; set; }

		/// <summary>
		/// Get firm settings service
		/// </summary>
		public IGetFirmService GetFirm { get; set; }

		/// <summary>
		/// The Agent repo
		/// </summary>
		public IReadOnlyRepository<Agent> AgentRepo { get; set; }

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

			// Check for login
			if (!SessionManager.IsLoggedIn()) return;

			var firm = GetFirm.Get();
			if ((String.IsNullOrEmpty(firm.BusinessName) || String.IsNullOrEmpty(firm.Email) || String.IsNullOrEmpty(firm.PhysicalAddress1) || String.IsNullOrEmpty(firm.PostalAddress1)) && !request.Url.AbsolutePath.Contains("manage-firm"))
			{
				filterContext.Controller.ViewBag.ForceFirmSettings = true;
				filterContext.Result = new RedirectResult($"/account/manage-firm");
				return;
			}
		}

		#endregion
	}
}

using Hasslefree.Core.Logging;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Web.Framework;
using System;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class LogoutController : BaseController
	{
		/* Dependencies */
		private ILogoutService LogoutService { get; }

		/* CTOR */
		public LogoutController(
			ILogoutService logoutService)
		{
			LogoutService = logoutService;
		}

		/// <summary>
		/// Log the user out of the system
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		[Route("account/logout")]
		public ActionResult Logout()
		{
			try
			{
				// Update the buying entity for the cart (if any)
				LogoutService.Logout();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}

			// Go home!
			return Redirect("/");
		}
	}
}
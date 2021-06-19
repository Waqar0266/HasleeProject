using Hasslefree.Core.Sessions;
using Hasslefree.Services.Profiles;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Accounts;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	[AccessControlFilter]
	public class ChangePasswordController : BaseController
	{
		/* Dependencies */
		private IChangeProfilePasswordService ChangePasswordService { get; }

		/* CTOR */
		public ChangePasswordController
		(
			ISessionManager sessionManager,
			IChangeProfilePasswordService changePasswordService
		)
		{
			ChangePasswordService = changePasswordService;
		}

		/* GET */
		[HttpGet]
		[Route("account/change-password")]
		public ActionResult Index()
		{
			// Get the model
			var model = new ChangePasswordModel();

			ViewBag.Title = "Change Password";

			// View
			return View("../Accounts/ChangePassword", model);
		}

		/* POST */
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("account/change-password")]
		public ActionResult Index(ChangePasswordModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Update
					var success = ChangePasswordService
						.WithCurrentPassword(model.OldPassword)
						.WithNewPassword(model.NewPassword)
						.Update();

					if (success)
					{
						TempData["Success"] = "You have successully updated your password!";
						return Redirect("/account/change-password");
					}

					// Not found
					ModelState.AddModelError(string.Empty, ChangePasswordService?.Warnings?.FirstOrDefault()?.Message ?? "Profile not found");
				}
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Core.Logging.Logger.LogError(ex);
				ModelState.AddModelError(string.Empty, ex.Message);
			}

			ViewBag.Title = "Change Password";

			return View("../Accounts/ChangePassword", model);
		}
	}
}
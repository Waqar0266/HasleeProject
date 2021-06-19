using Hasslefree.Core;
using Hasslefree.Core.Logging;
using Hasslefree.Services.Accounts.Password;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Accounts;
using System;
using System.Linq;
using System.Web.Mvc;
using static System.String;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class ResetPasswordController : BaseController
	{
		/* Dependencies */
		private IWebHelper WebHelper { get; }
		private IResetPasswordService ResetService { get; }

		/* CTOR */
		public ResetPasswordController
		(
			IWebHelper webHelper,
			IResetPasswordService resetService
		)
		{
			WebHelper = webHelper;
			ResetService = resetService;
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("account/reset")]
		public ActionResult Reset(string email, string hash, string redirectTo = null)
		{
			// Model
			var model = new ResetPasswordModel {Email = email, Hash = hash};

			// Form Action
			ViewBag.FormAction = "/account/reset" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			// Ajax
			if (Request.IsAjaxRequest()) return PartialView("../Accounts/Reset", model);

			// Html
			return View("../Accounts/Reset", model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[Route("account/reset")]
		public ActionResult Reset(ResetPasswordModel model, string redirectTo = null)
		{
			try
			{
				// Validation Error
				if (ModelState.IsValid)
				{
					// Reset the password
					var success = ResetService.ResetPassword(model.Email, model.Password, model.Hash, model.Otp);

					if (success)
					{
						// Json
						if (WebHelper.IsJsonRequest())
							return Json(new {Success = true}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect(IsNullOrEmpty(redirectTo) ? "/account/reset-success" : redirectTo);
					}

					// Add errors to ModelState
					ModelState.AddModelError(Empty, ResetService?.Warnings?.FirstOrDefault()?.Message ?? "There was a problem trying to reset your password.");
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				ModelState.AddModelError(Empty, ex.Message);
			}

			// Json
			if (WebHelper.IsAjaxRequest()) return Json(new { Success = false, ModelState }, JsonRequestBehavior.AllowGet);

			// Form Action
			ViewBag.FormAction = "/account/reset" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Accounts/Reset", model);

			// Html
			return View("../Accounts/Reset", model);
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("account/reset-success")]
		public ActionResult Success()
		{
			return View("../Accounts/Reset-Success");
		}
	}
}
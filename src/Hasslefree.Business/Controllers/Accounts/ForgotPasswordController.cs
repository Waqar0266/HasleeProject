using Hasslefree.Core;
using Hasslefree.Core.Logging;
using Hasslefree.Services.Accounts.Password;
using Hasslefree.Services.Configuration;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.Accounts;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.String;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class ForgotPasswordController : BaseController
	{
		/* Dependencies */
		private IWebHelper WebHelper { get; }
		private IForgotPasswordService ForgotService { get; }

		/* CTOR */
		public ForgotPasswordController
		(
			IWebHelper webHelper,
			IForgotPasswordService forgotService,
			ISettingsService settingsService)
		{
			WebHelper = webHelper;
			ForgotService = forgotService;
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("account/forgot")]
		public ActionResult Forgot(string redirectTo = null)
		{
			// Form Action
			ViewBag.FormAction = "/account/forgot" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			// Ajax
			if (Request.IsAjaxRequest()) return PartialView("../Accounts/Forgot");

			// Html
			return View("../Accounts/Forgot");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[Route("account/forgot")]
		public ActionResult Forgot(ForgotPasswordModel model, string redirectTo = null)
		{
			try
			{
				// Validation Error
				if (ModelState.IsValid)
				{
					// Perform the forgot password
					int otp;
					string hash;
					var success = ForgotService.ForgotPassword(model.Email, out otp, out hash);

					// Decide what to do next
					if (success)
					{
						// Url encode the hash
						hash = HttpUtility.UrlEncode(hash);

						// Json
						if (WebHelper.IsJsonRequest()) return Json(new { Success = true, model.Email, Hash = hash }, JsonRequestBehavior.AllowGet);

						// Ajax
						if (WebHelper.IsAjaxRequest()) return PartialView("../Accounts/Reset", new ResetPasswordModel { Email = model.Email, Hash = hash });

						// Default
						var param = IsNullOrEmpty(redirectTo) ? Empty : $"&redirectTo={redirectTo}";

						return Redirect($"/account/reset/?email={model.Email}&hash={hash}" + param);
					}

					// Add errors to ModelState
					ModelState.AddModelError(Empty,
						ForgotService?.Warnings?.FirstOrDefault()?.Message ?? "There was a problem trying to send the forgot password pin");
				}
				else
				{
					ModelState.AddModelError(String.Empty, @"We couldn't verify if you are a human.");
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
			ViewBag.FormAction = "/account/forgot" +
								 (IsNullOrEmpty(redirectTo)
									 ? Empty
									 : $"/?redirectTo={redirectTo}");

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Accounts/Forgot", model);

			// Html
			return View("../Accounts/Forgot", model);
		}

		/// <summary>
		/// The forgot password email
		/// </summary>
		/// <param name="otp"></param>
		/// <returns></returns>
		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/forgot-email")]
		public ActionResult Email(int otp)
		{
			var model = new { };
			return View("../Emails/Forgot-Password-Email", model);
		}
	}
}
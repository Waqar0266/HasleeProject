using Hasslefree.Core;
using Hasslefree.Core.Logging;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Accounts;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using static System.String;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class AccountLoginController : BaseController
	{
		#region Private Properties

		// Services
		private ILoginService LoginService { get; }
		private ISettingsService SettingsService { get; }
		private IGetPersonService GetPersonService { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public AccountLoginController
		(
			ILoginService loginService,
			ISettingsService settingsService,
			IWebHelper webHelper,
			IGetPersonService getPersonService
		)
		{
			// Services
			LoginService = loginService;
			SettingsService = settingsService;
			GetPersonService = getPersonService;
			// Other
			WebHelper = webHelper;
		}

		#endregion

		#region Actions

		[HttpGet, AllowAnonymous, Route("account/login")]
		public ActionResult Login(string redirectTo = null, string email = "")
		{
			// Validate redirectTo
			if (!ValidateRedirectTo((redirectTo))) return HttpNotFound();

			// Form Action
			ViewBag.FormAction = "/account/login" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			var model = new LoginModel()
			{
				Email = email
			};

			// Ajax
			if (Request.IsAjaxRequest()) return PartialView("../Accounts/Login");

			// Html
			return View("../Accounts/Login", model);
		}

		[HttpPost, AllowAnonymous, ValidateAntiForgeryToken, Route("account/login")]
		[SessionFilter(Order = 3)]
		public ActionResult Login(LoginModel model, string redirectTo = null)
		{
			try
			{
				// Validation Error
				if (ModelState.IsValid)
				{
					// Log into the website
					bool success = false;
					var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
					using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
					{
						if (LoginService.WithEmail(model.Email.Trim()).WithPassword(model.Password).ValidateLogin())
							// Log the person into the session
							success = LoginService
								.WithEmail(model.Email.Trim())
								.WithPassword(model.Password)
								.Remember(model.RememberMe)
								.Login();

						// Apply all changes
						scope.Complete();
					}

					// Success?
					if (success)
					{
						// Json
						if (WebHelper.IsJsonRequest()) return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

						// Ajax
						if (WebHelper.IsAjaxRequest()) return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

						// Html
						return Redirect(IsNullOrWhiteSpace(redirectTo) ? "/account/profile" : redirectTo);
					}

					// Add errors to ModelState
					ModelState.AddModelError(Empty, LoginService?.Warnings?.FirstOrDefault()?.Message ?? "There was a problem when trying to login");
				}
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Logger.LogError(ex);
				ModelState.AddModelError(Empty, ex.Message);
			}

			// Form Action
			ViewBag.FormAction = "/account/login" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			// Ajax
			if (WebHelper.IsAjaxRequest()) return Json(new { Success = false, ModelState }, JsonRequestBehavior.AllowGet);

			// Html
			return View("../Accounts/Login", model);
		}

		#endregion

		#region Private Methods

		private bool ValidateRedirectTo(string redirectTo)
		{
			if (IsNullOrEmpty((redirectTo))) return true;
			redirectTo = redirectTo.ToLower().Trim();
			if (redirectTo.StartsWith("http") || redirectTo.StartsWith("www") || redirectTo.Contains(".")) return false;
			return true;
		}

		#endregion
	}
}
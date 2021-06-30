using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Emails;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Accounts;
using Hasslefree.Web.Mvc.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;
using static System.String;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class RegisterController : BaseController
	{
		#region Private Properties

		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }
		private ISendMail SendMail { get; }
		private ISettingsService SettingsService { get; }
		private IGetPersonService GetPersonService { get; }
		private ILoginService LoginService { get; }

		private IGetPersonService GetPerson { get; }


		#endregion

		#region Constructor

		public RegisterController
		(
			IWebHelper webHelper,
			ISessionManager sessionManager,
			ISendMail sendMail,
			ISettingsService settingsService,
			IGetPersonService getPersonService,
			ILoginService loginService,
			IGetPersonService getPerson
		)
		{
			WebHelper = webHelper;
			SessionManager = sessionManager;
			SendMail = sendMail;
			SettingsService = settingsService;

			GetPersonService = getPersonService;
			LoginService = loginService;

			GetPerson = getPerson;
		}

		#endregion

		#region Actions

		[HttpGet, AllowAnonymous]
		[Route("account/register")]
		public ActionResult Register(string redirectTo = null, string type = null)
		{
			// Form Action
			ViewBag.FormAction = "/account/register" +
								 (IsNullOrEmpty(redirectTo)
									 ? Empty
									 : $"/?redirectTo={redirectTo}");

			// ViewBags
			ViewBag.Titles = Enum.GetNames(typeof(Titles));
			ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
			ViewBag.Title = "Register";
			ViewBag.AccountType = type;

			return View("../Accounts/Register");
		}

		[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
		[Route("account/register")]
		[SessionFilter(Order = 3)]
		public ActionResult Register(RegisterModel model, string redirectTo = null)
		{
			try
			{
				var person = GetPersonService[model.Email];
				if (person != null)
				{
					//Check if login credentials are valid
					if (LoginService.WithEmail(model.Email).WithPassword("").ValidateLogin())
					{
						return Redirect(String.Format("/account/login?redirectTo=&email={0}",
							model.Email));
					}
					else
					{
						ModelState.AddModelError(Empty, @"Your email has already been registered.");
						// Form Action
						ViewBag.FormAction = "/account/register" +
											 (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

						// ViewBags
						ViewBag.Titles = Enum.GetNames(typeof(Titles));
						ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
						ViewBag.Title = "Register";

						// Html
						return View("../Accounts/Register", model);
					}
				}


				// Ajax
				if (WebHelper.IsAjaxRequest())
					return Json(new
					{
						SessionId = SessionManager.Session.Reference,
						RedirectUrl = "/",
						Result = true
					});

				// Default
				return Redirect(IsNullOrWhiteSpace(redirectTo)
					? "/account/register-success"
					: redirectTo);
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Core.Logging.Logger.LogError(ex);
				ModelState.AddModelError(Empty, ex.Message);
			}

			// Ajax
			if (WebHelper.IsAjaxRequest()) return Json(new { SessionId = "", RedirectUrl = "", Result = false });

			// Form Action
			ViewBag.FormAction = "/account/register" + (IsNullOrEmpty(redirectTo) ? Empty : $"/?redirectTo={redirectTo}");

			// ViewBags
			ViewBag.Titles = Enum.GetNames(typeof(Titles));
			ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
			ViewBag.Title = "Register";

			// Html
			return View("../Accounts/Register", model);
		}

		/// <summary>
		/// The welcome email
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[HttpGet, Email, AllowAnonymous]
		[Route("account/register-email")]
		public ActionResult Email(string email)
		{
			var model = new { };
			return View("../Accounts/Register-Email", model);
		}

		/// <summary>
		/// The OTP email
		/// </summary>
		/// <param name="otp"></param>
		/// <returns></returns>
		[HttpGet, Email, AllowAnonymous]
		[Route("account/register-otp-email")]
		public ActionResult EmailOtp(int otp)
		{
			var model = new { };
			ViewBag.OneTimePassword = otp;
			return View("../Accounts/Register-Otp-Email", model);
		}

		/// <summary>
		/// The merchant notification email
		/// </summary>
		/// <returns></returns>
		[HttpGet, Email, AllowAnonymous]
		[Route("account/register-merchant-email")]
		public ActionResult EmailMerchant(int personId, int accountId)
		{
			var model = new { };
			var person = GetPersonService[personId];
			return View("../Accounts/Register-Email-Merchant", model);
		}

		[HttpGet, AllowAnonymous]
		[Route("account/register-success")]
		public ActionResult Success()
		{
			return View("../Accounts/Register-Success");
		}

		#endregion
	}
}

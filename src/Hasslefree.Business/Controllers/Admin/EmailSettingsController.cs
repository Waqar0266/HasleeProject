using Hasslefree.Core.Configuration;
using Hasslefree.Core.Logging;
using Hasslefree.Services.Configuration;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Admin
{
	[AccessControlFilter(Permission = "Admin")]
	public class EmailSettingsController : BaseController
	{
		private ISettingsService SettingsService { get; }

		public EmailSettingsController(ISettingsService settingsService)
		{
			SettingsService = settingsService;
		}

		[HttpGet, Route("email-settings")]
		public ActionResult Index(bool success = false)
		{
			var model = SettingsService.LoadSetting<EmailSettings>();

			ViewBag.Title = "Email Settings";
			ViewBag.Success = success;

			// Default
			return View("../Admin/EmailSettings", model);
		}

		[HttpPost, Route("email-settings")]
		public ActionResult Create(EmailSettings model)
		{
			try
			{
				//update the settings
				SettingsService.SaveSetting<EmailSettings>(model);

				// Default
				return Redirect("/email-settings?success=true");
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			ViewBag.Title = "Email Settings";

			// Default
			return View("../Admin/EmailSettings", model);
		}
	}
}
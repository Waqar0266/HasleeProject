using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Profiles;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.People;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	[AccessControlFilter]
	public class ProfileController : BaseController
	{
		/* Dependencies */
		private ISessionManager SessionManager { get; }
		private IUpdateProfileService UpdateService { get; }

		/* CTOR */
		public ProfileController
		(
			ISessionManager sessionManager,
			IUpdateProfileService updateService
		)
		{
			SessionManager = sessionManager;
			UpdateService = updateService;
		}

		/* GET */
		[HttpGet]
		[Route("account/profile")]
		public ActionResult Index()
		{
			// Get the model
			var model = GetModel();

			// Set select lists in ViewBag
			SetViewBag();

			// View
			return View("../Accounts/Profile", model);
		}

		/* POST */
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("account/profile")]
		public ActionResult Index(PersonModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Try to get the gender from the model
					Gender gender;
					Enum.TryParse(model.Gender, out gender);

					//Profile Info
					UpdateService
						.SetPerson(p => p.FirstName, model.FirstName)
						.SetPerson(p => p.Surname, model.Surname)
						.SetPerson(p => p.Email, model.Email)
						.SetPerson(p => p.Title, model.PersonTitle)
						.SetPerson(p => p.Gender, gender)
						.SetPerson(p => p.Mobile, model.Mobile)
						.SetPerson(p => p.Birthday, model.Birthday)
						.SetPerson(p => p.ModifiedOn, DateTime.UtcNow);

					// Update
					var success = UpdateService.Update();

					if (success)
					{
						TempData["Success"] = "You have successully updated your profile!";
						return Redirect("/account/profile");
					}

					// Not found
					ModelState.AddModelError(String.Empty, UpdateService?.Warnings?.FirstOrDefault()?.Message ?? "Profile not found");
				}
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Core.Logging.Logger.LogError(ex);
				ModelState.AddModelError(String.Empty, ex.Message);
			}

			// Set select lists in ViewBag
			SetViewBag();

			return View(model);
		}

		#region Private
		private PersonModel GetModel()
		{
			// Get person from session
			var person = SessionManager.Login.Person;

			return new PersonModel
			{
				FirstName = person.FirstName,
				Surname = person.Surname,
				Email = person.Email,
				Birthday = person.Birthday,
				Gender = person.GenderEnum,
				Mobile = person.Mobile,
				PersonTitle = person.Title
			};
		}

		private void SetViewBag()
		{
			ViewBag.Genders = Enum.GetNames(typeof(Gender));
			ViewBag.Titles = Enum.GetNames(typeof(Titles));
			ViewBag.Title = "Profile";
		}

		#endregion
	}
}
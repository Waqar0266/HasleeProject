using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Agent,Director")]
	[AgentFilter]
	public class AddRentalController : BaseController
	{
		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }

		//Helper & Managers
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }
		private ISendMail SendMail { get; }

		//Services
		private ICreateRentalService CreateRentalService { get; }

		public AddRentalController(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,

			//Helper & Managers
			IWebHelper webHelper,
			ISendMail sendMail,
			ISessionManager sessionManager,

			//Services
			ICreateRentalService createRentalService)
		{
			//Repos
			AgentRepo = agentRepo;

			//Helpers
			WebHelper = webHelper;
			SendMail = sendMail;
			SessionManager = sessionManager;

			//Services
			CreateRentalService = createRentalService;
		}

		[HttpGet, Route("account/add-rental")]
		public ActionResult Index()
		{
			ViewBag.Title = "Add Rental";

			// Normal HTML
			return View("../Accounts/Rentals/Crud");
		}

		[HttpPost, Route("account/add-rental")]
		public ActionResult Index(RentalCreate model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//new
					CreateRentalService.New(model.RentalType, model.LeaseType, model.Premises, model.StandErf, model.Address, model.Township);

					//attach agent id
					var personId = SessionManager.Login.PersonId;
					var agent = AgentRepo.Table.FirstOrDefault(a => a.PersonId == personId);
					CreateRentalService.WithAgentId(agent.AgentId);

					foreach (var landlord in model.Landlords) CreateRentalService.WithLandlord(landlord.IdNumber, landlord.Name, landlord.Surname, landlord.Email, landlord.Mobile);

					bool success = CreateRentalService.Create();

					// Success
					if (success)
					{
						//Send the emails to the landlord(s)
						foreach (var landlord in CreateRentalService.Landlords)
						{
							var email = GetTempData(landlord.Tempdata).Split(';')[2];
							SendMail.WithUrlBody($"/account/rentals/emails/landlord-initial-email?rentalId={CreateRentalService.RentalId}&landlordId={landlord.RentalLandlordId}").Send("Complete Rental Listing", email);
						}


						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect("/account/rentals");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			ViewBag.Title = "Add Rental";

			if (CreateRentalService.HasWarnings) CreateRentalService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = CreateRentalService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Accounts/Rentals/Crud", model);

			// Default
			return View("../Accounts/Rentals/Crud", model);
		}

		#region Private Methods

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		#endregion
	}
}
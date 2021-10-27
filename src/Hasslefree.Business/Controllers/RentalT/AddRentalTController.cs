using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Emails;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.RentalT
{
	[AccessControlFilter(Permission = "Agent,Director")]
	[AgentFilter]
	public class AddRentalTController : BaseController
	{
		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }

		//Helper & Managers
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }
		private ISendMail SendMail { get; }

		//Services
		private ICreateRentalTService CreateRentalTService { get; }

		public AddRentalTController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,

			//Helper & Managers
			IWebHelper webHelper,
			ISendMail sendMail,
			ISessionManager sessionManager,

			//Services
			ICreateRentalTService createRentalTService
		)
		{
			//Repos
			AgentRepo = agentRepo;

			//Helpers
			WebHelper = webHelper;
			SendMail = sendMail;
			SessionManager = sessionManager;

			//Services
			CreateRentalTService = createRentalTService;
		}

		[HttpGet, Route("account/add-tenant")]
		public ActionResult Index()
		{
			ViewBag.Title = "Add Tenant";

			// Normal HTML
			return View("../Rentals/RentalTs/Crud");
		}

		[HttpPost, Route("account/add-tenant")]
		public ActionResult Index(RentalTCreate model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//var success = CreateRentalTService.New(model.RentalId, ).Create();

					//// Success
					//if (success)
					//{
					//	var rental = GetRental[model.RentalId.Value].Get();
					//	foreach (var landlord in rental.RentalLandlords)
					//	{
					//		var email = landlord.Person.Email;
					//		SendMail.WithUrlBody($"/account/rentals/emails/existing-rental-landlord-initial-email?existingRentalId={CreateExistingRentalService.ExistingRentalId}&landlordId={landlord.RentalLandlordId}").Send("Complete Existing Rental Listing", email);
					//	}

					//	// Ajax (+ Json)
					//	if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
					//	{
					//		Success = true,
					//		AgentId = 1,
					//	}, JsonRequestBehavior.AllowGet);

					//	// Default
					//	return Redirect("/account/rentals");
					//}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			ViewBag.Title = "Add Rental";

			//if (CreateRentalService.HasWarnings) CreateRentalService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

			//// Ajax (Json)
			//if (WebHelper.IsJsonRequest()) return Json(new
			//{
			//	Success = false,
			//	Message = CreateRentalService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			//}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/Crud", model);

			// Default
			return View("../Rentals/Crud", model);
		}

		#region Private Methods

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		#endregion
	}
}
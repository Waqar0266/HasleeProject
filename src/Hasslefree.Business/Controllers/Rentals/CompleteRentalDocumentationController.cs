using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class CompleteRentalDocumentationController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IDataRepository<LandlordDocumentation> LandlordDocumentationRepo { get; }
		private IDataRepository<RentalLandlord> RentalLandlordRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }

		// Services
		private IUpdateRentalService UpdateRentalService { get; }
		private ILogoutService LogoutService { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteRentalDocumentationController
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IDataRepository<LandlordDocumentation> landlordDocumentationRepo,
			IDataRepository<RentalLandlord> rentalLandlordRepo,
			IDataRepository<Person> personRepo,

			//Services
			IUpdateRentalService updateRentalService,
			ILogoutService logoutService,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			RentalRepo = rentalRepo;
			LandlordDocumentationRepo = landlordDocumentationRepo;
			RentalLandlordRepo = rentalLandlordRepo;
			PersonRepo = personRepo;

			// Services
			UpdateRentalService = updateRentalService;
			LogoutService = logoutService;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/complete-documentation")]
		[AccessControlFilter]
		public ActionResult CompleteDocumentation(string hash)
		{
			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			string rentalUniqueId = decodedHash.Split(';')[0];

			var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == rentalUniqueId);
			if (rental.RentalStatus != RentalStatus.PendingLandlordDocumentation) return Redirect($"/account/rental/l/complete-signature?hash={hash}");

			var model = new CompleteRentalLandlordDocumentation
			{
				RentalGuid = decodedHash.Split(';')[0],
				LandlordGuid = decodedHash.Split(';')[1],
				DocumentsToUpload = GetDocumentsToUpload(rental)
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteDocumentation", model);

			// Default
			return View("../Rentals/CompleteDocumentation", model);
		}

		[HttpPost, Route("account/rental/complete-documentation")]
		[AccessControlFilter]
		public ActionResult CompleteDocumentation(CompleteRentalLandlordDocumentation model)
		{
			try
			{
				if (ModelState.IsValid)
				{

					var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.RentalGuid.ToLower());
					var rentalLandlord = RentalLandlordRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.LandlordGuid.ToLower());

					foreach (var i in model.UploadIds.Split(','))
					{
						if (Int32.TryParse(i, out int id))
						{
							if (id > 0) LandlordDocumentationRepo.Insert(new Core.Domain.Rentals.LandlordDocumentation()
							{
								RentalLandlordId = rentalLandlord.RentalLandlordId,
								DownloadId = id
							});
						}
					}

					var success = UpdateRentalService[rental.RentalId]
					.Set(a => a.RentalStatus, RentalStatus.PendingLandlordSignature)
					.Update();

					// Success
					if (success)
					{
						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.UniqueId.ToString().ToLower()};{model.LandlordGuid.ToLower()}"));

						// Default
						return Redirect($"/account/rental/l/complete-signature?hash={hash}");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			PrepViewBags();

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				//Message = CreateAgentService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rental/CompleteDocumentation", model);

			// Default
			return View("../Rental/CompleteDocumentation", model);
		}

		#endregion

		#region Private Methods

		private void PrepViewBags()
		{
			ViewBag.Title = "Complete Landlord Documentation";
		}

		private List<string> GetDocumentsToUpload(Rental rental)
		{
			if (rental.LeaseType == LeaseType.Natural) return new List<string>() { "ID - Smart card ID (both sides)", "Proof of current address to be leased", "Proof of SARS income tax number" };
			if (rental.LeaseType == LeaseType.ClosedCorporation) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Members" };
			if (rental.LeaseType == LeaseType.Company) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Directors" };
			if (rental.LeaseType == LeaseType.Trust) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Trustees" };
			return new List<string>();
		}

		#endregion
	}
}
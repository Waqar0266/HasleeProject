using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class CompleteExistingRentalController : BaseController
	{
		#region Private Properties 

		// Services
		private IGetExistingRentalService GetExistingRental { get; }
		private IUpdateExistingRentalService UpdateExistingRental { get; }
		private ILogoutService LogoutService { get; }
		private ILoginService LoginService { get; }
		private ISendMail SendMail { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteExistingRentalController
		(
			//Services
			IGetExistingRentalService getExistingRental,
			IUpdateExistingRentalService updateExistingRental,
			ILogoutService logoutService,
			ILoginService loginService,
			ISendMail sendMail,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			// Services
			GetExistingRental = getExistingRental;
			UpdateExistingRental = updateExistingRental;
			LogoutService = logoutService;
			LoginService = loginService;
			SendMail = sendMail;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/complete-existing-rental")]
		[AccessControlFilter(Permission = "Landlord")]
		public ActionResult CompleteExistingRegistration(string hash)
		{
			string uniqueId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));
			var rental = GetExistingRental[uniqueId.Split(';')[0]].Get();

			if (rental.Status != Core.Domain.Rentals.ExistingRentalStatus.PendingLandlordRegistration) return Redirect("/account/rentals");

			var model = new CompleteExistingRental
			{
				ExistingRentalId = rental.ExistingRentalId,
				Option = rental.ExistingRentalType,
				Premises = rental.Rental.Premises
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteExistingRegistration", model);

			// Default
			return View("../Rentals/CompleteExistingRegistration", model);
		}

		[HttpPost, Route("account/rental/complete-existing-rental")]
		[AccessControlFilter(Permission = "Landlord")]
		public ActionResult CompleteRegistration(CompleteExistingRental model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
					{
						var success = UpdateExistingRental[model.ExistingRentalId]
						.Set(x => x.ModifiedOn, DateTime.Now)
						.Set(x => x.ExistingRentalStatus, ExistingRentalStatus.PendingLandlordWitnessSignature)
						.Set(x => x.AmendedAddendum, model.AmendedAddendum)
						.Set(x => x.EndDate, model.EndDate)
						.Set(x => x.MaterialChanges, model.FurtherMaterialChanges)
						.Set(x => x.ParkingBays, model.ParkingBays)
						.Set(x => x.RenewalCommencementDate, model.RenewalCommencementDate)
						.Set(x => x.RenewalPeriod, model.RenewalPeriod)
						.Set(x => x.RenewalTerminationDate, model.RenewalTerminationDate)
						.Set(x => x.StartDate, model.StartDate)
						.Set(x => x.TerminationDate, model.TerminationDate)
						.Set(x => x.LandlordWitness1Email, model.Witness1Email)
						.Set(x => x.LandlordWitness1Name, model.Witness1Name)
						.Set(x => x.LandlordWitness1Surname, model.Witness1Surname)
						.Set(x => x.LandlordWitness2Email, model.Witness2Email)
						.Set(x => x.LandlordWitness2Name, model.Witness2Name)
						.Set(x => x.LandlordWitness2Surname, model.Witness2Surname)
						.Update();

						// Success
						if (success)
						{
							//Send the landlord witness emails
							SendLandlordWitnessEmail(model.Witness1Email, 1, model.ExistingRentalId);
							SendLandlordWitnessEmail(model.Witness2Email, 2, model.ExistingRentalId);

							//complete the scope
							transactionScope.Complete();

							// Ajax (+ Json)
							if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
							{
								Success = true,
								AgentId = 1,
							}, JsonRequestBehavior.AllowGet);

							// Default
							return Redirect($"/account/rentals");
						}

					}
				}
			}
			catch (DbEntityValidationException ev)
			{
				foreach (var e in ev.EntityValidationErrors)
				{
					foreach (var error in e.ValidationErrors) ModelState.AddModelError("", error.ErrorMessage);
				}

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			PrepViewBags();

			var errors = "";

			ModelState.AddModelError("", errors);

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteExistingRegistration", model);

			// Default
			return View("../Rentals/CompleteExistingRegistration", model);
		}

		#endregion

		#region Private Methods

		private void PrepViewBags()
		{
			ViewBag.Title = "Complete Existing Rental";
		}

		private bool SendLandlordWitnessEmail(string email, int witnessNumber, int existingRentalId)
		{
			var url = $"account/existing-rental/emails/landlord-witness-email?witnessNumber={witnessNumber}&existingRentalId={existingRentalId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("Existing Listing - Landlord Witness Signature");
		}

		#endregion
	}
}
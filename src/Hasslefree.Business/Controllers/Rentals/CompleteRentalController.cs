using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Landlords.Crud;
using Hasslefree.Services.People.Interfaces;
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
	public class CompleteRentalController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }
		private IReadOnlyRepository<RentalMandate> RentalMandateRepo { get; }

		// Services
		private IUpdateRentalService UpdateRentalService { get; }
		private IUpdateRentalLandlordService UpdateRentalLandlordService { get; }
		private IUpdateRentalMandateService UpdateRentalMandateService { get; }
		private ICreatePersonService CreatePerson { get; }
		private ILogoutService LogoutService { get; }
		private ILoginService LoginService { get; }
		private ICountryQueryService Countries { get; }
		private ICreateLandlordBankAccountService CreateLandlordBankAccountService { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteRentalController
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,
			IReadOnlyRepository<RentalMandate> rentalMandateRepo,

			//Services
			IUpdateRentalService updateRentalService,
			IUpdateRentalMandateService updateRentalMandateService,
			ICreatePersonService createPerson,
			ILogoutService logoutService,
			ICountryQueryService countries,
			IUpdateRentalLandlordService updateRentalLandlordService,
			ICreateLandlordBankAccountService createLandlordBankAccountService,
			ILoginService loginService,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			RentalRepo = rentalRepo;
			RentalLandlordRepo = rentalLandlordRepo;
			RentalMandateRepo = rentalMandateRepo;

			// Services
			UpdateRentalService = updateRentalService;
			CreatePerson = createPerson;
			LogoutService = logoutService;
			Countries = countries;
			UpdateRentalLandlordService = updateRentalLandlordService;
			UpdateRentalMandateService = updateRentalMandateService;
			CreateLandlordBankAccountService = createLandlordBankAccountService;
			LoginService = loginService;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/complete-rental")]
		public ActionResult CompleteRegistration(string hash)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/complete-rental?hash={hash}");
			}

			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == decodedHash.Split(';')[0]);
			var landlord = RentalLandlordRepo.Table.FirstOrDefault(r => r.UniqueId.ToString().ToLower() == decodedHash.Split(';')[1]);

			if (rental.RentalStatus != RentalStatus.PendingNew) return Redirect($"/account/rental/complete-documentation?hash={hash}");

			var model = new CompleteRental
			{
				RentalGuid = decodedHash.Split(';')[0],
				RentalId = rental.RentalId,
				RentalLandlordId = decodedHash.Split(';')[1],
				Name = GetTempData(landlord.Tempdata).Split(';')[0],
				Surname = GetTempData(landlord.Tempdata).Split(';')[1],
				Email = GetTempData(landlord.Tempdata).Split(';')[2],
				Mobile = GetTempData(landlord.Tempdata).Split(';')[3],
				IdNumber = landlord.IdNumber,
				Address = rental.Address,
				Premises = rental.Premises,
				StandErf = rental.StandErf,
				Township = rental.Township
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteRegistration", model);

			// Default
			return View("../Rentals/CompleteRegistration", model);
		}

		[HttpPost, Route("account/rental/complete-rental")]
		[SessionFilter(Order = 3)]
		public ActionResult CompleteRegistration(CompleteRental model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.RentalGuid.ToLower());
					var landlord = RentalLandlordRepo.Table.FirstOrDefault(r => r.UniqueId.ToString().ToLower() == model.RentalLandlordId.ToLower());

					//create the person (landlord)
					CreatePerson
					.New(model.Name, "", model.Surname, model.Email, model.Title.ResolveTitle(), null, model.Gender, CalculateDateOfBirth(model.IdNumber))
					.WithContactDetails(null, null, model.Mobile)
					.WithPassword(model.Password, "")
					.WithSecurityGroup("Landlord")
					.Create();

					var personId = CreatePerson.PersonId;

					var success = UpdateRentalService[model.RentalId]
					.Set(a => a.Address, model.Address)
					.Set(a => a.Deposit, model.Deposit)
					.Set(a => a.DepositPaymentDate, model.DepositPaymentDate)
					.Set(a => a.ModifiedOn, DateTime.Now)
					.Set(a => a.MonthlyPaymentDate, model.RentalPaymentDate)
					.Set(a => a.MonthlyRental, model.MonthlyRental)
					.Set(a => a.Premises, model.Premises)
					.Set(a => a.StandErf, model.StandErf)
					.Set(a => a.Township, model.Township)
					.Set(a => a.RentalStatus, RentalStatus.PendingLandlordDocumentation)
					.Update();

					success = UpdateRentalLandlordService[landlord.RentalLandlordId]
					.Set(x => x.PersonId, personId)
					.Set(x => x.VatNumber, model.VatNumber)
					.Set(x => x.IncomeTaxNumber, model.IncomeTaxNumber)
					.Update();

					var rentalMandateId = 0;
					if (RentalMandateRepo.Table.Any(r => r.RentalId == rental.RentalId)) rentalMandateId = RentalMandateRepo.Table.FirstOrDefault(r => r.RentalId == rental.RentalId).RentalMandateId;

					success = UpdateRentalMandateService.WithRentalId(rental.RentalId)[rentalMandateId]
					.Set(x => x.Procurement1Percentage, model.Procurement1Percentage)
					.Set(x => x.Procurement1Amount, model.Procurement1Amount)
					.Set(x => x.Procurement2Percentage, model.Procurement2Percentage)
					.Set(x => x.Procurement2Amount, model.Procurement2Amount)
					.Set(x => x.Procurement3Percentage, model.Procurement3Percentage)
					.Set(x => x.Procurement3Amount, model.Procurement3Amount)
					.Set(x => x.ManagementAmount, model.ManagementAmount)
					.Set(x => x.ManagementPercentage, model.ManagementPercentage)
					.Set(x => x.SaleAmount, model.SaleAmount)
					.Set(x => x.SalePercentage, model.SalePercentage)
					.Update();

					//create the landlord bank account
					success = CreateLandlordBankAccountService.WithRentalId(rental.RentalId)
					.New(model.AccountHolder, model.Bank, model.Branch, model.BranchCode, model.AccountNumber, model.BankReference)
					.Create();

					// Success
					if (success)
					{
						//Auto login the new landlord
						LoginService.WithGuid(CreatePerson.PersonGuid).Login();

						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect($"/account/rental/complete-documentation?id={model.RentalGuid}&lid={model.RentalLandlordId}");
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

			var errors = "";

			if (UpdateRentalService.HasWarnings) UpdateRentalService.Warnings.ForEach(w => errors += w.Message + "\n");

			ModelState.AddModelError("", errors);

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteRegistration", model);

			// Default
			return View("../Rentals/CompleteRegistration", model);
		}

		#endregion

		#region Private Methods

		private void PrepViewBags()
		{
			ViewBag.Title = "Complete Rental";

			ViewBag.Titles = new List<string> { "Mr", "Miss", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
			ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
		}

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		private DateTime CalculateDateOfBirth(string idNumber)
		{
			string id = idNumber.Substring(0, 6);
			string y = id.Substring(0, 2);
			string year = $"20{y}";
			if (Int32.Parse(id.Substring(0, 1)) > 2) year = $"19{y}";

			int month = Int32.Parse(id.Substring(2, 2));
			int day = Int32.Parse(id.Substring(4, 2));

			return new DateTime(Int32.Parse(year), month, day);
		}

		#endregion
	}
}
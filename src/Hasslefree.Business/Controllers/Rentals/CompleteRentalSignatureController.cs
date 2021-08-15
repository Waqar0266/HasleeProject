using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.RentalForms;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class CompleteRentalSignatureController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<LandlordDocumentation> LandlordDocumentationRepo { get; }
		private IReadOnlyRepository<RentalForm> RentalFormRepo { get; }
		private IReadOnlyRepository<RentalWitness> RentalWitnessRepo { get; }
		private IReadOnlyRepository<Download> DownloadRepo { get; }
		private IReadOnlyRepository<AgentAddress> AgentAddressRepo { get; }

		// Services
		private IUpdateRentalService UpdateRentalService { get; }
		private IUpdateRentalLandlordService UpdateRentalLandlordService { get; }
		private IUploadPictureService UploadPicture { get; }
		private IUploadDownloadService UploadDownload { get; }
		private IFillFormService FillForm { get; }
		private IGetFirmService GetFirmService { get; }
		private ICreateRentalFormService CreateRentalForm { get; }
		private ILogoutService LogoutService { get; }
		private ISendMail SendMail { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteRentalSignatureController
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<LandlordDocumentation> landlordDocumentationRepo,
			IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,
			IReadOnlyRepository<RentalWitness> rentalWitnessRepo,
			IReadOnlyRepository<RentalForm> rentalFormRepo,
			IReadOnlyRepository<Download> downloadRepo,
			IReadOnlyRepository<AgentAddress> agentAddressRepo,

			//Services
			IUpdateRentalService updateRentalService,
			IUploadPictureService uploadPicture,
			IUploadDownloadService uploadDownload,
			IFillFormService fillForm,
			IGetFirmService getFirmService,
			ICreateRentalFormService createRentalForm,
			ILogoutService logoutService,
			ISendMail sendMail,
			IUpdateRentalLandlordService updateRentalLandlordService,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			RentalRepo = rentalRepo;
			PersonRepo = personRepo;
			LandlordDocumentationRepo = landlordDocumentationRepo;
			RentalLandlordRepo = rentalLandlordRepo;
			RentalFormRepo = rentalFormRepo;
			DownloadRepo = downloadRepo;
			AgentAddressRepo = agentAddressRepo;
			RentalWitnessRepo = rentalWitnessRepo;

			// Services
			UpdateRentalService = updateRentalService;
			UploadPicture = uploadPicture;
			UploadDownload = uploadDownload;
			FillForm = fillForm;
			GetFirmService = getFirmService;
			CreateRentalForm = createRentalForm;
			LogoutService = logoutService;
			SendMail = sendMail;
			UpdateRentalLandlordService = updateRentalLandlordService;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/complete-signature")]
		public ActionResult CompleteSignature(string id, string lid)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/complete-signature?id={id}");
			}

			var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == id.ToLower());
			var landlord = RentalLandlordRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == lid.ToLower());

			var model = new CompleteRentalSignature
			{
				RentalGuid = id,
				LandlordGuid = lid
			};

			ViewBag.Title = "Complete Landlord Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteSignature", model);

			// Default
			return View("../Rentals/CompleteSignature", model);
		}

		[HttpGet, Route("account/rental/complete-witness-signature")]
		public ActionResult CompleteSignature(string hash)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/complete-witness-signature?hash={hash}");
			}

			var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

			var uniqueId = decodedHash.Split(';')[0];
			var witnessNumber = Int32.Parse(decodedHash.Split(';')[1]);
			var witnessFor = decodedHash.Split(';')[2];
			var rentalId = Int32.Parse(decodedHash.Split(';')[3]);

			var rentalWitness = RentalWitnessRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == uniqueId.ToLower());

			var model = new CompleteRentalWitnessSignature
			{
				UniqueId = uniqueId,
				WitnessNumber = witnessNumber,
				RentalId = rentalId
			};

			ViewBag.Title = "Complete Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteSignature", model);

			// Default
			return View("../Rentals/CompleteWitnessSignature", model);
		}

		[HttpPost, Route("account/rental/complete-signature")]
		public ActionResult CompleteSignature(CompleteRentalSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.LandlordGuid.ToLower());
					var landlord = RentalLandlordRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.RentalGuid.ToLower());

					var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == landlord.PersonId);

					//add the signatures
					UploadPicture.WithPath("signatures");

					var signatureData = RemoveWhitespace(model.Signature);

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = signatureData,
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.png",
						Name = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.png",
						MimeType = "image/png",
						AlternateText = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.jpg"
					});

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = RemoveWhitespace(model.Initials),
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial.png",
						Name = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial.png",
						MimeType = "image/png",
						AlternateText = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial"
					});

					var pictures = UploadPicture.Save();

					var success = UpdateRentalService[rental.RentalId]
					.Set(a => a.RentalStatus, RentalStatus.PendingWitnessSignature)
					.Update();

					success = UpdateRentalLandlordService[landlord.RentalLandlordId]
					.Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature"))
					.Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial"))
					.Update();

					var firmSettings = GetFirmService.Get();

					//get the addresses
					var agentAddresses = AgentAddressRepo.Table.Where(a => a.AgentId == rental.AgentId).Select(a => a.Address).ToList();
					var postal = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Postal);
					var residential = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Residential);

					if (rental.LeaseType == LeaseType.Natural)



						UploadDownload.WithPath("forms");

					var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} EAAB Registration Form_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_eaab_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentRegistrationFormData,
					//	Size = agentRegistrationFormData.Length
					//});

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} Agent Contract_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_agent_contract_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentContractData,
					//	Size = agentContractData.Length
					//});

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} Appointment Letter_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_appointment_letter_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentAppointmentLetterData,
					//	Size = agentAppointmentLetterData.Length
					//});

					//var downloads = UploadDownload.Save();

					//success = CreateAgentForm.New(FormName.Eaab, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} EAAB Registration Form_{dateStamp}.pdf").DownloadId).Create();
					//success = CreateAgentForm.New(FormName.AgentContract, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Agent Contract_{dateStamp}.pdf").DownloadId).Create();
					//success = CreateAgentForm.New(FormName.AppointmentLetter, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Appointment Letter_{dateStamp}.pdf").DownloadId).Create();

					////Send the email to the director
					//success = SendDirectorEmail(agent.AgentId);

					// Success
					if (success)
					{
						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect($"/account/agent/complete-eaab?id=");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			var errors = "";

			//if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
			//if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

			ModelState.AddModelError("", errors);

			ViewBag.Title = "Complete Agent Signature";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

			// Default
			return View("../Agents/CompleteSignature", model);
		}

		[HttpPost, Route("account/rental/complete-witness-signature")]
		public ActionResult CompleteWitnessSignature(CompleteRentalWitnessSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rentalWitness = RentalWitnessRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.UniqueId.ToLower());

					//add the signatures
					UploadPicture.WithPath("signatures");

					var signatureData = RemoveWhitespace(model.Signature);

					//UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	File = signatureData,
					//	Format = Core.Domain.Media.PictureFormat.Png,
					//	Key = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.png",
					//	Name = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.png",
					//	MimeType = "image/png",
					//	AlternateText = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature.jpg"
					//});

					//UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	File = RemoveWhitespace(model.Initials),
					//	Format = Core.Domain.Media.PictureFormat.Png,
					//	Key = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial.png",
					//	Name = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial.png",
					//	MimeType = "image/png",
					//	AlternateText = $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial"
					//});

					var pictures = UploadPicture.Save();

					//var success = UpdateRentalService[rental.RentalId]
					//.Set(a => a.RentalStatus, RentalStatus.PendingWitnessSignature)
					//.Update();

					//success = UpdateRentalLandlordService[landlord.RentalLandlordId]
					//.Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_signature"))
					//.Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{person.FirstName.ToLower().Replace(" ", "-")}_{person.Surname.ToLower().Replace(" ", "-")}_initial"))
					//.Update();

					//var firmSettings = GetFirmService.Get();

					////get the addresses
					//var agentAddresses = AgentAddressRepo.Table.Where(a => a.AgentId == rental.AgentId).Select(a => a.Address).ToList();
					//var postal = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Postal);
					//var residential = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Residential);

					//if (rental.LeaseType == LeaseType.Natural)



					//	UploadDownload.WithPath("forms");

					//var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} EAAB Registration Form_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_eaab_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentRegistrationFormData,
					//	Size = agentRegistrationFormData.Length
					//});

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} Agent Contract_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_agent_contract_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentContractData,
					//	Size = agentContractData.Length
					//});

					//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					//{
					//	Action = Web.Models.Common.CrudAction.Create,
					//	ContentType = "application/pdf",
					//	DownloadType = Core.Domain.Media.DownloadType.Document,
					//	Extension = "pdf",
					//	FileName = $"{model.Name} {model.Surname} Appointment Letter_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
					//	Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_appointment_letter_{dateStamp}.pdf",
					//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					//	Data = agentAppointmentLetterData,
					//	Size = agentAppointmentLetterData.Length
					//});

					//var downloads = UploadDownload.Save();

					//success = CreateAgentForm.New(FormName.Eaab, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} EAAB Registration Form_{dateStamp}.pdf").DownloadId).Create();
					//success = CreateAgentForm.New(FormName.AgentContract, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Agent Contract_{dateStamp}.pdf").DownloadId).Create();
					//success = CreateAgentForm.New(FormName.AppointmentLetter, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Appointment Letter_{dateStamp}.pdf").DownloadId).Create();

					////Send the email to the director
					//success = SendDirectorEmail(agent.AgentId);

					// Success
					if (true)
					{
						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect($"/account/rental/complete-eaab?id=");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			var errors = "";

			//if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
			//if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

			ModelState.AddModelError("", errors);

			ViewBag.Title = "Complete Agent Signature";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

			// Default
			return View("../Agents/CompleteSignature", model);
		}

		#endregion

		#region Private Methods

		private void FillNaturalForms(Rental rental, RentalMandate rentalMandate, List<LandlordBankAccount> landlordBankAccounts, Address landlordPhysicalAddress, Address landlordPostalAddress, Address agentPhysicalAddress, Address agentPostalAddress, Person agentPerson, Agent agent, List<RentalLandlord> landlords)
		{

			var agentContractData = FillForm.Prepare("Mandate Agreement.pdf")
						.WithField("TheAgent", $"{agentPerson.FirstName.ToUpper()} {agentPerson.Surname.ToUpper()}")
						.WithField("AgentIdNumber", $"{agent.IdNumber}")
						.WithField("AgentVATNumber", $"")
						.WithField("FFCNumber", $"{agent.FfcNumber}")
						.WithField("TheLandlord", $"{String.Join(" / ", landlords.Select(a => a.Person.FirstName + " " + a.Person.Surname))}")
						.WithField("LandlordIdNumber", landlords.FirstOrDefault().IdNumber)
						.WithField("LandlordVatNumber", landlords.FirstOrDefault().VatNumber)
						.WithField("LandlordIncomeTaxNumber", landlords.FirstOrDefault().IncomeTaxNumber)
						.WithField("ThePremises", rental.Premises)
						.WithField("StandErf", rental.StandErf)
						.WithField("Township", rental.Township)
						.WithField("Address", rental.Address)
						.WithField("MonthlyRental", rental.MonthlyRental.Value.ToString("F"))
						.WithField("Deposit", rental.Deposit.Value.ToString("F"))
						.WithField("RentalPaymentDate", rental.MonthlyPaymentDate.Value.ToString("yyyy-MM-dd"))
						.WithField("DepositPaymentDate", rental.DepositPaymentDate.Value.ToString("yyyy-MM-dd"))
						.WithField("FirstProcurementPercentage", rentalMandate.Procurement1Percentage.Value.ToString())
						.WithField("FirstProcurementAmount", rentalMandate.Procurement1Amount.Value.ToString("F"))
						.WithField("SecondProcurementPercentage", rentalMandate.Procurement2Percentage.Value.ToString())
						.WithField("SecondProcurementAmount", rentalMandate.Procurement2Amount.Value.ToString("F"))
						.WithField("ThirdProcurementPercentage", rentalMandate.Procurement3Percentage.Value.ToString())
						.WithField("ThirdProcurementAmount", rentalMandate.Procurement3Amount.Value.ToString("F"))
						.WithField("ManagementCommissionPercentage", rentalMandate.ManagementPercentage.Value.ToString())
						.WithField("ManagementCommissionAmount", rentalMandate.ManagementAmount.Value.ToString("F"))
						.WithField("SaleCommissionPercentage", rentalMandate.SalePercentage.Value.ToString())
						.WithField("SaleCommissionAmount", rentalMandate.SaleAmount.Value.ToString("F"))
						.WithField("AccountHolder", landlordBankAccounts.FirstOrDefault().AccountHolder)
						.WithField("Bank", landlordBankAccounts.FirstOrDefault().Bank)
						.WithField("Branch", landlordBankAccounts.FirstOrDefault().Branch)
						.WithField("BranchCode", landlordBankAccounts.FirstOrDefault().BranchCode)
						.WithField("AccountNumber", landlordBankAccounts.FirstOrDefault().AccountNumber)
						.WithField("BankReference", landlordBankAccounts.FirstOrDefault().BankReference)
						.WithField("LandlordPhysicalAddress", landlordPhysicalAddress.Address1)
						.WithField("LandlordPostalAddress", landlordPostalAddress.Address1)
						.WithField("LandlordEmail", landlords.FirstOrDefault().Person.Email)
						.WithField("LandlordPhoneNumber", landlords.FirstOrDefault().Person.Mobile)
						.WithField("AgentPhysicalAddress", agentPhysicalAddress.Address1)
						.WithField("AgentPostalAddress", agentPostalAddress.Address1)
						.WithField("AgentEmail", agentPerson.Email)
						.WithField("AgentPhoneNumber", agentPerson.Mobile)
						.WithCheckbox("DirectMarketingYes", rental.Marketing)
						.WithCheckbox("DirectMarketingNo", !rental.Marketing)
						.WithCheckbox("FindingTenant", rental.Procurement)
						.WithCheckbox("Management", rental.Management)
						.WithField("SpecificRequirements", rental.SpecificRequirements);


			//Save the form
			var mandateForm = FillForm
								.Process();
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

		private static byte[] RemoveWhitespace(string base64)
		{
			Bitmap bmp;
			byte[] imageAsBytes = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));
			using (var ms = new MemoryStream(imageAsBytes))
			{
				var image = Bitmap.FromStream(ms);
				bmp = new Bitmap(image);
			}

			int w = bmp.Width;
			int h = bmp.Height;

			Func<int, bool> allWhiteRow = row =>
			{
				for (int i = 0; i < w; ++i)
				{
					if (bmp.GetPixel(i, row).R != 255)
						return false;
				}
				return true;
			};

			Func<int, bool> allWhiteColumn = col =>
			{
				for (int i = 0; i < h; ++i)
				{
					if (bmp.GetPixel(col, i).R != 255)
						return false;
				}
				return true;
			};

			int topmost = 0;
			for (int row = 0; row < h; ++row)
			{
				if (allWhiteRow(row))
					topmost = row;
				else break;
			}

			int bottommost = 0;
			for (int row = h - 1; row >= 0; --row)
			{
				if (allWhiteRow(row))
					bottommost = row;
				else break;
			}

			int leftmost = 0, rightmost = 0;
			for (int col = 0; col < w; ++col)
			{
				if (allWhiteColumn(col))
					leftmost = col;
				else
					break;
			}

			for (int col = w - 1; col >= 0; --col)
			{
				if (allWhiteColumn(col))
					rightmost = col;
				else
					break;
			}

			if (rightmost == 0) rightmost = w; // As reached left
			if (bottommost == 0) bottommost = h; // As reached top.

			int croppedWidth = rightmost - leftmost;
			int croppedHeight = bottommost - topmost;

			if (croppedWidth == 0) // No border on left or right
			{
				leftmost = 0;
				croppedWidth = w;
			}

			if (croppedHeight == 0) // No border on top or bottom
			{
				topmost = 0;
				croppedHeight = h;
			}

			try
			{
				var target = new Bitmap(croppedWidth, croppedHeight);
				using (Graphics g = Graphics.FromImage(target))
				{
					g.DrawImage(bmp,
					  new RectangleF(0, 0, croppedWidth, croppedHeight),
					  new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
					  GraphicsUnit.Pixel);
				}

				target.MakeTransparent();

				return target.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
			}
			catch (Exception ex)
			{
				throw new Exception(
				  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
				  ex);
			}
		}

		//private bool SendDirectorEmail(int agentId)
		//{
		//	var url = $"account/agent/emails/director-email?agentId={agentId}";

		//	var agentForms = AgentFormRepo.Table.Where(a => a.AgentId == agentId).Select(a => a.DownloadId).ToList();

		//	var ids = agentForms;

		//	var downloads = DownloadRepo.Table.Where(a => ids.Contains(a.DownloadId)).ToList();

		//	var attachments = new List<Attachment>();

		//	SendMail.WithUrlBody(url).WithRecipient("director@hasslefree.sa.com");

		//	foreach (var download in downloads)
		//	{
		//		var data = new WebClient().DownloadData(download.RelativeFolderPath);
		//		SendMail.WithAttachment(new Attachment(new MemoryStream(data), download.FileName, download.ContentType));
		//	}

		//	return SendMail.Send("Agent Profile Review");
		//}

		#endregion
	}
}
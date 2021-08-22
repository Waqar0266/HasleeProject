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
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Framework.Filters;
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
	public class CompleteRentalAgentSignatureController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<Agent> AgentRepo { get; }
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
		private IUpdateRentalWitnessService UpdateRentalWitnessService { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteRentalAgentSignatureController
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<Agent> agentRepo,
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
			IUpdateRentalWitnessService updateRentalWitnessService,

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
			AgentRepo = agentRepo;

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
			UpdateRentalWitnessService = updateRentalWitnessService;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/a/complete")]
		[AccessControlFilter]
		public ActionResult CompleteAgent(string hash)
		{
			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			string rentalUniqueId = decodedHash.Split(';')[0];
			string landlordUniqueId = decodedHash.Split(';')[1];

			var model = new CompleteRentalAgent
			{
				RentalGuid = decodedHash.Split(';')[0],
				AgentGuid = decodedHash.Split(';')[1]
			};

			ViewBag.Title = "Complete Rental - Agent";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteAgent", model);

			// Default
			return View("../Rentals/CompleteAgent", model);
		}

		[HttpPost, Route("account/rental/a/complete")]
		public ActionResult CompleteAgent(CompleteRentalAgent model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.RentalGuid.ToLower());
					var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == model.AgentGuid.ToLower());
					var agentPerson = PersonRepo.Table.FirstOrDefault(p => p.PersonId == agent.PersonId);

					//add the witnesses to the database
					int rentalWitnessId = 0;
					if (RentalWitnessRepo.Table.Any(r => r.RentalId == rental.RentalId)) rentalWitnessId = RentalWitnessRepo.Table.FirstOrDefault(r => r.RentalId == rental.RentalId).RentalWitnessId;
					var success = UpdateRentalWitnessService.WithRentalId(rental.RentalId)[rentalWitnessId]
					.Set(r => r.AgentWitness1Email, model.Witness1Email)
					.Set(r => r.AgentWitness1Name, model.Witness1Name)
					.Set(r => r.AgentWitness1Surname, model.Witness1Surname)
					.Set(r => r.AgentWitness1Mobile, model.Witness1Mobile)
					.Set(r => r.AgentWitness2Email, model.Witness2Email)
					.Set(r => r.AgentWitness2Name, model.Witness2Name)
					.Set(r => r.AgentWitness2Surname, model.Witness2Surname)
					.Set(r => r.AgentWitness2Mobile, model.Witness2Mobile)
					.Update();

					SendAgentWitnessEmail(model.Witness1Email, rentalWitnessId, rental.RentalId, 1);
					SendAgentWitnessEmail(model.Witness2Email, rentalWitnessId, rental.RentalId, 2);

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
						return Redirect($"/account/rentals");
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

			ViewBag.Title = "Complete Rental - Agent";

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

		[HttpPost, Route("account/rental/a/complete-witness-signature")]
		public ActionResult CompleteWitnessSignature(CompleteRentalWitnessLandlordSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rentalWitness = RentalWitnessRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.UniqueId.ToLower());
					var rental = RentalRepo.Table.FirstOrDefault(a => a.RentalId == model.RentalId);
					var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == rental.AgentId);
					var agentPerson = PersonRepo.Table.FirstOrDefault(a => a.PersonId == agent.PersonId);

					//add the signatures
					UploadPicture.WithPath($"signatures/rental/{rentalWitness.RentalId}");

					var signatureData = RemoveWhitespace(model.Signature);

					string name = model.WitnessNumber == 1 ? rentalWitness.LandlordWitness1Name : rentalWitness.LandlordWitness2Name;
					string surname = model.WitnessNumber == 2 ? rentalWitness.LandlordWitness1Surname : rentalWitness.LandlordWitness2Surname;

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = signatureData,
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
						Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png",
						MimeType = "image/png",
						AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.jpg"
					});

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = RemoveWhitespace(model.Initials),
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png",
						Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png",
						MimeType = "image/png",
						AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial"
					});

					var pictures = UploadPicture.Save();
					bool success = false;

					if (model.WitnessNumber == 1)
					{
						success = UpdateRentalWitnessService.WithRentalId(rentalWitness.RentalId)[rentalWitness.RentalWitnessId]
						.Set(a => a.LandlordWitness1Id, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
						.Update();
					}

					if (model.WitnessNumber == 2)
					{
						success = UpdateRentalWitnessService.WithRentalId(rentalWitness.RentalId)[rentalWitness.RentalWitnessId]
						.Set(a => a.LandlordWitness2Id, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
						.Update();
					}

					int rentalWitnessId = UpdateRentalWitnessService.RentalWitnessId;



					// Success
					if (success)
					{
						//verify if landlord witnesses signed
						rentalWitness = RentalWitnessRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.UniqueId.ToLower());
						if (rentalWitness.AgentWitness1Id.HasValue && rentalWitness.AgentWitness2Id.HasValue)
						{
							//fill all the forms
						}

						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect($"/account/rental/complete-witness-signature/success");
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
			var mandateAgreementData = FillForm.Prepare("Mandate Agreement - Natural.pdf")
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

		private bool SendAgentWitnessEmail(string email, int rentalWitnessId, int rentalId, int witnessNumber)
		{
			var url = $"account/rental/emails/agent-witness-email?witnessNumber={witnessNumber}&rentalId={rentalId}&witnessId={rentalWitnessId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("New Listing - Agent Witness Signature");
		}

		#endregion
	}
}
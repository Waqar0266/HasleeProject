using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.RentalForms;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class CompleteExistingRentalAgentSignatureController : BaseController
	{
		#region Private Properties 

		// Services
		private IUpdateExistingRentalService UpdateExistingRentalService { get; }
		private IUploadPictureService UploadPicture { get; }
		private IUploadDownloadService UploadDownload { get; }
		private IFillFormService FillForm { get; }
		private ICreateExistingRentalFormService CreateExistingRentalForm { get; }
		private ILogoutService LogoutService { get; }
		private ISendMail SendMail { get; }
		private IGetExistingRentalService GetExistingRental { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteExistingRentalAgentSignatureController
		(
			//Services
			IUpdateExistingRentalService updateExistingRentalService,
			IUploadPictureService uploadPicture,
			IUploadDownloadService uploadDownload,
			IFillFormService fillForm,
			ICreateExistingRentalFormService createExistingRentalForm,
			ILogoutService logoutService,
			ISendMail sendMail,
			IGetExistingRentalService getExistingRental,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			// Services
			UpdateExistingRentalService = updateExistingRentalService;
			UploadPicture = uploadPicture;
			UploadDownload = uploadDownload;
			FillForm = fillForm;
			CreateExistingRentalForm = createExistingRentalForm;
			LogoutService = logoutService;
			SendMail = sendMail;
			GetExistingRental = getExistingRental;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/existing-rental/a/complete")]
		[AccessControlFilter(Permission = "Agent")]
		public ActionResult CompleteAgent(string hash)
		{
			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			string rentalUniqueId = decodedHash.Split(';')[0];

			var existingRental = GetExistingRental[rentalUniqueId].Get();

			if (existingRental.Status != ExistingRentalStatus.PendingAgentSignature) return Redirect("/account/rentals");

			var model = new CompleteExistingRentalAgent
			{
				ExistingRentalId = existingRental.ExistingRentalId
			};

			ViewBag.Title = "Complete Existing Rental - Agent";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteAgent", model);

			// Default
			return View("../ExistingRentals/CompleteAgent", model);
		}

		[HttpGet, Route("account/existing-rental/a/complete-witness-signature/success")]
		public ActionResult CompleteLandlordWitnessSignatureSuccess()
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/existing-rental/a/complete-witness-signature/success");
			}

			ViewBag.Title = "Completed Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteWitnessSignatureSuccess");

			// Default
			return View("../ExistingRentals/CompleteWitnessSignatureSuccess");
		}

		[HttpGet, Route("account/existing-rental/a/complete-witness-signature")]
		public ActionResult CompleteAgentWitnessSignature(string hash)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/a/complete-witness-signature?hash={hash}");
			}

			var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

			var rentalUniqueId = decodedHash.Split(';')[0];
			var witnessNumber = Int32.Parse(decodedHash.Split(';')[1]);

			var existingRental = GetExistingRental[rentalUniqueId].Get();
			if (witnessNumber == 1 && existingRental.AgentWitness1SignatureId.HasValue) return Redirect("/account/existing-rental/a/complete-witness-signature/success");
			if (witnessNumber == 2 && existingRental.AgentWitness2SignatureId.HasValue) return Redirect("/account/existing-rental/a/complete-witness-signature/success");

			var model = new CompleteExistingRentalWitnessAgentSignature
			{
				ExistingRentalId = existingRental.ExistingRentalId,
				WitnessNumber = witnessNumber
			};

			ViewBag.Title = "Complete Agent Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteAgentWitnessSignature", model);

			// Default
			return View("../ExistingRentals/CompleteAgentWitnessSignature", model);
		}

		[HttpPost, Route("account/existing-rental/a/complete")]
		public ActionResult CompleteAgent(CompleteExistingRentalAgent model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var existingRental = GetExistingRental[model.ExistingRentalId].Get();

					var success = UpdateExistingRentalService[existingRental.ExistingRentalId]
					.Set(a => a.AgentWitness1Email, model.Witness1Email)
					.Set(a => a.AgentWitness1Name, model.Witness1Name)
					.Set(a => a.AgentWitness1Surname, model.Witness1Surname)
					.Set(a => a.ModifiedOn, DateTime.Now)
					.Set(a => a.AgentWitness2Email, model.Witness2Email)
					.Set(a => a.AgentWitness2Name, model.Witness2Name)
					.Set(a => a.AgentWitness2Surname, model.Witness2Surname)
					.Set(a => a.ExistingRentalStatus, ExistingRentalStatus.PendingAgentWitnessSignature)
					.Update();

					SendAgentWitnessEmail(model.Witness1Email, existingRental.ExistingRentalId, 1);
					SendAgentWitnessEmail(model.Witness2Email, existingRental.ExistingRentalId, 2);

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

			ViewBag.Title = "Complete Existing Rental - Agent";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteAgent", model);

			// Default
			return View("../ExistingRentals/CompleteAgent", model);
		}

		[HttpPost, Route("account/existing-rental/a/complete-witness-signature")]
		public ActionResult CompleteWitnessSignature(CompleteExistingRentalWitnessLandlordSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
					{
						var existingRental = GetExistingRental[model.ExistingRentalId].Get();

						//add the signatures
						UploadPicture.WithPath($"signatures/existing-rental/{existingRental.ExistingRentalId}");

						var signatureData = RemoveWhitespace(model.Signature);

						string name = model.WitnessNumber == 1 ? existingRental.AgentWitness1Name : existingRental.AgentWitness2Name;
						string surname = model.WitnessNumber == 2 ? existingRental.AgentWitness1Surname : existingRental.AgentWitness2Surname;

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

						var pictures = UploadPicture.Save();
						bool success = false;

						if (model.WitnessNumber == 1)
						{
							success = UpdateExistingRentalService[existingRental.ExistingRentalId]
							.Set(a => a.AgentWitness1SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.ModifiedOn, DateTime.Now)
							.Update();
						}

						if (model.WitnessNumber == 2)
						{
							success = UpdateExistingRentalService[existingRental.ExistingRentalId]
							.Set(a => a.AgentWitness2SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.ModifiedOn, DateTime.Now)
							.Update();
						}

						existingRental = GetExistingRental[model.ExistingRentalId].Get();

						// Success
						if (success)
						{
							//verify if landlord witnesses signed
							if (existingRental.AgentWitness1SignatureId.HasValue && existingRental.AgentWitness2SignatureId.HasValue)
							{
								//fill all the forms, and email it
								//update the rental
								UpdateExistingRentalService[model.ExistingRentalId]
								.Set(a => a.ModifiedOn, DateTime.Now)
								.Set(a => a.ExistingRentalStatus, ExistingRentalStatus.Completed)
								.Update();

								//fill the correct forms
								FillForms(existingRental);

								existingRental = GetExistingRental[model.ExistingRentalId].Get();

								//send the docs to the landlords
								foreach (var landlord in existingRental.Rental.RentalLandlords) SendAmmendedAddendumEmail(landlord.Person.Email, existingRental);

								//send the docs to the agent
								SendAmmendedAddendumEmail(existingRental.Rental.AgentPerson.Email, existingRental);
							}
							else
							{
								//update the rental
								UpdateExistingRentalService[model.ExistingRentalId]
								.Set(a => a.ModifiedOn, DateTime.Now)
								.Update();
							}

							//complete the scope
							transactionScope.Complete();

							// Ajax (+ Json)
							if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
							{
								Success = true,
								AgentId = 1,
							}, JsonRequestBehavior.AllowGet);

							// Default
							return Redirect($"/account/existing-rental/a/complete-witness-signature/success");
						}
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

			ModelState.AddModelError("", errors);

			ViewBag.Title = "Complete Agent Signature";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteSignature", model);

			// Default
			return View("../ExistingRentals/CompleteSignature", model);
		}

		#endregion

		#region Private Methods

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

		private bool SendAgentWitnessEmail(string email, int existignRentalId, int witnessNumber)
		{
			var url = $"account/existing-rental/emails/agent-witness-email?witnessNumber={witnessNumber}&existingRentalId={existignRentalId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("Existing Rental Listing - Agent Witness Signature");
		}


		private bool SendAmmendedAddendumEmail(string email, ExistingRentalGet existingRental)
		{
			//var url = $"account/rental/emails/landlord-documentation-email?rentalId={rental.RentalId}&landlordId={landlordId}";

			//var attachments = new List<Attachment>();

			//SendMail.WithUrlBody(url).WithRecipient(email);

			//var mandate = rental.Forms.FirstOrDefault(a => a.Type == "MandateAgreement");

			//var data = new WebClient().DownloadData(mandate.Path);
			//SendMail.WithAttachment(new Attachment(new MemoryStream(data), mandate.Name, mandate.MimeType));

			//return SendMail.Send("Completed Listing - Landlord Documentation");

			return true;
		}

		private void FillForms(ExistingRentalGet existingRental)
		{
			if (existingRental.ExistingRentalType == ExistingRentalType.AddendumMandate)
			{
				//var landlordWitness1Signature = new WebClient().DownloadData(existingRental.LandlordWitness1Signature.Path);
				//var landlordWitness2Signature = new WebClient().DownloadData(existingRental.LandlordWitness2Signature.Path);

				//var landlordSignature = new WebClient().DownloadData(existingRental.Rental.RentalLandlords.FirstOrDefault().Signature.Path);
				//var agentSignature = new WebClient().DownloadData(existingRental.Rental.Agent.Signature.Path);

				//var agentWitness1Signature = new WebClient().DownloadData(existingRental.AgentWitness1Signature.Path);
				//var agentWitness2Signature = new WebClient().DownloadData(existingRental.AgentWitness2Signature.Path);

				//var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

				//var mandateAgreement = FillForm.Prepare("")
				//			.WithField("TheAgent", $"{rental.AgentPerson.FirstName.ToUpper()} {rental.AgentPerson.Surname.ToUpper()}")
				//			.WithField("AgentIdNumber", $"{rental.Agent.IdNumber}")
				//			.WithField("AgentVATNumber", $"")
				//			.WithField("FFCNumber", $"{rental.Agent.FfcNumber}")
				//			.WithField("TheLandlord", $"{String.Join(" / ", rental.RentalLandlords.Select(a => a.Person.FirstName + " " + a.Person.Surname))}")
				//			.WithField("LandlordIdNumber", rental.RentalLandlords.FirstOrDefault().IdNumber)
				//			.WithField("LandlordVatNumber", rental.RentalLandlords.FirstOrDefault().VatNumber)
				//			.WithField("LandlordIncomeTaxNumber", rental.RentalLandlords.FirstOrDefault().IncomeTaxNumber)
				//			.WithField("ThePremises", rental.Premises)
				//			.WithField("StandErf", rental.StandErf)
				//			.WithField("Township", rental.Township)
				//			.WithField("Address", rental.Address)
				//			.WithField("MonthlyRental", rental.MonthlyRental.ToString("F"))
				//			.WithField("Deposit", rental.Deposit.ToString("F"))
				//			.WithField("RentalPaymentDate", rental.MonthlyPaymentDate.ToString("yyyy-MM-dd"))
				//			.WithField("DepositPaymentDate", rental.DepositPaymentDate.ToString("yyyy-MM-dd"))
				//			.WithField("FirstProcurementPercentage", rental.RentalMandate.Procurement1Percentage.Value.ToString())
				//			.WithField("FirstProcurementAmount", rental.RentalMandate.Procurement1Amount.Value.ToString("F"))
				//			.WithField("SecondProcurementPercentage", rental.RentalMandate.Procurement2Percentage.Value.ToString())
				//			.WithField("SecondProcurementAmount", rental.RentalMandate.Procurement2Amount.Value.ToString("F"))
				//			.WithField("ThirdProcurementPercentage", rental.RentalMandate.Procurement3Percentage.Value.ToString())
				//			.WithField("ThirdProcurementAmount", rental.RentalMandate.Procurement3Amount.Value.ToString("F"))
				//			.WithField("ManagementCommissionPercentage", rental.RentalMandate.ManagementPercentage.Value.ToString())
				//			.WithField("ManagementCommissionAmount", rental.RentalMandate.ManagementAmount.Value.ToString("F"))
				//			.WithField("SaleCommissionPercentage", rental.RentalMandate.SalePercentage.Value.ToString())
				//			.WithField("SaleCommissionAmount", rental.RentalMandate.SaleAmount.Value.ToString("F"))
				//			.WithField("AccountHolder", rental.LandlordBankAccounts.FirstOrDefault().AccountHolder)
				//			.WithField("Bank", rental.LandlordBankAccounts.FirstOrDefault().Bank)
				//			.WithField("Branch", rental.LandlordBankAccounts.FirstOrDefault().Branch)
				//			.WithField("BranchCode", rental.LandlordBankAccounts.FirstOrDefault().BranchCode)
				//			.WithField("AccountNumber", rental.LandlordBankAccounts.FirstOrDefault().AccountNumber)
				//			.WithField("BankReference", rental.LandlordBankAccounts.FirstOrDefault().BankReference)
				//			.WithField("LandlordPhysicalAddress", rental.LandlordPhysicalAddress.Address1)
				//			.WithField("LandlordPostalAddress", rental.LandlordPostalAddress.Address1)
				//			.WithField("LandlordEmail", rental.RentalLandlords.FirstOrDefault().Person.Email)
				//			.WithField("LandlordPhoneNumber", rental.RentalLandlords.FirstOrDefault().Person.Mobile)
				//			.WithField("AgentPhysicalAddress", rental.AgentPhysicalAddress.Address1)
				//			.WithField("AgentPostalAddress", rental.AgentPostalAddress.Address1)
				//			.WithField("AgentEmail", rental.AgentPerson.Email)
				//			.WithField("AgentPhoneNumber", rental.AgentPerson.Mobile)

				//			.WithCheckbox("FindingTenant", rental.Procurement)
				//			.WithCheckbox("Management", rental.Management)
				//			.WithCheckbox("Negotiating", rental.Negotiating)
				//			.WithCheckbox("Informing", rental.Informing)
				//			.WithCheckbox("IncomingSnaglist", rental.IncomingSnaglist)
				//			.WithCheckbox("OutgoingSnaglist", rental.OutgoingSnaglist)
				//			.WithCheckbox("Explaining", rental.Explaining)
				//			.WithCheckbox("Paying", rental.PayingLandlord)
				//			.WithCheckbox("ContactLandlord", rental.ContactLandlord)
				//			.WithCheckbox("ProvideLandlord", rental.ProvideLandlord)
				//			.WithCheckbox("AskConsent", rental.AskLandlordConsent)
				//			.WithCheckbox("DepositFromLandlord", rental.ProcureDepositLandlord)
				//			.WithCheckbox("DepositFromPreviousRentalAgent", rental.ProcureDepositPreviousRentalAgent)
				//			.WithCheckbox("ProcureDepositInTrust", rental.TransferDeposit)
				//			.WithField("DepositFromOther", rental.ProcureDepositOther)
				//			.WithField("SpecificRequirements", rental.SpecificRequirements)
				//			.WithField("AgentSignedAt", rental.Agent.SignedAt)
				//			.WithField("AgentSignedOnDay", rental.Agent.SignedOn.Value.ToString("dd"))
				//			.WithField("AgentSignedOnMonth", rental.Agent.SignedOn.Value.ToString("MMMM"))
				//			.WithField("AgentSignedOnYear", rental.Agent.SignedOn.Value.ToString("yy"))
				//			.WithField("AgentName", $"{rental.AgentPerson.FirstName} {rental.AgentPerson.Surname}")
				//			.WithField("AgentWitness1", $"{rental.RentalWitness.AgentWitness1Name} {rental.RentalWitness.AgentWitness1Surname}")
				//			.WithField("AgentWitness2", $"{rental.RentalWitness.AgentWitness2Name} {rental.RentalWitness.AgentWitness2Surname}")
				//			.WithField("LandlordSignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
				//			.WithField("LandlordSignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
				//			.WithField("LandlordSignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
				//			.WithField("LandlordSignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))
				//			.WithField("LandlordName", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName} {rental.RentalLandlords.FirstOrDefault().Person.Surname}")
				//			.WithField("LandlordWitness1", $"{rental.RentalWitness.LandlordWitness1Name} {rental.RentalWitness.LandlordWitness1Surname}")
				//			.WithField("LandlordWitness2", $"{rental.RentalWitness.LandlordWitness2Name} {rental.RentalWitness.LandlordWitness2Surname}");

				////initials & signatures
				//mandateAgreement.WithImage(agentWitness1Initial, 0, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 0, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 0, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 0, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 0, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 0, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 1, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 1, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 1, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 1, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 1, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 1, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 2, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 2, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 2, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 2, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 2, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 2, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 3, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 3, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 3, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 3, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 3, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 3, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 4, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 4, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 4, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 4, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 4, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 4, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 5, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 5, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 5, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 5, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 5, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 5, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 6, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 6, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 6, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 6, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 6, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 6, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 7, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 7, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 7, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 7, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 7, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 7, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 8, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 8, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 8, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 8, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 8, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 8, 260, 80, 30, 30, true, true)

				//.WithImage(agentWitness1Initial, 9, 120, 80, 30, 30, true, true)
				//.WithImage(agentWitness2Initial, 9, 150, 80, 30, 30, true, true)
				//.WithImage(landlordWitness1Initial, 9, 180, 80, 30, 30, true, true)
				//.WithImage(landlordWitness2Initial, 9, 210, 80, 30, 30, true, true)
				//.WithImage(landlordInitials, 9, 230, 80, 30, 30, true, true)
				//.WithImage(agentInitials, 9, 260, 80, 30, 30, true, true)

				////agent and witnesses
				//.WithImage(agentSignature, 9, 80, 200, 50, 50)
				//.WithImage(agentWitness1Signature, 9, 150, 200, 50, 50, true)
				//.WithImage(agentWitness2Signature, 9, 150, 260, 50, 50, true)

				////landlord and witnesses
				//.WithImage(landlordSignature, 9, 80, 420, 50, 50)
				//.WithImage(landlordWitness1Signature, 9, 150, 420, 50, 50, true)
				//.WithImage(landlordWitness2Signature, 9, 150, 480, 50, 50, true);

				////special conditions
				//for (int i = 0; i < 14 && i < specialConditions.Count(); i++)
				//{
				//	var specialConditionsLine = specialConditions[i];
				//	mandateAgreement.WithField($"SpecialConditionsRow{i + 1}", specialConditionsLine);
				//}

				//var mandateAgreementData = mandateAgreement.Process();

				//UploadDownload.WithPath("rental_forms");

				//UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
				//{
				//	Action = Web.Models.Common.CrudAction.Create,
				//	ContentType = "application/pdf",
				//	DownloadType = Core.Domain.Media.DownloadType.Document,
				//	Extension = "pdf",
				//	FileName = $"{rental.RentalGuid} Mandate Agreement Form_{dateStamp}.pdf",
				//	Key = $"{rental.RentalGuid} Mandate Agreement Form_{dateStamp}.pdf",
				//	MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
				//	Data = mandateAgreementData,
				//	Size = mandateAgreementData.Length
				//});

				//var downloads = UploadDownload.Save();

				//CreateRentalForm.New(RentalFormName.MandateAgreement, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();
			}
		}

		private static IEnumerable<string> SplitToLines(string stringToSplit, int maxLineLength)
		{
			if (String.IsNullOrEmpty(stringToSplit)) yield return "";

			string[] words = stringToSplit.Split(' ');
			StringBuilder line = new StringBuilder();
			foreach (string word in words)
			{
				if (word.Length + line.Length <= maxLineLength)
				{
					line.Append(word + " ");
				}
				else
				{
					if (line.Length > 0)
					{
						yield return line.ToString().Trim();
						line.Clear();
					}
					string overflow = word;
					while (overflow.Length > maxLineLength)
					{
						yield return overflow.Substring(0, maxLineLength);
						overflow = overflow.Substring(maxLineLength);
					}
					line.Append(overflow + " ");
				}
			}
			yield return line.ToString().Trim();
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
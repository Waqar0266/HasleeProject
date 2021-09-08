using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Common;
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
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	public class CompleteRentalAgentSignatureController : BaseController
	{
		#region Private Properties 

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
		private IUpdateRentalMandateService UpdateRentalMandateService { get; }
		private IUpdateAgentService UpdateAgent { get; }
		private IGetRentalService GetRental { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteRentalAgentSignatureController
		(
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
			IUpdateRentalMandateService updateRentalMandateService,
			IUpdateAgentService updateAgent,
			IGetRentalService getRental,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
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
			UpdateRentalMandateService = updateRentalMandateService;
			UpdateAgent = updateAgent;
			GetRental = getRental;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/rental/a/complete")]
		[AccessControlFilter(Permission = "Agent")]
		public ActionResult CompleteAgent(string hash)
		{
			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			string rentalUniqueId = decodedHash.Split(';')[0];
			string landlordUniqueId = decodedHash.Split(';')[1];

			var rental = GetRental[rentalUniqueId].Get();

			if (rental.RentalStatus != RentalStatus.PendingAgentSignature) return Redirect("/account/rentals");

			var model = new CompleteRentalAgent
			{
				RentalGuid = rentalUniqueId,
				AgentGuid = landlordUniqueId,
				Address = rental.Address,
				AskLandlordConsent = rental.AskLandlordConsent,
				ContactLandlord = rental.ContactLandlord,
				Deposit = rental.Deposit,
				DepositPaymentDate = rental.DepositPaymentDate,
				Explaining = rental.Explaining,
				FfcNumber = rental.Agent.FfcNumber,
				IncomingSnaglist = rental.IncomingSnaglist,
				Informing = rental.Informing,
				Management = rental.Management,
				ManagementAmount = rental.RentalMandate.ManagementAmount,
				ManagementPercentage = rental.RentalMandate.ManagementPercentage,
				MonthlyRental = rental.MonthlyRental,
				Negotiating = rental.Negotiating,
				OutgoingSnaglist = rental.OutgoingSnaglist,
				PayingLandlord = rental.PayingLandlord,
				Premises = rental.Premises,
				ProcureDepositLandlord = rental.ProcureDepositLandlord,
				ProcureDepositOther = rental.ProcureDepositOther,
				ProcureDepositPreviousRentalAgent = rental.ProcureDepositPreviousRentalAgent,
				Procurement = rental.Procurement,
				Procurement1Amount = rental.RentalMandate.Procurement1Amount,
				Procurement1Percentage = rental.RentalMandate.Procurement1Percentage,
				Procurement2Amount = rental.RentalMandate.Procurement2Amount,
				Procurement2Percentage = rental.RentalMandate.Procurement2Percentage,
				Procurement3Amount = rental.RentalMandate.Procurement3Amount,
				Procurement3Percentage = rental.RentalMandate.Procurement3Percentage,
				ProvideLandlord = rental.ProvideLandlord,
				Rental = rental.MonthlyRental,
				RentalPaymentDate = rental.MonthlyPaymentDate,
				SaleAmount = rental.RentalMandate.SaleAmount,
				SalePercentage = rental.RentalMandate.SalePercentage,
				SpecialConditions = rental.SpecialConditions,
				SpecificRequirements = rental.SpecificRequirements,
				StandErf = rental.StandErf,
				Township = rental.Township,
				TransferDeposit = rental.TransferDeposit,
				VatNumber = rental.Agent.VatNumber,
				Marketing = rental.Marketing
			};

			ViewBag.Title = "Complete Rental - Agent";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteAgent", model);

			// Default
			return View("../Rentals/CompleteAgent", model);
		}

		[HttpGet, Route("account/rental/a/complete-witness-signature/success")]
		public ActionResult CompleteLandlordWitnessSignatureSuccess()
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/complete-witness-signature/success");
			}

			ViewBag.Title = "Completed Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteWitnessSignatureSuccess");

			// Default
			return View("../Rentals/CompleteWitnessSignatureSuccess");
		}

		[HttpGet, Route("account/rental/a/complete-witness-signature")]
		public ActionResult CompleteAgentWitnessSignature(string hash)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/rental/a/complete-witness-signature?hash={hash}");
			}

			var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

			var rentalUniqueId = decodedHash.Split(';')[0];
			var witnessUniqueId = decodedHash.Split(';')[1];
			var witnessNumber = Int32.Parse(decodedHash.Split(';')[2]);

			var rental = GetRental[rentalUniqueId].Get();
			if (witnessNumber == 1 && rental.RentalWitness.AgentWitness1SignatureId.HasValue && rental.RentalWitness.AgentWitness1InitialsId.HasValue) return Redirect("/account/rental/a/complete-witness-signature/success");
			if (witnessNumber == 2 && rental.RentalWitness.AgentWitness2SignatureId.HasValue && rental.RentalWitness.AgentWitness2InitialsId.HasValue) return Redirect("/account/rental/a/complete-witness-signature/success");

			var model = new CompleteRentalWitnessAgentSignature
			{
				UniqueId = witnessUniqueId,
				WitnessNumber = witnessNumber,
				RentalId = rental.RentalId
			};

			ViewBag.Title = "Complete Agent Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteAgentWitnessSignature", model);

			// Default
			return View("../Rentals/CompleteAgentWitnessSignature", model);
		}

		[HttpPost, Route("account/rental/a/complete")]
		public ActionResult CompleteAgent(CompleteRentalAgent model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var rental = GetRental[model.RentalGuid].Get();

					var success = UpdateRentalService[rental.RentalId]
					.Set(a => a.Address, model.Address)
					.Set(a => a.Deposit, model.Deposit)
					.Set(a => a.DepositPaymentDate, model.DepositPaymentDate)
					.Set(a => a.ModifiedOn, DateTime.Now)
					.Set(a => a.MonthlyPaymentDate, model.RentalPaymentDate)
					.Set(a => a.MonthlyRental, model.MonthlyRental)
					.Set(a => a.Premises, model.Premises)
					.Set(a => a.StandErf, model.StandErf)
					.Set(a => a.Township, model.Township)
					.Set(a => a.RentalStatus, RentalStatus.PendingAgentWitnessSignature)
					.Set(a => a.Marketing, model.Marketing)
					.Set(a => a.Procurement, model.Procurement)
					.Set(a => a.Management, model.Management)
					.Set(a => a.Negotiating, model.Negotiating)
					.Set(a => a.Informing, model.Informing)
					.Set(a => a.IncomingSnaglist, model.IncomingSnaglist)
					.Set(a => a.OutgoingSnaglist, model.OutgoingSnaglist)
					.Set(a => a.Explaining, model.Explaining)
					.Set(a => a.PayingLandlord, model.PayingLandlord)
					.Set(a => a.ContactLandlord, model.ContactLandlord)
					.Set(a => a.ProvideLandlord, model.ProvideLandlord)
					.Set(a => a.AskLandlordConsent, model.AskLandlordConsent)
					.Set(a => a.ProcureDepositLandlord, model.ProcureDepositLandlord)
					.Set(a => a.ProcureDepositPreviousRentalAgent, model.ProcureDepositPreviousRentalAgent)
					.Set(a => a.ProcureDepositOther, model.ProcureDepositOther)
					.Set(a => a.TransferDeposit, model.TransferDeposit)
					.Set(a => a.SpecificRequirements, model.SpecificRequirements)
					.Set(a => a.SpecialConditions, model.SpecialConditions)
					.Update();

					success = UpdateAgent[rental.AgentId]
					.Set(a => a.ModifiedOn, DateTime.Now)
					.Set(a => a.VatNumber, model.VatNumber)
					.Update();

					var rentalMandateId = 0;
					if (rental.RentalMandate != null) rentalMandateId = rental.RentalMandate.RentalMandateId;

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

					//add the witnesses to the database
					int rentalWitnessId = 0;
					if (rental.RentalWitness != null) rentalWitnessId = rental.RentalWitness.RentalWitnessId;
					success = UpdateRentalWitnessService.WithRentalId(rental.RentalId)[rentalWitnessId]
					.Set(r => r.AgentWitness1Email, model.Witness1Email)
					.Set(r => r.AgentWitness1Name, model.Witness1Name)
					.Set(r => r.AgentWitness1Surname, model.Witness1Surname)
					.Set(r => r.AgentWitness1Mobile, model.Witness1Mobile)
					.Set(r => r.AgentWitness2Email, model.Witness2Email)
					.Set(r => r.AgentWitness2Name, model.Witness2Name)
					.Set(r => r.AgentWitness2Surname, model.Witness2Surname)
					.Set(r => r.AgentWitness2Mobile, model.Witness2Mobile)
					.Update();

					rentalWitnessId = UpdateRentalWitnessService.RentalWitnessId;

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
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteAgent", model);

			// Default
			return View("../Agents/CompleteAgent", model);
		}

		[HttpPost, Route("account/rental/a/complete-witness-signature")]
		public ActionResult CompleteWitnessSignature(CompleteRentalWitnessLandlordSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
					{
						var rental = GetRental[model.RentalId].Get();

						//add the signatures
						UploadPicture.WithPath($"signatures/rental/{rental.RentalId}");

						var signatureData = RemoveWhitespace(model.Signature);

						string name = model.WitnessNumber == 1 ? rental.RentalWitness.AgentWitness1Name : rental.RentalWitness.AgentWitness2Name;
						string surname = model.WitnessNumber == 2 ? rental.RentalWitness.AgentWitness1Surname : rental.RentalWitness.AgentWitness2Surname;

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
							success = UpdateRentalWitnessService.WithRentalId(rental.RentalWitness.RentalId)[rental.RentalWitness.RentalWitnessId]
							.Set(a => a.AgentWitness1SignedAt, model.SignedAtSignature)
							.Set(a => a.AgentWitness1SignedOn, DateTime.Now)
							.Set(a => a.AgentWitness1SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.AgentWitness1InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png").PictureId)
							.Update();
						}

						if (model.WitnessNumber == 2)
						{
							success = UpdateRentalWitnessService.WithRentalId(rental.RentalWitness.RentalId)[rental.RentalWitness.RentalWitnessId]
							.Set(a => a.AgentWitness2SignedAt, model.SignedAtSignature)
							.Set(a => a.AgentWitness2SignedOn, DateTime.Now)
							.Set(a => a.AgentWitness2SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.AgentWitness2InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initial.png").PictureId)
							.Update();
						}

						int rentalWitnessId = UpdateRentalWitnessService.RentalWitnessId;

						rental = GetRental[model.RentalId].Get();

						// Success
						if (success)
						{
							//verify if landlord witnesses signed
							if (rental.RentalWitness.AgentWitness1SignatureId.HasValue && rental.RentalWitness.AgentWitness1InitialsId.HasValue && rental.RentalWitness.AgentWitness2SignatureId.HasValue && rental.RentalWitness.AgentWitness2InitialsId.HasValue)
							{
								//fill all the forms, and email it
								//update the rental
								UpdateRentalService[rental.RentalId]
								.Set(a => a.ModifiedOn, DateTime.Now)
								.Set(a => a.RentalStatus, RentalStatus.PendingProperty24)
								.Update();

								//fill the correct forms
								FillForms(rental);

								//send the email to the agent to link the property
								SendAgentPropertyLinkEmail(rental.AgentPerson.Email, rental.RentalId);
							}
							else
							{
								//update the rental
								UpdateRentalService[rental.RentalId]
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
							return Redirect($"/account/rental/a/complete-witness-signature/success");
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
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

			// Default
			return View("../Agents/CompleteSignature", model);
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

		private bool SendAgentWitnessEmail(string email, int rentalWitnessId, int rentalId, int witnessNumber)
		{
			var url = $"account/rental/emails/agent-witness-email?witnessNumber={witnessNumber}&rentalId={rentalId}&witnessId={rentalWitnessId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("New Listing - Agent Witness Signature");
		}

		private bool SendAgentPropertyLinkEmail(string email, int rentalId)
		{
			var url = $"account/rental/emails/agent-property-link-email?rentalId={rentalId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("New Listing - Agent Property 24 Link");
		}

		private void FillForms(RentalGet rental)
		{
			var landlordWitness1Initial = new WebClient().DownloadData(rental.RentalWitness.LandlordWitness1Initials.Path);
			var landlordWitness1Signature = new WebClient().DownloadData(rental.RentalWitness.LandlordWitness1Signature.Path);
			var landlordWitness2Initial = new WebClient().DownloadData(rental.RentalWitness.LandlordWitness2Initials.Path);
			var landlordWitness2Signature = new WebClient().DownloadData(rental.RentalWitness.LandlordWitness2Signature.Path);

			var landlordInitials = new WebClient().DownloadData(rental.RentalLandlords.FirstOrDefault().Initials.Path);
			var landlordSignature = new WebClient().DownloadData(rental.RentalLandlords.FirstOrDefault().Signature.Path);

			var agentSignature = new WebClient().DownloadData(rental.Agent.Signature.Path);
			var agentInitials = new WebClient().DownloadData(rental.Agent.Initials.Path);

			var agentWitness1Initial = new WebClient().DownloadData(rental.RentalWitness.AgentWitness1Initials.Path);
			var agentWitness1Signature = new WebClient().DownloadData(rental.RentalWitness.AgentWitness1Signature.Path);
			var agentWitness2Initial = new WebClient().DownloadData(rental.RentalWitness.AgentWitness2Initials.Path);
			var agentWitness2Signature = new WebClient().DownloadData(rental.RentalWitness.AgentWitness2Signature.Path);

			string mandateAgreementName = "";
			string powerOfAttorneyName = "";

			var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

			var specialConditions = String.IsNullOrEmpty(rental.SpecialConditions) ? new List<string>() : SplitToLines(rental.SpecialConditions, 170).ToList();

			switch (rental.LeaseType)
			{
				case LeaseType.Natural:
					{
						mandateAgreementName = "Mandate Agreement - Natural.pdf";
						powerOfAttorneyName = "Special Power of Attorney – Natural.pdf";



						break;
					}
				case LeaseType.ClosedCorporation:
					{
						mandateAgreementName = "Mandate Agreement - Juristic.pdf";
						powerOfAttorneyName = "Special Power of Attorney – Juristic.pdf";

						//fill fica prescribed form
						var ficaPrescribedForm = FillForm.Prepare("FICA PRESCRIBED CLIENT FORM (JURISTIC PERSON).pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithCheckbox("PtyLimited", rental.RentalFica.CompanyType == CompanyType.Pty)
						.WithCheckbox("CloseCorporation", rental.RentalFica.CompanyType == CompanyType.ClosedCorporation)
						.WithCheckbox("Partnership", rental.RentalFica.CompanyType == CompanyType.Partnership)
						.WithCheckbox("Trust", rental.RentalFica.CompanyType == CompanyType.Trust)
						.WithField("RegisteredAddress", rental.RentalFica.RegisteredAddress.Address1)
						.WithField("TradeName", rental.RentalFica.TradeName)
						.WithField("HeadOfficeAddress", rental.RentalFica.HeadOfficeAddress.Address1)
						.WithField("BranchAddress", rental.RentalFica.BranchAddress.Address1)
						.WithField("Home", rental.RentalFica.Phone)
						.WithField("Work", rental.RentalFica.Work)
						.WithField("Fax", rental.RentalFica.Fax)
						.WithField("Mobile", rental.RentalFica.Mobile)
						.WithField("Email", rental.RentalFica.Email)
						.WithField("Member1NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
						.WithField("Member1IdNumber", rental.RentalFica.Partner1IdNumber)
						.WithField("Member1Nationality", rental.RentalFica.Partner1Nationality)
						.WithField("Member1Address", rental.RentalFica.Partner1Address?.Address1 ?? "")
						.WithField("Member1Home", rental.RentalFica.Partner1Phone)
						.WithField("Member1Work", rental.RentalFica.Partner1Work)
						.WithField("Member1Fax", rental.RentalFica.Partner1Fax)
						.WithField("Member1Mobile", rental.RentalFica.Partner1Mobile)
						.WithField("Member1Email", rental.RentalFica.Partner1Email)
						.WithField("Member2NameSurname", $"{rental.RentalFica.Partner2Name} {rental.RentalFica.Partner2Surname}")
						.WithField("Member2IdNumber", rental.RentalFica.Partner2IdNumber)
						.WithField("Member2Nationality", rental.RentalFica.Partner2Nationality)
						.WithField("Member2Address", rental.RentalFica.Partner2Address?.Address1 ?? "")
						.WithField("Member2Home", rental.RentalFica.Partner2Phone)
						.WithField("Member2Work", rental.RentalFica.Partner2Work)
						.WithField("Member2Fax", rental.RentalFica.Partner2Fax)
						.WithField("Member2Mobile", rental.RentalFica.Partner2Mobile)
						.WithField("Member2Email", rental.RentalFica.Partner2Email)
						.WithField("Member3NameSurname", $"{rental.RentalFica.Partner3Name} {rental.RentalFica.Partner3Surname}")
						.WithField("Member3IdNumber", rental.RentalFica.Partner3IdNumber)
						.WithField("Member3Nationality", rental.RentalFica.Partner3Nationality)
						.WithField("Member3Address", rental.RentalFica.Partner3Address?.Address1 ?? "")
						.WithField("Member3Home", rental.RentalFica.Partner3Phone)
						.WithField("Member3Work", rental.RentalFica.Partner3Work)
						.WithField("Member3Fax", rental.RentalFica.Partner3Fax)
						.WithField("Member3Mobile", rental.RentalFica.Partner3Mobile)
						.WithField("Member3Email", rental.RentalFica.Partner3Email)
						.WithField("StaffMember", rental.RentalFica.StaffMember)
						.WithField("TransactionType", rental.RentalFica.TransactionType)
						.WithField("SignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
						.WithField("SignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
						.WithField("SignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
						.WithField("SignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))
						.WithField("AgentNameSurname", $"{rental.AgentPerson.FirstName} {rental.AgentPerson.Surname}")
						.WithImage(agentSignature, 2, 80, 390, 50, 50);

						if (!String.IsNullOrEmpty(rental.RentalFica.Partner1IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member1DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner1IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner2IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member2DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner2IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner3IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member3DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner3IdNumber).ToString("yyyy-MM-dd"));

						var ficaPrescribedFormData = ficaPrescribedForm.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = ficaPrescribedFormData,
							Size = ficaPrescribedFormData.Length
						});

						var downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.FicaPrescribedClientForm, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						//fill resolution by the members
						var resolutionOfMembers = FillForm.Prepare("RESOLUTION BY THE MEMBERS OF A CLOSE CORPORATION.pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithField("HeldAt", rental.RentalResolution.HeldAt)
						.WithField("HeldOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("HeldOnYear", rental.RentalResolution.HeldOn.ToString("yy"))
						.WithField("Landlord", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName} {rental.RentalLandlords.FirstOrDefault().Person.Surname}")
						.WithField("AuthorizedNameSurname", $"{rental.RentalResolution.AuthorizedName} {rental.RentalResolution.AuthorizedSurname}")
						.WithField("Premises", rental.Premises)
						.WithField("DatedAt", rental.RentalResolution.HeldAt)
						.WithField("DatedOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("DatedOnYear", rental.RentalResolution.HeldOn.ToString("yy"));

						var memberCount = 1;
						int imageHeight = 265;
						foreach (var member in rental.RentalResolution.Members)
						{
							var memberSignature = new WebClient().DownloadData(member.Signature.Path);
							resolutionOfMembers = resolutionOfMembers
							.WithField($"Member{memberCount}NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
							.WithField($"Member{memberCount}IdNumber", rental.RentalFica.Partner1IdNumber)
							.WithImage(memberSignature, 0, 400, imageHeight, 20, 20);

							memberCount++;
							imageHeight = imageHeight + 17;
						}

						var resolutionOfMembersData = resolutionOfMembers.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Resolution by the Members_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Resolution by the Members_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = resolutionOfMembersData,
							Size = resolutionOfMembersData.Length
						});

						downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.ResolutionByTheMembers, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						break;
					}

				case LeaseType.Company:
					{
						mandateAgreementName = "Mandate Agreement - Juristic.pdf";
						powerOfAttorneyName = "Special Power of Attorney – Juristic.pdf";

						//fill fica prescribed form
						var ficaPrescribedForm = FillForm.Prepare("FICA PRESCRIBED CLIENT FORM (JURISTIC PERSON).pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithCheckbox("PtyLimited", rental.RentalFica.CompanyType == CompanyType.Pty)
						.WithCheckbox("CloseCorporation", rental.RentalFica.CompanyType == CompanyType.ClosedCorporation)
						.WithCheckbox("Partnership", rental.RentalFica.CompanyType == CompanyType.Partnership)
						.WithCheckbox("Trust", rental.RentalFica.CompanyType == CompanyType.Trust)
						.WithField("RegisteredAddress", rental.RentalFica.RegisteredAddress.Address1)
						.WithField("TradeName", rental.RentalFica.TradeName)
						.WithField("HeadOfficeAddress", rental.RentalFica.HeadOfficeAddress.Address1)
						.WithField("BranchAddress", rental.RentalFica.BranchAddress.Address1)
						.WithField("Home", rental.RentalFica.Phone)
						.WithField("Work", rental.RentalFica.Work)
						.WithField("Fax", rental.RentalFica.Fax)
						.WithField("Mobile", rental.RentalFica.Mobile)
						.WithField("Email", rental.RentalFica.Email)
						.WithField("Member1NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
						.WithField("Member1IdNumber", rental.RentalFica.Partner1IdNumber)
						.WithField("Member1Nationality", rental.RentalFica.Partner1Nationality)
						.WithField("Member1Address", rental.RentalFica.Partner1Address?.Address1 ?? "")
						.WithField("Member1Home", rental.RentalFica.Partner1Phone)
						.WithField("Member1Work", rental.RentalFica.Partner1Work)
						.WithField("Member1Fax", rental.RentalFica.Partner1Fax)
						.WithField("Member1Mobile", rental.RentalFica.Partner1Mobile)
						.WithField("Member1Email", rental.RentalFica.Partner1Email)
						.WithField("Member2NameSurname", $"{rental.RentalFica.Partner2Name} {rental.RentalFica.Partner2Surname}")
						.WithField("Member2IdNumber", rental.RentalFica.Partner2IdNumber)
						.WithField("Member2Nationality", rental.RentalFica.Partner2Nationality)
						.WithField("Member2Address", rental.RentalFica.Partner2Address?.Address1 ?? "")
						.WithField("Member2Home", rental.RentalFica.Partner2Phone)
						.WithField("Member2Work", rental.RentalFica.Partner2Work)
						.WithField("Member2Fax", rental.RentalFica.Partner2Fax)
						.WithField("Member2Mobile", rental.RentalFica.Partner2Mobile)
						.WithField("Member2Email", rental.RentalFica.Partner2Email)
						.WithField("Member3NameSurname", $"{rental.RentalFica.Partner3Name} {rental.RentalFica.Partner3Surname}")
						.WithField("Member3IdNumber", rental.RentalFica.Partner3IdNumber)
						.WithField("Member3Nationality", rental.RentalFica.Partner3Nationality)
						.WithField("Member3Address", rental.RentalFica.Partner3Address?.Address1 ?? "")
						.WithField("Member3Home", rental.RentalFica.Partner3Phone)
						.WithField("Member3Work", rental.RentalFica.Partner3Work)
						.WithField("Member3Fax", rental.RentalFica.Partner3Fax)
						.WithField("Member3Mobile", rental.RentalFica.Partner3Mobile)
						.WithField("Member3Email", rental.RentalFica.Partner3Email)
						.WithField("StaffMember", rental.RentalFica.StaffMember)
						.WithField("TransactionType", rental.RentalFica.TransactionType)
						.WithField("SignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
						.WithField("SignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
						.WithField("SignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
						.WithField("SignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))
						.WithField("AgentNameSurname", $"{rental.AgentPerson.FirstName} {rental.AgentPerson.Surname}")
						.WithImage(agentSignature, 2, 80, 390, 50, 50);

						if (!String.IsNullOrEmpty(rental.RentalFica.Partner1IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member1DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner1IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner2IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member2DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner2IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner3IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member3DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner3IdNumber).ToString("yyyy-MM-dd"));

						var ficaPrescribedFormData = ficaPrescribedForm.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = ficaPrescribedFormData,
							Size = ficaPrescribedFormData.Length
						});

						var downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.FicaPrescribedClientForm, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						//fill resolution by the members
						var resolutionOfMembers = FillForm.Prepare("RESOLUTION BY THE DIRECTORS OF A COMPANY.pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithField("HeldAt", rental.RentalResolution.HeldAt)
						.WithField("HeldOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("HeldOnYear", rental.RentalResolution.HeldOn.ToString("yy"))
						.WithField("Landlord", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName} {rental.RentalLandlords.FirstOrDefault().Person.Surname}")
						.WithField("AuthorizedNameSurname", $"{rental.RentalResolution.AuthorizedName} {rental.RentalResolution.AuthorizedSurname}")
						.WithField("Premises", rental.Premises)
						.WithField("DatedAt", rental.RentalResolution.HeldAt)
						.WithField("DatedOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("DatedOnYear", rental.RentalResolution.HeldOn.ToString("yy"));

						var memberCount = 1;
						int imageHeight = 265;
						foreach (var member in rental.RentalResolution.Members)
						{
							var memberSignature = new WebClient().DownloadData(member.Signature.Path);
							resolutionOfMembers = resolutionOfMembers
							.WithField($"Member{memberCount}NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
							.WithField($"Member{memberCount}IdNumber", rental.RentalFica.Partner1IdNumber)
							.WithImage(memberSignature, 0, 400, imageHeight, 20, 20);

							memberCount++;
							imageHeight = imageHeight + 17;
						}

						var resolutionOfMembersData = resolutionOfMembers.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Resolution by the Members_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Resolution by the Members_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = resolutionOfMembersData,
							Size = resolutionOfMembersData.Length
						});

						downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.ResolutionByTheMembers, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						break;
					}

				case LeaseType.Trust:
					{
						mandateAgreementName = "Mandate Agreement - Juristic.pdf";
						powerOfAttorneyName = "Special Power of Attorney – Juristic.pdf";

						//fill fica prescribed form
						var ficaPrescribedForm = FillForm.Prepare("FICA PRESCRIBED CLIENT FORM (JURISTIC PERSON).pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithCheckbox("PtyLimited", rental.RentalFica.CompanyType == CompanyType.Pty)
						.WithCheckbox("CloseCorporation", rental.RentalFica.CompanyType == CompanyType.ClosedCorporation)
						.WithCheckbox("Partnership", rental.RentalFica.CompanyType == CompanyType.Partnership)
						.WithCheckbox("Trust", rental.RentalFica.CompanyType == CompanyType.Trust)
						.WithField("RegisteredAddress", rental.RentalFica.RegisteredAddress.Address1)
						.WithField("TradeName", rental.RentalFica.TradeName)
						.WithField("HeadOfficeAddress", rental.RentalFica.HeadOfficeAddress.Address1)
						.WithField("BranchAddress", rental.RentalFica.BranchAddress.Address1)
						.WithField("Home", rental.RentalFica.Phone)
						.WithField("Work", rental.RentalFica.Work)
						.WithField("Fax", rental.RentalFica.Fax)
						.WithField("Mobile", rental.RentalFica.Mobile)
						.WithField("Email", rental.RentalFica.Email)
						.WithField("Member1NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
						.WithField("Member1IdNumber", rental.RentalFica.Partner1IdNumber)
						.WithField("Member1Nationality", rental.RentalFica.Partner1Nationality)
						.WithField("Member1Address", rental.RentalFica.Partner1Address?.Address1 ?? "")
						.WithField("Member1Home", rental.RentalFica.Partner1Phone)
						.WithField("Member1Work", rental.RentalFica.Partner1Work)
						.WithField("Member1Fax", rental.RentalFica.Partner1Fax)
						.WithField("Member1Mobile", rental.RentalFica.Partner1Mobile)
						.WithField("Member1Email", rental.RentalFica.Partner1Email)
						.WithField("Member2NameSurname", $"{rental.RentalFica.Partner2Name} {rental.RentalFica.Partner2Surname}")
						.WithField("Member2IdNumber", rental.RentalFica.Partner2IdNumber)
						.WithField("Member2Nationality", rental.RentalFica.Partner2Nationality)
						.WithField("Member2Address", rental.RentalFica.Partner2Address?.Address1 ?? "")
						.WithField("Member2Home", rental.RentalFica.Partner2Phone)
						.WithField("Member2Work", rental.RentalFica.Partner2Work)
						.WithField("Member2Fax", rental.RentalFica.Partner2Fax)
						.WithField("Member2Mobile", rental.RentalFica.Partner2Mobile)
						.WithField("Member2Email", rental.RentalFica.Partner2Email)
						.WithField("Member3NameSurname", $"{rental.RentalFica.Partner3Name} {rental.RentalFica.Partner3Surname}")
						.WithField("Member3IdNumber", rental.RentalFica.Partner3IdNumber)
						.WithField("Member3Nationality", rental.RentalFica.Partner3Nationality)
						.WithField("Member3Address", rental.RentalFica.Partner3Address?.Address1 ?? "")
						.WithField("Member3Home", rental.RentalFica.Partner3Phone)
						.WithField("Member3Work", rental.RentalFica.Partner3Work)
						.WithField("Member3Fax", rental.RentalFica.Partner3Fax)
						.WithField("Member3Mobile", rental.RentalFica.Partner3Mobile)
						.WithField("Member3Email", rental.RentalFica.Partner3Email)
						.WithField("StaffMember", rental.RentalFica.StaffMember)
						.WithField("TransactionType", rental.RentalFica.TransactionType)
						.WithField("SignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
						.WithField("SignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
						.WithField("SignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
						.WithField("SignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))
						.WithField("AgentNameSurname", $"{rental.AgentPerson.FirstName} {rental.AgentPerson.Surname}")
						.WithImage(agentSignature, 2, 80, 390, 50, 50);

						if (!String.IsNullOrEmpty(rental.RentalFica.Partner1IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member1DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner1IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner2IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member2DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner2IdNumber).ToString("yyyy-MM-dd"));
						if (!String.IsNullOrEmpty(rental.RentalFica.Partner3IdNumber)) ficaPrescribedForm = ficaPrescribedForm.WithField("Member3DateOfBirth", CalculateDateOfBirth(rental.RentalFica.Partner3IdNumber).ToString("yyyy-MM-dd"));

						var ficaPrescribedFormData = ficaPrescribedForm.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Fica Prescribed Form_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = ficaPrescribedFormData,
							Size = ficaPrescribedFormData.Length
						});

						var downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.FicaPrescribedClientForm, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						//fill resolution by the members
						var resolutionOfMembers = FillForm.Prepare("RESOLUTION BY THE TRUSTEES OF A TRUST.pdf")
						.WithField("RegisteredBusinessName", rental.RentalFica.RegisteredBusinessName)
						.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
						.WithField("HeldAt", rental.RentalResolution.HeldAt)
						.WithField("HeldOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("HeldOnYear", rental.RentalResolution.HeldOn.ToString("yy"))
						.WithField("Landlord", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName} {rental.RentalLandlords.FirstOrDefault().Person.Surname}")
						.WithField("AuthorizedNameSurname", $"{rental.RentalResolution.AuthorizedName} {rental.RentalResolution.AuthorizedSurname}")
						.WithField("Premises", rental.Premises)
						.WithField("DatedAt", rental.RentalResolution.HeldAt)
						.WithField("DatedOnDayMonth", rental.RentalResolution.HeldOn.ToString("dd MMMM"))
						.WithField("DatedOnYear", rental.RentalResolution.HeldOn.ToString("yy"));

						var memberCount = 1;
						int imageHeight = 265;
						foreach (var member in rental.RentalResolution.Members)
						{
							var memberSignature = new WebClient().DownloadData(member.Signature.Path);
							resolutionOfMembers = resolutionOfMembers
							.WithField($"Member{memberCount}NameSurname", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}")
							.WithField($"Member{memberCount}IdNumber", rental.RentalFica.Partner1IdNumber)
							.WithImage(memberSignature, 0, 400, imageHeight, 20, 20);

							memberCount++;
							imageHeight = imageHeight + 17;
						}

						var resolutionOfMembersData = resolutionOfMembers.Process();

						UploadDownload.WithPath("rental_forms");

						UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							ContentType = "application/pdf",
							DownloadType = Core.Domain.Media.DownloadType.Document,
							Extension = "pdf",
							FileName = $"{rental.RentalGuid} Resolution by the Trustees_{dateStamp}.pdf",
							Key = $"{rental.RentalGuid} Resolution by the Trustees_{dateStamp}.pdf",
							MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
							Data = resolutionOfMembersData,
							Size = resolutionOfMembersData.Length
						});

						downloads = UploadDownload.Save();

						CreateRentalForm.New(RentalFormName.ResolutionByTheMembers, rental.RentalId, downloads.FirstOrDefault().DownloadId).Create();

						break;
					}

				default:
					break;
			}

			var mandateAgreement = FillForm.Prepare(mandateAgreementName)
						.WithField("TheAgent", $"{rental.AgentPerson.FirstName.ToUpper()} {rental.AgentPerson.Surname.ToUpper()}")
						.WithField("AgentIdNumber", $"{rental.Agent.IdNumber}")
						.WithField("AgentVATNumber", $"")
						.WithField("FFCNumber", $"{rental.Agent.FfcNumber}")
						.WithField("TheLandlord", $"{String.Join(" / ", rental.RentalLandlords.Select(a => a.Person.FirstName + " " + a.Person.Surname))}")
						.WithField("LandlordIdNumber", rental.RentalLandlords.FirstOrDefault().IdNumber)
						.WithField("LandlordVatNumber", rental.RentalLandlords.FirstOrDefault().VatNumber)
						.WithField("LandlordIncomeTaxNumber", rental.RentalLandlords.FirstOrDefault().IncomeTaxNumber)
						.WithField("ThePremises", rental.Premises)
						.WithField("StandErf", rental.StandErf)
						.WithField("Township", rental.Township)
						.WithField("Address", rental.Address)
						.WithField("MonthlyRental", rental.MonthlyRental.ToString("F"))
						.WithField("Deposit", rental.Deposit.ToString("F"))
						.WithField("RentalPaymentDate", rental.MonthlyPaymentDate.ToString("yyyy-MM-dd"))
						.WithField("DepositPaymentDate", rental.DepositPaymentDate.ToString("yyyy-MM-dd"))
						.WithField("FirstProcurementPercentage", rental.RentalMandate.Procurement1Percentage.Value.ToString())
						.WithField("FirstProcurementAmount", rental.RentalMandate.Procurement1Amount.Value.ToString("F"))
						.WithField("SecondProcurementPercentage", rental.RentalMandate.Procurement2Percentage.Value.ToString())
						.WithField("SecondProcurementAmount", rental.RentalMandate.Procurement2Amount.Value.ToString("F"))
						.WithField("ThirdProcurementPercentage", rental.RentalMandate.Procurement3Percentage.Value.ToString())
						.WithField("ThirdProcurementAmount", rental.RentalMandate.Procurement3Amount.Value.ToString("F"))
						.WithField("ManagementCommissionPercentage", rental.RentalMandate.ManagementPercentage.Value.ToString())
						.WithField("ManagementCommissionAmount", rental.RentalMandate.ManagementAmount.Value.ToString("F"))
						.WithField("SaleCommissionPercentage", rental.RentalMandate.SalePercentage.Value.ToString())
						.WithField("SaleCommissionAmount", rental.RentalMandate.SaleAmount.Value.ToString("F"))
						.WithField("AccountHolder", rental.LandlordBankAccounts.FirstOrDefault().AccountHolder)
						.WithField("Bank", rental.LandlordBankAccounts.FirstOrDefault().Bank)
						.WithField("Branch", rental.LandlordBankAccounts.FirstOrDefault().Branch)
						.WithField("BranchCode", rental.LandlordBankAccounts.FirstOrDefault().BranchCode)
						.WithField("AccountNumber", rental.LandlordBankAccounts.FirstOrDefault().AccountNumber)
						.WithField("BankReference", rental.LandlordBankAccounts.FirstOrDefault().BankReference)
						.WithField("LandlordPhysicalAddress", rental.LandlordPhysicalAddress.Address1)
						.WithField("LandlordPostalAddress", rental.LandlordPostalAddress.Address1)
						.WithField("LandlordEmail", rental.RentalLandlords.FirstOrDefault().Person.Email)
						.WithField("LandlordPhoneNumber", rental.RentalLandlords.FirstOrDefault().Person.Mobile)
						.WithField("AgentPhysicalAddress", rental.AgentPhysicalAddress.Address1)
						.WithField("AgentPostalAddress", rental.AgentPostalAddress.Address1)
						.WithField("AgentEmail", rental.AgentPerson.Email)
						.WithField("AgentPhoneNumber", rental.AgentPerson.Mobile)

						.WithCheckbox("FindingTenant", rental.Procurement)
						.WithCheckbox("Management", rental.Management)
						.WithCheckbox("Negotiating", rental.Negotiating)
						.WithCheckbox("Informing", rental.Informing)
						.WithCheckbox("IncomingSnaglist", rental.IncomingSnaglist)
						.WithCheckbox("OutgoingSnaglist", rental.OutgoingSnaglist)
						.WithCheckbox("Explaining", rental.Explaining)
						.WithCheckbox("Paying", rental.PayingLandlord)
						.WithCheckbox("ContactLandlord", rental.ContactLandlord)
						.WithCheckbox("ProvideLandlord", rental.ProvideLandlord)
						.WithCheckbox("AskConsent", rental.AskLandlordConsent)
						.WithCheckbox("DepositFromLandlord", rental.ProcureDepositLandlord)
						.WithCheckbox("DepositFromPreviousRentalAgent", rental.ProcureDepositPreviousRentalAgent)
						.WithCheckbox("ProcureDepositInTrust", rental.TransferDeposit)
						.WithField("DepositFromOther", rental.ProcureDepositOther)
						.WithField("SpecificRequirements", rental.SpecificRequirements)
						.WithField("AgentSignedAt", rental.Agent.SignedAt)
						.WithField("AgentSignedOnDay", rental.Agent.SignedOn.Value.ToString("dd"))
						.WithField("AgentSignedOnMonth", rental.Agent.SignedOn.Value.ToString("MMMM"))
						.WithField("AgentSignedOnYear", rental.Agent.SignedOn.Value.ToString("yy"))
						.WithField("AgentName", $"{rental.AgentPerson.FirstName} {rental.AgentPerson.Surname}")
						.WithField("AgentWitness1", $"{rental.RentalWitness.AgentWitness1Name} {rental.RentalWitness.AgentWitness1Surname}")
						.WithField("AgentWitness2", $"{rental.RentalWitness.AgentWitness2Name} {rental.RentalWitness.AgentWitness2Surname}")
						.WithField("LandlordSignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
						.WithField("LandlordSignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
						.WithField("LandlordSignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
						.WithField("LandlordSignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))
						.WithField("LandlordName", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName} {rental.RentalLandlords.FirstOrDefault().Person.Surname}")
						.WithField("LandlordWitness1", $"{rental.RentalWitness.LandlordWitness1Name} {rental.RentalWitness.LandlordWitness1Surname}")
						.WithField("LandlordWitness2", $"{rental.RentalWitness.LandlordWitness2Name} {rental.RentalWitness.LandlordWitness2Surname}");

			if (rental.LeaseType == LeaseType.Natural)
				mandateAgreement = mandateAgreement
									.WithCheckbox("DirectMarketingYes", rental.Marketing)
									.WithCheckbox("DirectMarketingNo", !rental.Marketing);

			//initials & signatures
			mandateAgreement.WithImage(agentWitness1Initial, 0, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 0, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 0, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 0, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 0, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 0, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 1, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 1, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 1, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 1, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 1, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 1, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 2, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 2, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 2, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 2, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 2, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 2, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 3, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 3, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 3, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 3, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 3, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 3, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 4, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 4, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 4, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 4, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 4, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 4, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 5, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 5, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 5, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 5, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 5, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 5, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 6, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 6, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 6, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 6, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 6, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 6, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 7, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 7, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 7, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 7, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 7, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 7, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 8, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 8, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 8, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 8, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 8, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 8, 260, 80, 30, 30, true, true)

			.WithImage(agentWitness1Initial, 9, 120, 80, 30, 30, true, true)
			.WithImage(agentWitness2Initial, 9, 150, 80, 30, 30, true, true)
			.WithImage(landlordWitness1Initial, 9, 180, 80, 30, 30, true, true)
			.WithImage(landlordWitness2Initial, 9, 210, 80, 30, 30, true, true)
			.WithImage(landlordInitials, 9, 230, 80, 30, 30, true, true)
			.WithImage(agentInitials, 9, 260, 80, 30, 30, true, true)

			//agent and witnesses
			.WithImage(agentSignature, 9, 80, 200, 50, 50)
			.WithImage(agentWitness1Signature, 9, 150, 200, 50, 50, true)
			.WithImage(agentWitness2Signature, 9, 150, 260, 50, 50, true)

			//landlord and witnesses
			.WithImage(landlordSignature, 9, 80, 420, 50, 50)
			.WithImage(landlordWitness1Signature, 9, 150, 420, 50, 50, true)
			.WithImage(landlordWitness2Signature, 9, 150, 480, 50, 50, true);

			//special conditions
			for (int i = 0; i < 14 && i < specialConditions.Count(); i++)
			{
				var specialConditionsLine = specialConditions[i];
				mandateAgreement.WithField($"SpecialConditionsRow{i + 1}", specialConditionsLine);
			}

			var mandateAgreementData = mandateAgreement.Process();

			UploadDownload.WithPath("rental_forms");

			UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
			{
				Action = Web.Models.Common.CrudAction.Create,
				ContentType = "application/pdf",
				DownloadType = Core.Domain.Media.DownloadType.Document,
				Extension = "pdf",
				FileName = $"{rental.RentalGuid} Mandate Agreement Form_{dateStamp}.pdf",
				Key = $"{rental.RentalGuid} Mandate Agreement Form_{dateStamp}.pdf",
				MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
				Data = mandateAgreementData,
				Size = mandateAgreementData.Length
			});

			var downloads1 = UploadDownload.Save();

			CreateRentalForm.New(RentalFormName.MandateAgreement, rental.RentalId, downloads1.FirstOrDefault().DownloadId).Create();

			if (rental.PowerOfAttorney)
			{
				var specialPowerofAttroney = FillForm.Prepare(powerOfAttorneyName)
				.WithField("Landlord", $"{rental.RentalLandlords.FirstOrDefault().Person.FirstName.ToUpper()} {rental.RentalLandlords.FirstOrDefault().Person.Surname.ToUpper()}")
				.WithField("Agent", $"{rental.AgentPerson.FirstName.ToUpper()} {rental.AgentPerson.Surname.ToUpper()}")
				.WithField("IdentityNumber", $"{rental.RentalLandlords.FirstOrDefault().IdNumber}")
				.WithField("Premises", $"{rental.Premises}")
				.WithField("Witness1", $"{rental.RentalWitness.LandlordWitness1Name} {rental.RentalWitness.LandlordWitness1Surname}")
				.WithField("Witness2", $"{rental.RentalWitness.LandlordWitness2Name} {rental.RentalWitness.LandlordWitness2Surname}")
				.WithField("SignedAt", rental.RentalLandlords.FirstOrDefault().SignedAt)
				.WithField("SignedOnDay", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("dd"))
				.WithField("SignedOnMonth", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("MMMM"))
				.WithField("SignedOnYear", rental.RentalLandlords.FirstOrDefault().SignedOn.Value.ToString("yy"))

				//initials
				.WithImage(landlordInitials, 0, 120, 70, 30, 30, true, true)
				.WithImage(landlordWitness1Initial, 0, 150, 70, 30, 30, true, true)
				.WithImage(landlordWitness2Initial, 0, 180, 70, 30, 30, true, true)

				//landlord and witnesses
				.WithImage(landlordSignature, 0, 80, 560, 50, 50)
				.WithImage(landlordWitness1Signature, 0, 150, 560, 50, 50, true)
				.WithImage(landlordWitness2Signature, 0, 150, 620, 50, 50, true);

				if (rental.LeaseType != LeaseType.Natural)
				{
					specialPowerofAttroney = specialPowerofAttroney
												.WithField("RegistrationNumber", rental.RentalFica.RegistrationNumber)
												.WithField("DirectorMember", $"{rental.RentalFica.Partner1Name} {rental.RentalFica.Partner1Surname}");
				}

				var specialPowerofAttroneyData = specialPowerofAttroney.Process();

				UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
				{
					Action = Web.Models.Common.CrudAction.Create,
					ContentType = "application/pdf",
					DownloadType = Core.Domain.Media.DownloadType.Document,
					Extension = "pdf",
					FileName = $"{rental.RentalGuid} Special Power of Attorney Form_{dateStamp}.pdf",
					Key = $"{rental.RentalGuid} Special Power of Attorney Form_{dateStamp}.pdf",
					MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					Data = specialPowerofAttroneyData,
					Size = specialPowerofAttroneyData.Length
				});

				downloads1 = UploadDownload.Save();

				CreateRentalForm.New(RentalFormName.SpecialPowerOfAttorney, rental.RentalId, downloads1.FirstOrDefault().DownloadId).Create();
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
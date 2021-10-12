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
using System.Net.Mail;
using System.Text;
using System.Transactions;
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
				return Redirect($"/account/existing-rental/a/complete-witness-signature?hash={hash}");
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
						var initialsData = RemoveWhitespace(model.Initials);

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

						UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
						{
							Action = Web.Models.Common.CrudAction.Create,
							File = initialsData,
							Format = Core.Domain.Media.PictureFormat.Png,
							Key = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initials.png",
							Name = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initials.png",
							MimeType = "image/png",
							AlternateText = $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initials.jpg"
						});

						var pictures = UploadPicture.Save();
						bool success = false;

						if (model.WitnessNumber == 1)
						{
							success = UpdateExistingRentalService[existingRental.ExistingRentalId]
							.Set(a => a.AgentWitness1SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.AgentWitness1InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initials.png").PictureId)
							.Set(a => a.ModifiedOn, DateTime.Now)
							.Update();
						}

						if (model.WitnessNumber == 2)
						{
							success = UpdateExistingRentalService[existingRental.ExistingRentalId]
							.Set(a => a.AgentWitness2SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
							.Set(a => a.AgentWitness2InitialsId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_initials.png").PictureId)
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

								if (existingRental.ExistingRentalType == ExistingRentalType.AddendumMandate)
								{
									//send the docs to the agent and landlord
									SendAmmendedAddendumEmail(existingRental.Rental.AgentPerson.Email, existingRental);
								}
								else
								{
									//send the docs to the agent and landlord
									SendRenewTerminateEmail(existingRental.Rental.AgentPerson.Email, existingRental);
								}
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


		private void SendAmmendedAddendumEmail(string email, ExistingRentalGet existingRental)
		{
			var mandate = existingRental.Forms.FirstOrDefault(a => a.Type == ExistingRentalFormName.AmmendedAddendum.ToString());
			var data = new WebClient().DownloadData(mandate.Path);
			string url;
			foreach (var landlord in existingRental.Rental.RentalLandlords)
			{
				url = $"account/existing-rental/emails/landlord-documentation-email?existingRentalId={existingRental.ExistingRentalId}&landlordId={landlord.RentalLandlordId}";

				SendMail
				.WithUrlBody(url)
				.WithRecipient(email)
				.WithAttachment(new Attachment(new MemoryStream(data), mandate.Name, mandate.MimeType));

				SendMail.Send("Completed Existing Rental Listing - Landlord Documentation");
			}

			//send agent documentation
			url = $"account/existing-rental/emails/agent-documentation-email?existingRentalId={existingRental.ExistingRentalId}";

			SendMail
			.WithUrlBody(url)
			.WithRecipient(email)
			.WithAttachment(new Attachment(new MemoryStream(data), mandate.Name, mandate.MimeType));

			SendMail.Send("Completed Existing Rental Listing - Agent Documentation");
		}

		private void SendRenewTerminateEmail(string email, ExistingRentalGet existingRental)
		{
			var mandate = existingRental.Forms.FirstOrDefault(a => a.Type == ExistingRentalFormName.RenewalTermination.ToString());
			var data = new WebClient().DownloadData(mandate.Path);
			string url;
			foreach (var landlord in existingRental.Rental.RentalLandlords)
			{
				url = $"account/existing-rental/emails/landlord-documentation-email?existingRentalId={existingRental.ExistingRentalId}&landlordId={landlord.RentalLandlordId}";

				SendMail
				.WithUrlBody(url)
				.WithRecipient(email)
				.WithAttachment(new Attachment(new MemoryStream(data), mandate.Name, mandate.MimeType));

				SendMail.Send("Completed Existing Rental Listing - Landlord Documentation");
			}

			//send agent documentation
			url = $"account/existing-rental/emails/agent-documentation-email?existingRentalId={existingRental.ExistingRentalId}";

			SendMail
			.WithUrlBody(url)
			.WithRecipient(email)
			.WithAttachment(new Attachment(new MemoryStream(data), mandate.Name, mandate.MimeType));

			SendMail.Send("Completed Existing Rental Listing - Agent Documentation");
		}

		private void FillForms(ExistingRentalGet existingRental)
		{
			if (existingRental.ExistingRentalType == ExistingRentalType.AddendumMandate)
			{
				var landlordWitness1Initial = new WebClient().DownloadData(existingRental.LandlordWitness1Initials.Path);
				var landlordWitness1Signature = new WebClient().DownloadData(existingRental.LandlordWitness1Signature.Path);
				var landlordWitness2Initial = new WebClient().DownloadData(existingRental.LandlordWitness2Initials.Path);
				var landlordWitness2Signature = new WebClient().DownloadData(existingRental.LandlordWitness2Signature.Path);

				var landlordInitial = new WebClient().DownloadData(existingRental.Rental.RentalLandlords.FirstOrDefault().Initials.Path);
				var landlordSignature = new WebClient().DownloadData(existingRental.Rental.RentalLandlords.FirstOrDefault().Signature.Path);
				var agentInitial = new WebClient().DownloadData(existingRental.Rental.Agent.Initials.Path);
				var agentSignature = new WebClient().DownloadData(existingRental.Rental.Agent.Signature.Path);

				var agentWitness1Initial = new WebClient().DownloadData(existingRental.AgentWitness1Initials.Path);
				var agentWitness1Signature = new WebClient().DownloadData(existingRental.AgentWitness1Signature.Path);
				var agentWitness2Initial = new WebClient().DownloadData(existingRental.AgentWitness2Initials.Path);
				var agentWitness2Signature = new WebClient().DownloadData(existingRental.AgentWitness2Signature.Path);

				var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

				var agreement = FillForm.Prepare("ADDENDUM TO MANDATE AGREEMENT.pdf")
							.WithField("Agent", $"{existingRental.Rental.AgentPerson.FirstName.ToUpper()} {existingRental.Rental.AgentPerson.Surname.ToUpper()}")
							.WithField("Landlord", $"{String.Join(" / ", existingRental.Rental.RentalLandlords.Select(a => a.Person.FirstName + " " + a.Person.Surname))}")
							.WithField("StartingOn", existingRental.StartDate.Value.ToString("yyyy/MM/dd"))
							.WithField("EndingOn", existingRental.EndDate.Value.ToString("yyyy/MM/dd"))
							.WithField("Premises", existingRental.Rental.Premises)
							.WithField("AgentSignedAt", existingRental.Rental.Agent.SignedAt)
							.WithField("AgentSignedOnDay", DateTime.Now.ToString("dd"))
							.WithField("AgentSignedOnMonth", DateTime.Now.ToString("MMMM"))
							.WithField("AgentSignedOnYear", DateTime.Now.ToString("yy"))
							.WithField("LandlordSignedAt", existingRental.Rental.RentalLandlords.FirstOrDefault().SignedAt)
							.WithField("LandlordSignedOnDay", DateTime.Now.ToString("dd"))
							.WithField("LandlordSignedOnMonth", DateTime.Now.ToString("MMMM"))
							.WithField("LandlordSignedOnYear", DateTime.Now.ToString("yy"))

							.WithCheckbox("Procurement", existingRental.Rental.Procurement)
							.WithCheckbox("Management", existingRental.Rental.Management);

				var addendum = SplitToLines(existingRental.AmendedAddendum, 15).ToList();
				//addendum
				for (int i = 0; i < 14 && i < addendum.Count(); i++)
				{
					var addendumLine = addendum[i];
					agreement.WithField($"Line{i + 1}", addendumLine);
				}

				//initials page 1
				agreement.WithImage(agentInitial, 0, 90, 60, 20, 20, true, true)
				.WithImage(landlordInitial, 0, 110, 60, 20, 20, true, true)
				.WithImage(agentWitness1Initial, 0, 130, 60, 20, 20, true, true)
				.WithImage(agentWitness2Initial, 0, 150, 60, 20, 20, true, true)
				.WithImage(landlordWitness1Initial, 0, 170, 60, 20, 20, true, true)
				.WithImage(landlordWitness2Initial, 0, 190, 60, 20, 20, true, true)

				//initials page 2
				.WithImage(agentInitial, 1, 90, 60, 20, 20, true, true)
				.WithImage(landlordInitial, 1, 110, 60, 20, 20, true, true)
				.WithImage(agentWitness1Initial, 1, 130, 60, 20, 20, true, true)
				.WithImage(agentWitness2Initial, 1, 150, 60, 20, 20, true, true)
				.WithImage(landlordWitness1Initial, 1, 170, 60, 20, 20, true, true)
				.WithImage(landlordWitness2Initial, 1, 190, 60, 20, 20, true, true)

				//agent signatures and witnesses
				.WithImage(agentSignature, 1, 80, 340, 40, 40)
				.WithImage(agentWitness1Signature, 1, 450, 340, 40, 40)
				.WithImage(agentWitness2Signature, 1, 450, 400, 40, 40)

				//landlord signatures and witnesses
				.WithImage(landlordSignature, 1, 80, 540, 40, 40)
				.WithImage(landlordWitness1Signature, 1, 450, 540, 40, 40)
				.WithImage(landlordWitness2Signature, 1, 450, 600, 40, 40);

				var agreementData = agreement.Process();

				UploadDownload.WithPath("existing_rental_forms");

				UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
				{
					Action = Web.Models.Common.CrudAction.Create,
					ContentType = "application/pdf",
					DownloadType = Core.Domain.Media.DownloadType.Document,
					Extension = "pdf",
					FileName = $"{existingRental.ExistingRentalGuid} Addedum to Mandate Agreement_{dateStamp}.pdf",
					Key = $"{existingRental.ExistingRentalGuid} Addedum to Mandate Agreement_{dateStamp}.pdf",
					MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					Data = agreementData,
					Size = agreementData.Length
				});

				var downloads = UploadDownload.Save();

				CreateExistingRentalForm.New(ExistingRentalFormName.AmmendedAddendum, existingRental.ExistingRentalId, downloads.FirstOrDefault().DownloadId).Create();
			}
			else
			{
				var landlordWitness1Initial = new WebClient().DownloadData(existingRental.LandlordWitness1Initials.Path);
				var landlordWitness1Signature = new WebClient().DownloadData(existingRental.LandlordWitness1Signature.Path);
				var landlordWitness2Initial = new WebClient().DownloadData(existingRental.LandlordWitness2Initials.Path);
				var landlordWitness2Signature = new WebClient().DownloadData(existingRental.LandlordWitness2Signature.Path);

				var landlordInitial = new WebClient().DownloadData(existingRental.Rental.RentalLandlords.FirstOrDefault().Initials.Path);
				var landlordSignature = new WebClient().DownloadData(existingRental.Rental.RentalLandlords.FirstOrDefault().Signature.Path);

				var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

				var agreement = FillForm.Prepare("NOTICE OF TERMINATION OR RENEWAL OF FIXED TERM LEASE.pdf")
							.WithField("Landlord", $"{String.Join(" / ", existingRental.Rental.RentalLandlords.Select(a => a.Person.FirstName + " " + a.Person.Surname))}")
							.WithField("Tenant", existingRental.Tenant)
							.WithField("Premises", existingRental.Rental.Premises)
							.WithField("ParkingBays", existingRental.ParkingBays)
							.WithField("TerminationDate", existingRental.TerminationDate.HasValue ? existingRental.TerminationDate.Value.ToString("yyyy-MM-dd") : "")
							.WithCheckbox("RenewYes", existingRental.ExistingRentalType == ExistingRentalType.Renew)
							.WithCheckbox("RenewNo", existingRental.ExistingRentalType != ExistingRentalType.Renew)
							.WithField("RenewalPeriod", existingRental.RenewalPeriod)
							.WithField("RenewalCommencementDate", existingRental.RenewalCommencementDate.HasValue ? existingRental.RenewalCommencementDate.Value.ToString("yyyy-MM-dd") : "")
							.WithField("RenewalTerminationDate", existingRental.RenewalTerminationDate.HasValue ? existingRental.RenewalTerminationDate.Value.ToString("yyyy-MM-dd") : "")
							.WithField("Rent", existingRental.Rental.MonthlyRental.ToString())
							.WithField("Deposit", existingRental.Rental.Deposit.ToString())
							.WithField("SignedAt", existingRental.Rental.RentalLandlords.FirstOrDefault().SignedAt)
							.WithField("SignedOnDay", DateTime.Now.ToString("dd"))
							.WithField("SignedOnMonth", DateTime.Now.ToString("MMMM"))
							.WithField("SignedOnYear", DateTime.Now.ToString("yy"));

				var materialChanges = SplitToLines(existingRental.MaterialChanges, 15).ToList();
				//addendum
				for (int i = 0; i < 4 && i < materialChanges.Count(); i++)
				{
					var materialChangesLine = materialChanges[i];
					agreement.WithField($"MaterialChanges{i + 1}", materialChangesLine);
				}

				//initials page 1
				agreement.WithImage(landlordWitness1Initial, 0, 90, 60, 20, 20, true, true)
				.WithImage(landlordWitness2Initial, 0, 110, 60, 20, 20, true, true)
				.WithImage(landlordInitial, 0, 130, 60, 20, 20, true, true)

				//initials page 2
				.WithImage(landlordWitness1Initial, 1, 90, 60, 20, 20, true, true)
				.WithImage(landlordWitness2Initial, 1, 110, 60, 20, 20, true, true)
				.WithImage(landlordInitial, 1, 130, 60, 20, 20, true, true)

				//landlord signatures and witnesses
				.WithImage(landlordSignature, 1, 80, 420, 40, 40)
				.WithImage(landlordWitness1Signature, 1, 450, 420, 40, 40)
				.WithImage(landlordWitness2Signature, 1, 450, 480, 40, 40);

				var agreementData = agreement.Process();

				UploadDownload.WithPath("existing_rental_forms");

				UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
				{
					Action = Web.Models.Common.CrudAction.Create,
					ContentType = "application/pdf",
					DownloadType = Core.Domain.Media.DownloadType.Document,
					Extension = "pdf",
					FileName = $"{existingRental.ExistingRentalGuid} Notice of termination or renewal of fixed term lease_{dateStamp}.pdf",
					Key = $"{existingRental.ExistingRentalGuid} Notice of termination or renewal of fixed term lease_{dateStamp}.pdf",
					MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
					Data = agreementData,
					Size = agreementData.Length
				});

				var downloads = UploadDownload.Save();

				CreateExistingRentalForm.New(ExistingRentalFormName.RenewalTermination, existingRental.ExistingRentalId, downloads.FirstOrDefault().DownloadId).Create();
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

		#endregion
	}
}
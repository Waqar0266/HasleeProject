using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
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
	public class CompleteExistingRentalLandlordSignatureController : BaseController
	{
		#region Private Properties 

		// Services
		private IUpdateExistingRentalService UpdateExistingRentalService { get; }
		private IUploadPictureService UploadPicture { get; }
		private IUploadDownloadService UploadDownload { get; }
		private ILogoutService LogoutService { get; }
		private ISendMail SendMail { get; }
		private IGetExistingRentalService GetExistingRental { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteExistingRentalLandlordSignatureController
		(
			//Services
			IUpdateExistingRentalService updateExistingRentalService,
			IUploadPictureService uploadPicture,
			IUploadDownloadService uploadDownload,
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
			LogoutService = logoutService;
			SendMail = sendMail;
			GetExistingRental = getExistingRental;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/existing-rental/l/complete-witness-signature")]
		public ActionResult CompleteLandlordWitnessSignature(string hash)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/existing-rental/l/complete-witness-signature?hash={hash}");
			}

			var decodedHash = Encoding.UTF8.GetString(Convert.FromBase64String(hash));

			var existingRentalGuid = decodedHash.Split(';')[0];
			var witnessNumber = Int32.Parse(decodedHash.Split(';')[1]);

			var existingRental = GetExistingRental[existingRentalGuid].Get();

			if (witnessNumber == 1 && existingRental.LandlordWitness1SignatureId.HasValue) return Redirect("/account/existing-rental/l/complete-witness-signature/success");
			if (witnessNumber == 2 && existingRental.LandlordWitness2SignatureId.HasValue) return Redirect("/account/existing-rental/l/complete-witness-signature/success");

			var model = new CompleteExistingRentalWitnessLandlordSignature
			{
				UniqueId = existingRentalGuid,
				WitnessNumber = witnessNumber,
				ExistingRentalId = existingRental.ExistingRentalId
			};

			ViewBag.Title = "Complete Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteWitnessSignature", model);

			// Default
			return View("../ExistingRentals/CompleteWitnessSignature", model);
		}

		[HttpGet, Route("account/existing-rental/l/complete-witness-signature/success")]
		public ActionResult CompleteLandlordWitnessSignatureSuccess()
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/existing-rental/complete-witness-signature/success");
			}

			ViewBag.Title = "Completed Witness Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteWitnessSignatureSuccess");

			// Default
			return View("../ExistingRentals/CompleteWitnessSignatureSuccess");
		}

		[HttpPost, Route("account/existing-rental/l/complete-witness-signature")]
		public ActionResult CompleteLandlordWitnessSignature(CompleteExistingRentalWitnessLandlordSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var existingRental = GetExistingRental[model.ExistingRentalId].Get();

					//add the signatures
					UploadPicture.WithPath($"signatures/existing-rental/{existingRental.ExistingRentalId}");

					var signatureData = RemoveWhitespace(model.Signature);

					string name = model.WitnessNumber == 1 ? existingRental.LandlordWitness1Name : existingRental.LandlordWitness2Name;
					string surname = model.WitnessNumber == 2 ? existingRental.LandlordWitness1Surname : existingRental.LandlordWitness2Surname;

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
						.Set(a => a.ModifiedOn, DateTime.Now)
						.Set(a => a.LandlordWitness1SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
						.Update();
					}

					if (model.WitnessNumber == 2)
					{
						success = UpdateExistingRentalService[existingRental.ExistingRentalId]
						.Set(a => a.LandlordWitness2SignatureId, pictures.FirstOrDefault(p => p.Name == $"{name.ToLower().Replace(" ", "-")}_{surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
						.Set(a => a.ModifiedOn, DateTime.Now)
						.Update();
					}

					existingRental = GetExistingRental[model.ExistingRentalId].Get();

					// Success
					if (success)
					{
						//verify if landlord witnesses signed
						if (existingRental.LandlordWitness1SignatureId.HasValue && existingRental.LandlordWitness2SignatureId.HasValue)
						{
							SendAgentSignatureEmail(existingRental.Rental.Agent.Person.Email, existingRental.ExistingRentalId);

							UpdateExistingRentalService[existingRental.ExistingRentalId]
							.Set(a => a.ModifiedOn, DateTime.Now)
							.Set(a => a.ExistingRentalStatus, ExistingRentalStatus.PendingAgentSignature)
							.Update();
						}

						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect($"/account/existing-rental/l/complete-witness-signature/success");
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

			ViewBag.Title = "Complete Landlord Witness Signature";

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../ExistingRentals/CompleteWitnessSignature", model);

			// Default
			return View("../ExistingRentals/CompleteWitnessSignature", model);
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

		private bool SendAgentSignatureEmail(string email, int existingRentalId)
		{
			var url = $"account/existing-rental/emails/agent-signature-email?existingRentalId={existingRentalId}";

			SendMail.WithUrlBody(url).WithRecipient(email);

			return SendMail.Send("New Listing - Agent Signature");
		}

		#endregion
	}
}
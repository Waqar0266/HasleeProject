using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Agents;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	public class CompleteAgentController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }

		// Services
		private IUpdateAgentService UpdateAgentService { get; }
		private IUploadPictureService UploadPicture { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public CompleteAgentController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,

			//Services
			IUpdateAgentService updateAgentService,
			IUploadPictureService uploadPicture,

			//Other
			IWebHelper webHelper
		)
		{
			//Repos
			AgentRepo = agentRepo;

			// Services
			UpdateAgentService = updateAgentService;
			UploadPicture = uploadPicture;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/agent/complete-registration")]
		public ActionResult CompleteRegistration(string id)
		{
			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == id.ToLower());

			if (agent.AgentStatus == AgentStatus.PendingDocumentation) return Redirect($"/account/agent/complete-documentation?id={id}");

			var model = new CompleteAgent
			{
				AgentGuid = id,
				AgentId = agent.AgentId,
				Title = GetTempData(agent.TempData).Split(';')[0],
				Name = GetTempData(agent.TempData).Split(';')[1],
				Surname = GetTempData(agent.TempData).Split(';')[2],
				Email = GetTempData(agent.TempData).Split(';')[3],
				Mobile = GetTempData(agent.TempData).Split(';')[4],
				IdNumber = agent.IdNumber,
				AgentStatus = agent.AgentStatus
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteRegistration", model);

			// Default
			return View("../Agents/CompleteRegistration", model);
		}

		[HttpPost, Route("account/agent/complete-registration")]
		public ActionResult CompleteRegistration(CompleteAgent model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == model.AgentGuid.ToLower());
					if (agent.AgentStatus == AgentStatus.PendingDocumentation) return Redirect($"/account/agent/complete-documentation?id={model.AgentGuid}");

					var personId = 0;

					var residentialAddress = new Address()
					{
						Type = AddressType.Residential,
						Address1 = model.ResidentialAddress1,
						Address2 = model.ResidentialAddress2,
						Address3 = model.ResidentialAddress3,
						Code = model.ResidentialAddressCode,
						Country = model.ResidentialAddressCountry,
						RegionName = model.ResidentialAddressProvince,
						Town = model.ResidentialAddressTown
					};

					var postalAddress = new Address()
					{
						Type = AddressType.Postal,
						Address1 = model.PostalAddress1,
						Address2 = model.PostalAddress2,
						Address3 = model.PostalAddress3,
						Code = model.PostalAddressCode,
						Country = model.PostalAddressCountry,
						RegionName = model.PostalAddressProvince,
						Town = model.PostalAddressTown
					};

					//add the signatures
					UploadPicture.WithPath("signatures");

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = RemoveWhitespace(model.Signature),
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_signature.png",
						Name = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_signature.png",
						MimeType = "image/png",
						AlternateText = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_signature.jpg"
					});

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = RemoveWhitespace(model.Initials),
						Format = Core.Domain.Media.PictureFormat.Png,
						Key = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_initial.png",
						Name = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_initial.png",
						MimeType = "image/png",
						AlternateText = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_initial"
					});

					var pictures = UploadPicture.Save();

					return View("../Agents/CompleteRegistration", model);

					var success = UpdateAgentService.WithAgentId(model.AgentId)
					.Set(a => a.AgentStatus, AgentStatus.PendingDocumentation)
					.Set(a => a.Convicted, model.Convicted)
					.Set(a => a.Dismissed, model.Dismissed)
					.Set(a => a.EaabReference, model.EaabReference)
					.Set(a => a.Ffc, !String.IsNullOrEmpty(model.FfcNumber))
					.Set(a => a.FfcIssueDate, model.FfcIssueDate)
					.Set(a => a.FfcNumber, model.FfcNumber)
					.Set(a => a.IdNumber, model.IdNumber)
					.Set(a => a.Insolvent, model.Insolvent)
					.Set(a => a.Nationality, model.Nationality)
					.Set(a => a.PersonId, personId)
					.Set(a => a.PreviousEmployer, model.PreviousEmployer)
					.Set(a => a.Race, model.Race)
					.Set(a => a.Withdrawn, model.Withdrawn)
					.Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_signature"))
					.Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_initial"))
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

						// Default
						return Redirect($"/account/agent/complete-registration?id={model.AgentGuid}");
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

			//if (CreateAgentService.HasWarnings) CreateAgentService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				//Message = CreateAgentService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteRegistration", model);

			// Default
			return View("../Agents/CompleteRegistration", model);
		}

		#endregion

		#region Private Methods

		private void PrepViewBags()
		{
			ViewBag.Title = "Complete Agent Registration";

			ViewBag.Titles = new List<string> { "Mr", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
			ViewBag.Races = new List<string> { "African", "White", "Coloured", "Indian", "Other" };
			ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
		}

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		public static byte[] RemoveWhitespace(string base64)
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

					if (bmp.GetPixel(i, row).A != 0)
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

					if (bmp.GetPixel(col, i).A != 0)
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
				return target.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
			}
			catch (Exception ex)
			{
				throw new Exception(
				  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
				  ex);
			}
		}

		#endregion
	}
}
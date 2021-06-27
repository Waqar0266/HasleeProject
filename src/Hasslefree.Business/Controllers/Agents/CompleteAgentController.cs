using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Data;
using Hasslefree.Services.AgentForms;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Common;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private IUploadDownloadService UploadDownload { get; }
		private IFillFormService FillForm { get; }
		private IGetFirmService GetFirmService { get; }
		private ICreateAgentFormService CreateAgentForm { get; }
		private ICreatePersonService CreatePerson { get; }

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
			IUploadDownloadService uploadDownload,
			IFillFormService fillForm,
			IGetFirmService getFirmService,
			ICreateAgentFormService createAgentForm,
			ICreatePersonService createPerson,

			//Other
			IWebHelper webHelper
		)
		{
			//Repos
			AgentRepo = agentRepo;

			// Services
			UpdateAgentService = updateAgentService;
			UploadPicture = uploadPicture;
			UploadDownload = uploadDownload;
			FillForm = fillForm;
			GetFirmService = getFirmService;
			CreateAgentForm = createAgentForm;
			CreatePerson = createPerson;

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

					//create the person
					CreatePerson.New(model.Name, "", model.Surname, model.Email, Titles.Mr, null, model.Gender, CalculateDateOfBirth(model.IdNumber)).WithPassword(model.Password, "").Create();

					var personId = CreatePerson.PersonId;

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

					var signatureData = RemoveWhitespace(model.Signature);

					UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						File = signatureData,
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

					var firmSettings = GetFirmService.Get();

					var formData = FillForm.Prepare("Individual_estate_agent_re_registration_form_1475180699.pdf")
					.WithCheckbox("Mr", model.Title.ToLower() == "mr")
					.WithCheckbox("Miss", model.Title.ToLower() == "miss")
					.WithCheckbox("Mrs", model.Title.ToLower() == "mrs")
					.WithCheckbox("Advocate", model.Title.ToLower() == "advocate")
					.WithCheckbox("Professor", model.Title.ToLower() == "professor")
					.WithCheckbox("Doctor", model.Title.ToLower() == "doctor")
					.WithField("Other Title", new List<string>() { "mr", "miss", "mrs", "advocate", "professor", "doctor" }.Contains(model.Title.ToLower()) ? "" : model.Title)
					.WithCheckbox("Male", model.Gender == Gender.Male)
					.WithCheckbox("Female", model.Gender == Gender.Female)
					.WithCheckbox("African", model.Race.ToLower() == "african")
					.WithCheckbox("White", model.Race.ToLower() == "white")
					.WithCheckbox("Coloured", model.Race.ToLower() == "coloured")
					.WithCheckbox("Indian", model.Race.ToLower() == "indian")
					.WithField("Other Race", new List<string>() { "african", "white", "coloured", "indian" }.Contains(model.Race.ToLower()) ? "" : model.Race)
					.WithField("Surname", model.Surname)
					.WithField("First Names", model.Name)
					.WithField("Identity No", $"   {model.IdNumber[0]}            {model.IdNumber[1]}             {model.IdNumber[2]}               {model.IdNumber[3]}           {model.IdNumber[4]}        {model.IdNumber[5]}          {model.IdNumber[6]}           {model.IdNumber[7]}          {model.IdNumber[8]}           {model.IdNumber[9]}         {model.IdNumber[10]}         {model.IdNumber[11]}        {model.IdNumber[12]}")
					.WithField("Date of Birth", CalculateDateOfBirth(model.IdNumber).ToString("yyyy/MM/dd"))
					.WithCheckbox("South African Citizen No", model.Nationality.ToLower() != "south african")
					.WithCheckbox("South African Citizen Yes", model.Nationality.ToLower() == "south african")
					.WithField("Nationality", model.Nationality.ToLower() != "south african" ? model.Nationality : "")
					.WithField("Residential Address 1", model.ResidentialAddress1)
					.WithField("Residential Address 2", model.ResidentialAddress2)
					.WithField("Residential Address 3", model.ResidentialAddress3)
					.WithField("Residential Address Code", model.ResidentialAddressCode)
					.WithField("Postal Address 1", model.PostalAddress1)
					.WithField("Postal Address 2", model.PostalAddress2)
					.WithField("Postal Address 3", model.PostalAddress3)
					.WithField("Postal Address Code", model.PostalAddressCode)
					.WithCheckbox("Eastern Cape", model.ResidentialAddressProvince.ToLower() == "eastern cape")
					.WithCheckbox("Free State", model.ResidentialAddressProvince.ToLower() == "free state")
					.WithCheckbox("Gauteng", model.ResidentialAddressProvince.ToLower() == "gauteng")
					.WithCheckbox("Kwazulu Natal", model.ResidentialAddressProvince.ToLower() == "kawazulu natal")
					.WithCheckbox("Limpopo", model.ResidentialAddressProvince.ToLower() == "limpopo")
					.WithCheckbox("Mpumalanga", model.ResidentialAddressProvince.ToLower() == "mpumalanga")
					.WithCheckbox("North West", model.ResidentialAddressProvince.ToLower() == "north west")
					.WithCheckbox("Northern Cape", model.ResidentialAddressProvince.ToLower() == "northern cape")
					.WithCheckbox("Western Cape", model.ResidentialAddressProvince.ToLower() == "western cape")
					.WithField("Tel No", model.Phone)
					.WithField("Cellphone No", model.Mobile)
					.WithField("Fax No", model.Fax)
					.WithField("Email address", model.Email)
					.WithCheckbox("Full Agent", agent.AgentType == AgentType.FullStatus)
					.WithCheckbox("Intern Agent", agent.AgentType == AgentType.Intern)
					.WithField("Firm Business Name 1", firmSettings.BusinessName)
					.WithField("Firm Trade Name 1", firmSettings.TradeName)
					.WithField("Firm Physical Address 1", firmSettings.PhysicalAddress1)
					.WithField("Firm Physical Address 2", firmSettings.PhysicalAddress2)
					.WithField("Firm Physical Address 3", firmSettings.PhysicalAddress3)
					.WithField("Firm Physical Address Code", firmSettings.PhysicalAddressCode)
					.WithField("Firm Postal Address 1", firmSettings.PostalAddress1)
					.WithField("Firm Postal Address 2", firmSettings.PostalAddress2)
					.WithField("Firm Postal Address 3", firmSettings.PostalAddress3)
					.WithField("Firm Postal Address Code", firmSettings.PostalAddressCode)
					.WithField("Firm Tel No", firmSettings.Phone)
					.WithField("Firm Fax No", firmSettings.Fax)
					.WithField("Firm Email", firmSettings.Email)
					.WithField("Firm Reference No", firmSettings.ReferenceNumber)
					.WithField("Previous Employer", model.PreviousEmployer)
					.WithCheckbox("FFC Yes", !String.IsNullOrEmpty(model.FfcNumber))
					.WithCheckbox("FFC No", String.IsNullOrEmpty(model.FfcNumber))
					.WithField("FFC Number", model.FfcNumber)
					.WithField("Date of Issue", model.FfcIssueDate.HasValue ? model.FfcIssueDate.Value.ToString("yyyy/MM/dd") : "")
					.WithField("EAAB Ref", model.EaabReference)
					.WithCheckbox("Dismiss No", !model.Dismissed)
					.WithCheckbox("Dismiss Yes", model.Dismissed)
					.WithCheckbox("Convicted No", !model.Convicted)
					.WithCheckbox("Convicted Yes", model.Convicted)
					.WithCheckbox("Insolvent No", !model.Insolvent)
					.WithCheckbox("Insolvent Yes", model.Insolvent)
					.WithCheckbox("Cer Withdrawn No", !model.Withdrawn)
					.WithCheckbox("Cer Withdrawn Yes", model.Withdrawn)
					.WithField("Date", DateTime.Now.ToString("yyyy/MM/dd"))
					.WithImage(signatureData, 0, 190, 160, 45, 45, true, true)
					.Process();

					var downloads = UploadDownload.WithPath("forms").Add(new Web.Models.Media.Downloads.DownloadModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						ContentType = "application/pdf",
						DownloadType = Core.Domain.Media.DownloadType.Document,
						Extension = "pdf",
						FileName = $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_eaab_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
						Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_eaab_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
						MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
						Data = formData,
						Size = formData.Length
					}).Save();

					CreateAgentForm.New(FormName.Eaab, agent.AgentId, downloads.FirstOrDefault().DownloadId).Create();

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
						return Redirect($"/account/agent/complete-documentation?id={model.AgentGuid}");
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

			if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
			if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

			ModelState.AddModelError("", errors);

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = errors ?? "Unexpected error has occurred."
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

			ViewBag.Titles = new List<string> { "Mr", "Miss", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
			ViewBag.Races = new List<string> { "African", "White", "Coloured", "Indian", "Other" };
			ViewBag.Provinces = new List<string> { "Eastern Cape", "Free State", "Gauteng", "KwaZulu Natal", "Limpopo", "Mpumalanga", "North West", "Northern Cape", "Western Cape" };
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

		#endregion
	}
}
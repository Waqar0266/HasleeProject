using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.AgentForms;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AgentFilter]
	public class CompleteAgentSignatureController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<AgentAddress> AgentAddressRepo { get; }
		private IReadOnlyRepository<AgentDocumentation> AgentDocumentationRepo { get; }
		private IReadOnlyRepository<AgentForm> AgentFormRepo { get; }
		private IReadOnlyRepository<Download> DownloadRepo { get; }

		// Services
		private IUpdateAgentService UpdateAgentService { get; }
		private IUploadPictureService UploadPicture { get; }
		private IUploadDownloadService UploadDownload { get; }
		private IFillFormService FillForm { get; }
		private IGetFirmService GetFirmService { get; }
		private ICreateAgentFormService CreateAgentForm { get; }
		private ILogoutService LogoutService { get; }
		private ISendMail SendMail { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteAgentSignatureController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<AgentAddress> agentAddressRepo,
			IReadOnlyRepository<AgentDocumentation> agentDocumentationRepo,
			IReadOnlyRepository<AgentForm> agentFormRepo,
			IReadOnlyRepository<Download> downloadRepo,

			//Services
			IUpdateAgentService updateAgentService,
			IUploadPictureService uploadPicture,
			IUploadDownloadService uploadDownload,
			IFillFormService fillForm,
			IGetFirmService getFirmService,
			ICreateAgentFormService createAgentForm,
			ILogoutService logoutService,
			ISendMail sendMail,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			AgentRepo = agentRepo;
			PersonRepo = personRepo;
			AgentAddressRepo = agentAddressRepo;
			AgentDocumentationRepo = agentDocumentationRepo;
			AgentFormRepo = agentFormRepo;
			DownloadRepo = downloadRepo;

			// Services
			UpdateAgentService = updateAgentService;
			UploadPicture = uploadPicture;
			UploadDownload = uploadDownload;
			FillForm = fillForm;
			GetFirmService = getFirmService;
			CreateAgentForm = createAgentForm;
			LogoutService = logoutService;
			SendMail = sendMail;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/agent/complete-signature")]
		[AccessControlFilter]
		public ActionResult CompleteSignature(string hash)
		{
			string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == decodedHash);

			var model = new CompleteAgentSignature
			{
				AgentGuid = decodedHash,
				Name = GetTempData(agent.TempData).Split(';')[1],
				Surname = GetTempData(agent.TempData).Split(';')[2]
			};

			ViewBag.Title = "Complete Agent Signature";

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteSignature", model);

			// Default
			return View("../Agents/CompleteSignature", model);
		}

		[HttpPost, Route("account/agent/complete-signature")]
		[AccessControlFilter]
		public ActionResult CompleteSignature(CompleteAgentSignature model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == model.AgentGuid.ToLower());

					var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == agent.PersonId);

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

					var success = UpdateAgentService.WithAgentId(agent.AgentId)
					.Set(a => a.AgentStatus, AgentStatus.PendingEaabRegistration)
					.Set(a => a.SignedAt, model.SignedAtSignature)
					.Set(a => a.SignedOn, DateTime.Now)
					.Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_signature.png").PictureId)
					.Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_initial.png").PictureId)
					.Update();

					var firmSettings = GetFirmService.Get();

					//get the addresses
					var agentAddresses = AgentAddressRepo.Table.Where(a => a.AgentId == agent.AgentId).Select(a => a.Address).ToList();
					var postal = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Postal);
					var residential = agentAddresses.FirstOrDefault(a => a.Type == AddressType.Residential);

					var agentContractData = FillForm.Prepare("Agent Contract.pdf")
					.WithField("Title", $"{person.FirstName.ToUpper()} {person.Surname.ToUpper()} ({person.IdNumber})")
					.WithField("Date", DateTime.Now.ToString("dd/MM/yyyy"))
					.WithField("AgentNameSurname", $"{person.FirstName} {person.Surname}")
					.WithField("AppointmentDate", DateTime.Now.ToString("dd/MM/yyyy"))
					.WithField("SignedAt", model.SignedAtSignature)
					.WithField("SignedDay", DateTime.Now.ToString("dd"))
					.WithField("SignedMonthYear", DateTime.Now.ToString("MMMM") + " " + DateTime.Now.ToString("yyyy"))
					.WithImage(signatureData, 6, 80, 300, 50, 50)
					.Process();

					var agentRegistrationFormData = FillForm.Prepare("Individual_estate_agent_re_registration_form_1475180699.pdf")
					.WithCheckbox("Mr", person.Title.ToLower() == "mr")
					.WithCheckbox("Miss", person.Title.ToLower() == "miss")
					.WithCheckbox("Mrs", person.Title.ToLower() == "mrs")
					.WithCheckbox("Advocate", person.Title.ToLower() == "advocate")
					.WithCheckbox("Professor", person.Title.ToLower() == "professor")
					.WithCheckbox("Doctor", person.Title.ToLower() == "doctor")
					.WithField("Other Title", new List<string>() { "mr", "miss", "mrs", "advocate", "professor", "doctor" }.Contains(person.Title.ToLower()) ? "" : person.Title)
					.WithCheckbox("Male", person.Gender == Gender.Male)
					.WithCheckbox("Female", person.Gender == Gender.Female)
					.WithCheckbox("African", agent.Race.ToLower() == "african")
					.WithCheckbox("White", agent.Race.ToLower() == "white")
					.WithCheckbox("Coloured", agent.Race.ToLower() == "coloured")
					.WithCheckbox("Indian", agent.Race.ToLower() == "indian")
					.WithField("Other Race", new List<string>() { "african", "white", "coloured", "indian" }.Contains(agent.Race.ToLower()) ? "" : agent.Race)
					.WithField("Surname", person.Surname)
					.WithField("First Names", person.FirstName)
					.WithField("Id_1", $"{person.IdNumber[0]}")
					.WithField("Id_2", $"{person.IdNumber[1]}")
					.WithField("Id_3", $"{person.IdNumber[2]}")
					.WithField("Id_4", $"{person.IdNumber[3]}")
					.WithField("Id_5", $"{person.IdNumber[4]}")
					.WithField("Id_6", $"{person.IdNumber[5]}")
					.WithField("Id_7", $"{person.IdNumber[6]}")
					.WithField("Id_8", $"{person.IdNumber[7]}")
					.WithField("Id_9", $"{person.IdNumber[8]}")
					.WithField("Id_10", $"{person.IdNumber[9]}")
					.WithField("Id_11", $"{person.IdNumber[10]}")
					.WithField("Id_12", $"{person.IdNumber[11]}")
					.WithField("Id_13", $"{person.IdNumber[12]}")
					.WithField("Date of Birth", CalculateDateOfBirth(person.IdNumber).ToString("yyyy/MM/dd"))
					.WithCheckbox("South African Citizen No", agent.Nationality.ToLower() != "south african")
					.WithCheckbox("South African Citizen Yes", agent.Nationality.ToLower() == "south african")
					.WithField("Nationality", agent.Nationality.ToLower() != "south african" ? agent.Nationality : "")
					.WithField("Residential Address 1", residential.Address1)
					.WithField("Residential Address 2", residential.Address2)
					.WithField("Residential Address 3", residential.Address3)
					.WithField("Residential Address Code", residential.Code)
					.WithField("Postal Address 1", postal.Address1)
					.WithField("Postal Address 2", postal.Address2)
					.WithField("Postal Address 3", postal.Address3)
					.WithField("Postal Address Code", postal.Code)
					.WithCheckbox("Eastern Cape", residential.RegionName.ToLower() == "eastern cape")
					.WithCheckbox("Free State", residential.RegionName.ToLower() == "free state")
					.WithCheckbox("Gauteng", residential.RegionName.ToLower() == "gauteng")
					.WithCheckbox("Kwazulu Natal", residential.RegionName.ToLower() == "kawazulu natal")
					.WithCheckbox("Limpopo", residential.RegionName.ToLower() == "limpopo")
					.WithCheckbox("Mpumalanga", residential.RegionName.ToLower() == "mpumalanga")
					.WithCheckbox("North West", residential.RegionName.ToLower() == "north west")
					.WithCheckbox("Northern Cape", residential.RegionName.ToLower() == "northern cape")
					.WithCheckbox("Western Cape", residential.RegionName.ToLower() == "western cape")
					.WithField("Tel No", person.Phone)
					.WithField("Cellphone No", person.Mobile)
					.WithField("Fax No", person.Fax)
					.WithField("Email address", person.Email)
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
					.WithField("AiNumber", firmSettings.AiNumber)
					.WithField("Previous Employer", agent.PreviousEmployer)
					.WithCheckbox("FFC Yes", !String.IsNullOrEmpty(agent.FfcNumber))
					.WithCheckbox("FFC No", String.IsNullOrEmpty(agent.FfcNumber))
					.WithField("FFC Number", agent.FfcNumber)
					.WithField("Date of Issue", agent.FfcIssueDate.HasValue ? agent.FfcIssueDate.Value.ToString("yyyy/MM/dd") : "")
					.WithField("EAAB Ref", agent.EaabReference)
					.WithCheckbox("Dismiss No", !agent.Dismissed)
					.WithCheckbox("Dismiss Yes", agent.Dismissed)
					.WithCheckbox("Convicted No", !agent.Convicted)
					.WithCheckbox("Convicted Yes", agent.Convicted)
					.WithCheckbox("Insolvent No", !agent.Insolvent)
					.WithCheckbox("Insolvent Yes", agent.Insolvent)
					.WithCheckbox("Cer Withdrawn No", !agent.Withdrawn)
					.WithCheckbox("Cer Withdrawn Yes", agent.Withdrawn)
					.WithField("Date", DateTime.Now.ToString("yyyy/MM/dd"))
					.WithImage(signatureData, 0, 190, 160, 45, 45, true, true)
					.Process();

					var agentAppointmentLetterData = FillForm.Prepare("Appointment letter.pdf")
					.WithField("AgentNameSurname", $"{person.FirstName} {person.Surname}")
					.WithField("IdNumber", person.IdNumber)
					.WithField("SignedAt", model.SignedAtSignature)
					.WithField("SignedDay", DateTime.Now.ToString("dd"))
					.WithField("SignedMonthYear", DateTime.Now.ToString("MMMM") + " " + DateTime.Now.ToString("yyyy"))
					.Process();

					UploadDownload.WithPath("forms");

					var dateStamp = DateTime.Now.ToString("yyyyMMddHHmm");

					UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						ContentType = "application/pdf",
						DownloadType = Core.Domain.Media.DownloadType.Document,
						Extension = "pdf",
						FileName = $"{model.Name} {model.Surname} EAAB Registration Form_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
						Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_eaab_{dateStamp}.pdf",
						MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
						Data = agentRegistrationFormData,
						Size = agentRegistrationFormData.Length
					});

					UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						ContentType = "application/pdf",
						DownloadType = Core.Domain.Media.DownloadType.Document,
						Extension = "pdf",
						FileName = $"{model.Name} {model.Surname} Agent Contract_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
						Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_agent_contract_{dateStamp}.pdf",
						MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
						Data = agentContractData,
						Size = agentContractData.Length
					});

					UploadDownload.Add(new Web.Models.Media.Downloads.DownloadModel()
					{
						Action = Web.Models.Common.CrudAction.Create,
						ContentType = "application/pdf",
						DownloadType = Core.Domain.Media.DownloadType.Document,
						Extension = "pdf",
						FileName = $"{model.Name} {model.Surname} Appointment Letter_{DateTime.Now.ToString("yyyyMMddHHmm")}.pdf",
						Key = $"{agent.AgentGuid}/{model.Name.ToLower().Replace(" ", "-")}_{model.Surname.ToLower().Replace(" ", "-")}_appointment_letter_{dateStamp}.pdf",
						MediaStorage = Core.Domain.Media.MediaStorage.Cloud,
						Data = agentAppointmentLetterData,
						Size = agentAppointmentLetterData.Length
					});

					var downloads = UploadDownload.Save();

					success = CreateAgentForm.New(FormName.Eaab, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} EAAB Registration Form_{dateStamp}.pdf").DownloadId).Create();
					success = CreateAgentForm.New(FormName.AgentContract, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Agent Contract_{dateStamp}.pdf").DownloadId).Create();
					success = CreateAgentForm.New(FormName.AppointmentLetter, agent.AgentId, downloads.FirstOrDefault(d => d.FileName == $"{model.Name} {model.Surname} Appointment Letter_{dateStamp}.pdf").DownloadId).Create();

					success = SendDirectorEmail(person.Email, agent.AgentId);

					// Success
					if (success)
					{
						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(agent.AgentGuid.ToString().ToLower()));

						// Default
						return Redirect($"/account/agent/complete-eaab?hash={hash}");
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

			if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
			if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

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

		private bool SendDirectorEmail(string email, int agentId)
		{
			var url = $"account/agent/emails/director-email?agentId={agentId}";

			var agentForms = AgentFormRepo.Table.Where(a => a.AgentId == agentId).Select(a => a.DownloadId).ToList();

			var ids = agentForms;

			var downloads = DownloadRepo.Table.Where(a => ids.Contains(a.DownloadId)).ToList();

			var attachments = new List<Attachment>();

			SendMail.WithUrlBody(url).WithRecipient(email);

			foreach (var download in downloads)
			{
				var data = new WebClient().DownloadData(download.RelativeFolderPath);
				SendMail.WithAttachment(new Attachment(new MemoryStream(data), download.FileName, download.ContentType));
			}

			return SendMail.Send("Agent Profile Review");
		}

		#endregion
	}
}
using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.AgentForms;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Common;
using Hasslefree.Services.Forms;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AgentFilter]
	public class CompleteAgentController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IDataRepository<AgentAddress> AgentAddressRepo { get; }

		// Services
		private IUpdateAgentService UpdateAgentService { get; }
		private IUploadPictureService UploadPicture { get; }
		private IUploadDownloadService UploadDownload { get; }
		private IFillFormService FillForm { get; }
		private IGetFirmService GetFirmService { get; }
		private ICreateAgentFormService CreateAgentForm { get; }
		private ICreatePersonService CreatePerson { get; }
		private ILogoutService LogoutService { get; }
		private ICountryQueryService Countries { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteAgentController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,
			IDataRepository<AgentAddress> agentAddressRepo,

			//Services
			IUpdateAgentService updateAgentService,
			IUploadPictureService uploadPicture,
			IUploadDownloadService uploadDownload,
			IFillFormService fillForm,
			IGetFirmService getFirmService,
			ICreateAgentFormService createAgentForm,
			ICreatePersonService createPerson,
			ILogoutService logoutService,
			ICountryQueryService countries,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			AgentRepo = agentRepo;
			AgentAddressRepo = agentAddressRepo;

			// Services
			UpdateAgentService = updateAgentService;
			UploadPicture = uploadPicture;
			UploadDownload = uploadDownload;
			FillForm = fillForm;
			GetFirmService = getFirmService;
			CreateAgentForm = createAgentForm;
			CreatePerson = createPerson;
			LogoutService = logoutService;
			Countries = countries;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/agent/complete-registration")]
		public ActionResult CompleteRegistration(string id)
		{
			if (SessionManager.IsLoggedIn())
			{
				LogoutService.Logout();
				return Redirect($"/account/agent/complete-registration?id={id}");
			}

			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == id.ToLower());

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

					//create the person
					CreatePerson
					.New(model.Name, "", model.Surname, model.Email, Titles.Mr, null, model.Gender, CalculateDateOfBirth(model.IdNumber))
					.WithContactDetails(model.Phone, model.Fax, model.Mobile)
					.WithPassword(model.Password, "")
					.WithSecurityGroup("Agent")
					.Create();

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

					//add the addresses
					AgentAddressRepo.Insert(new AgentAddress()
					{
						Address = residentialAddress,
						AgentId = agent.AgentId
					});

					AgentAddressRepo.Insert(new AgentAddress()
					{
						Address = postalAddress,
						AgentId = agent.AgentId
					});

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

			ViewBag.Countries = Countries.Get().Select(c => c.Name).ToList();
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
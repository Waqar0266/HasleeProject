﻿using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Logging;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
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

			//Other
			IWebHelper webHelper
		)
		{
			//Repos
			AgentRepo = agentRepo;

			// Services
			UpdateAgentService = updateAgentService;

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

		#endregion
	}
}
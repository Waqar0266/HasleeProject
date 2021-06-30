using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Security;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	public class CompleteAgentEaabController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }

		// Services
		private IUpdateAgentService UpdateAgentService { get; }
		private ILogoutService LogoutService { get; }
		private ISecurityService SecurityService { get; }

		// Other
		private IWebHelper WebHelper { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Constructor

		public CompleteAgentEaabController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,

			//Services
			IUpdateAgentService updateAgentService,
			ILogoutService logoutService,
			ISecurityService securityService,

			//Other
			IWebHelper webHelper,
			ISessionManager sessionManager
		)
		{
			//Repos
			AgentRepo = agentRepo;

			// Services
			UpdateAgentService = updateAgentService;
			LogoutService = logoutService;
			SecurityService = securityService;

			// Other
			WebHelper = webHelper;
			SessionManager = sessionManager;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/agent/complete-eaab")]
		[AccessControlFilter]
		public ActionResult Create(string id)
		{
			var isAgent = SecurityService.IsInSecurityGroup(Hasslefree.Web.Framework.SessionManager.Current.Login.LoginId, "Agent");
			if (!isAgent)
			{
				LogoutService.Logout();
				return Redirect($"/account/agent/complete-eaab?id={id}");
			}

			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == id.ToLower());
			if (agent.AgentStatus == AgentStatus.PendingSignature) return Redirect($"/account/agent/complete-signature?id={agent.AgentGuid}");
			if (agent.AgentStatus == AgentStatus.PendingRegistration) return Redirect($"/account/agent/complete-registration?id={agent.AgentGuid}");
			if (agent.AgentStatus == AgentStatus.PendingDocumentation) return Redirect($"/account/agent/complete-documentation?id={agent.AgentGuid}");

			var model = new CompleteAgentEaab
			{
				AgentGuid = agent.AgentGuid.ToString()
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteEaab", model);

			// Default
			return View("../Agents/CompleteEaab", model);
		}

		[HttpPost, Route("account/agent/complete-eaab")]
		public ActionResult Create(CompleteAgentEaab model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == model.AgentGuid.ToLower());
					if (agent.AgentStatus == AgentStatus.PendingSignature) return Redirect($"/account/agent/complete-signature?id={model.AgentGuid}");
					if (agent.AgentStatus == AgentStatus.PendingRegistration) return Redirect($"/account/agent/complete-registration?id={model.AgentGuid}");
					if (agent.AgentStatus == AgentStatus.PendingDocumentation) return Redirect($"/account/agent/complete-documentation?id={model.AgentGuid}");

					var success = UpdateAgentService.WithAgentId(agent.AgentId)
					.Set(a => a.EaabProofOfPaymentId, model.DownloadId)
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
						return Redirect($"/account/profile");
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
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteDocumentation", model);

			// Default
			return View("../Agents/CompleteDocumentation", model);
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
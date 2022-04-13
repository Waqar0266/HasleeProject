using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Emails;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AccessControlFilter(Permission = "Director")]
	public class UpdateAgentController : BaseController
	{
		#region Private Properties

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }

		// Services
		private IGetAgentService GetAgentService { get; }
		private IUpdateAgentService UpdateAgentService { get; }
		private ISendMail SendMail { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public UpdateAgentController
		(
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<Person> personRepo,
			IGetAgentService getAgentService,
			IUpdateAgentService updateAgentService,
			ISendMail sendMail,
			IWebHelper webHelper
		)
		{
			//Repos
			AgentRepo = agentRepo;
			PersonRepo = personRepo;

			// Services
			GetAgentService = getAgentService;
			UpdateAgentService = updateAgentService;
			SendMail = sendMail;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		[HttpGet, Route("account/agent")]
		public ActionResult Update(int agentId)
		{
			// Model
			var agent = GetAgentService[agentId];

			if (agent == null)
			{
				if (WebHelper.IsAjaxRequest())
					return Json(new { }, JsonRequestBehavior.AllowGet);

				return RedirectToAction("List", "AgentList");
			}

			var model = new AgentModel
			{
				AgentId = agent.AgentId,
				AgentType = (AgentType)Enum.Parse(typeof(AgentType), agent.AgentTypeEnum),
				AgentStatus = (AgentStatus)Enum.Parse(typeof(AgentStatus), agent.AgentStatusEnum),
				Email = agent.Email,
				Mobile = agent.Mobile,
				Name = agent.Name,
				Surname = agent.Surname,
				Title = agent.Title,
				Documents = agent.Documents,
				Forms = agent.Forms,
				EaabProofOfPayment = agent.EaabProofOfPayment
			};

			// View bags
			PrepViewBag();

			// Ajax
			if (WebHelper.IsAjaxRequest())
				return PartialView("../Agents/Update", model);

			// Default
			return View("../Agents/Update", model);
		}

		[HttpPost, Route("account/agent")]
		public ActionResult Update(int agentId, AgentModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					// Basic Properties
					var success = UpdateAgentService[agentId]
						.Set(m => m.ModifiedOn, DateTime.Now)
						.Set(m => m.AgentType, model.AgentType)
						.Update();

					// Success
					if (success)
					{
						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest())
							return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

						// Default
						return RedirectToAction("Update", "UpdateAgent", new { agentId });
					}
				}
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			// ViewBags
			PrepViewBag();

			if (UpdateAgentService.HasWarnings)
				UpdateAgentService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = UpdateAgentService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			if (WebHelper.IsAjaxRequest())
				return PartialView("../Agents/Update", model);

			// Default
			return View("../Agents/Update", model);
		}

		[HttpPost, Route("agent/approve")]
		public ActionResult Approve(int id)
		{
			try
			{
				var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == id);
				var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == agent.PersonId.Value);

				// Basic Properties
				var success = UpdateAgentService[id]
					.Set(m => m.ModifiedOn, DateTime.Now)
					.Set(m => m.AgentStatus, AgentStatus.PendingEaabRegistration)
					.Update();

				//send the approved email
				if (success) SendMail.WithUrlBody($"/account/agent/emails/approved-email?agentId={id}").Send("Agent Profile Approved", person.Email);

				// Success
				return Json(new { success = success });
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
				while (ex.InnerException != null) ex = ex.InnerException;
				ModelState.AddModelError("", ex.Message);
			}

			return Json(new { success = false });
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/agent/emails/approved-email")]
		public ActionResult Email(int agentId)
		{
			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == agentId);

			var model = new ApprovedAgentModel()
			{
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/documents"
			};

			return View("../Emails/Agent-Approved-Email", model);
		}

		#region Private 

		/// <summary>
		/// Set required view bags for action and the crud title
		/// </summary>
		private void PrepViewBag()
		{
			// Set current crud title to display
			ViewBag.Title = "Update Agent";

			ViewBag.Types = Enum.GetNames(typeof(AgentType));
		}

		#endregion
	}
}
using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AccessControlFilter(Permission = "Director")]
	public class UpdateAgentController : BaseController
	{
		#region Private Properties

		// Services
		private IGetAgentService GetAgentService { get; }
		private IUpdateAgentService UpdateAgentService { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public UpdateAgentController
		(
			IGetAgentService getAgentService,
			IUpdateAgentService updateAgentService,
			IWebHelper webHelper
		)
		{
			// Services
			GetAgentService = getAgentService;
			UpdateAgentService = updateAgentService;

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
				Email = agent.Email,
				IdNumber = agent.IdNumber,
				Mobile = agent.Mobile,
				Name = agent.Name,
				Surname = agent.Surname,
				Title = agent.Title,
				Documents = agent.Documents,
				Forms = agent.Forms
			};

			// View bags
			PrepViewBag(agentId);

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
			PrepViewBag(agentId);

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

		#region Private 

		/// <summary>
		/// Set required view bags for action and the crud title
		/// </summary>
		private void PrepViewBag(int agentId)
		{
			// Set current crud title to display
			ViewBag.Title = "Update Agent";

			ViewBag.Types = Enum.GetNames(typeof(AgentType));
		}

		#endregion
	}
}
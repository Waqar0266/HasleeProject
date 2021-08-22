using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Logging;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Services.Emails;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Agents
{
	[AccessControlFilter(Permission = "Director")]
	[FirmFilter]
	public class CreateAgentController : BaseController
	{
		#region Private Properties 

		//Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }

		// Services
		private ICreateAgentService CreateAgentService { get; }
		private ISendMail SendMail { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public CreateAgentController
		(
			//Repos
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<Person> personRepo,

			//Services
			ICreateAgentService createAgentService,
			ISendMail sendMail,

			//Other
			IWebHelper webHelper
		)
		{
			//Repos
			AgentRepo = agentRepo;
			PersonRepo = personRepo;

			// Services
			CreateAgentService = createAgentService;
			SendMail = sendMail;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		#region Actions

		[HttpGet, Route("account/add-agent")]
		public ActionResult Create()
		{
			var model = new AgentCreate
			{
			};

			PrepViewBags();

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CRUD", model);

			// Default
			return View("../Agents/CRUD", model);
		}

		[HttpPost, Route("account/add-agent")]
		public ActionResult Create(AgentCreate model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					bool success = CreateAgentService.New(model.AgentType, model.Title, model.Name, model.Surname, model.IdNumber, model.Email, model.Mobile).Create();

					// Success
					if (success)
					{
						//Send the email
						SendMail.WithUrlBody($"/account/agent/emails/invite-email?agentId={CreateAgentService.AgentId}").Send("Complete Agent Application", model.Email);

						// Ajax (+ Json)
						if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
						{
							Success = true,
							AgentId = 1,
						}, JsonRequestBehavior.AllowGet);

						// Default
						return Redirect("/account/agents");
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

			if (CreateAgentService.HasWarnings) CreateAgentService.Warnings.ForEach(w => ModelState.AddModelError("", w.Message));

			// Ajax (Json)
			if (WebHelper.IsJsonRequest()) return Json(new
			{
				Success = false,
				Message = CreateAgentService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
			}, JsonRequestBehavior.AllowGet);

			// Ajax
			if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CRUD", model);

			// Default
			return View("../Agents/CRUD", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/agent/emails/invite-email")]
		public ActionResult Email(int agentId)
		{
			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == agentId);

			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(agent.AgentGuid.ToString().ToLower()));

			var model = new InviteAgent()
			{
				Title = GetTempData(agent.TempData).Split(';')[0],
				Name = GetTempData(agent.TempData).Split(';')[1],
				Surname = GetTempData(agent.TempData).Split(';')[2],
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/agent/complete-registration?hash={hash}"
			};

			return View("../Emails/Agent-Invite-Email", model);
		}

		[HttpGet]
		[Email]
		[AllowAnonymous]
		[Route("account/agent/emails/director-email")]
		public ActionResult DirectorEmail(int agentId)
		{
			var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentId == agentId);
			var person = PersonRepo.Table.FirstOrDefault(p => p.PersonId == agent.PersonId);

			var model = new DirectorAgentComplete()
			{
				Name = person.FirstName,
				Surname = person.Surname,
				Gender = person.Gender,
				Email = person.Email,
				Mobile = person.Mobile,
				Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/agent?agentId={agent.AgentId}"
			};

			return View("../Emails/Director-Agent-Registration-Done-Email", model);
		}

		#endregion

		#region Private Methods

		private void PrepViewBags()
		{
			ViewBag.Title = "Add Agent";

			ViewBag.Types = Enum.GetNames(typeof(AgentType));
			ViewBag.Titles = new List<string> { "Mr", "Miss", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
		}

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		#endregion
	}
}
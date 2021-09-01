using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Agents;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	[AccessControlFilter]
	[AgentFilter]
	public class DocumentsController : BaseController
	{
		/* Dependencies */
		private IReadOnlyRepository<Agent> AgentRepo { get; }

		private ISessionManager SessionManager { get; }

		private IGetAgentService GetAgent { get; }

		/* CTOR */
		public DocumentsController
		(
			IReadOnlyRepository<Agent> agentRepo,
			ISessionManager sessionManager,
			IGetAgentService getAgent
		)
		{
			SessionManager = sessionManager;
			GetAgent = getAgent;
			AgentRepo = agentRepo;
		}

		/* GET */
		[HttpGet]
		[Route("account/documents")]
		public ActionResult Index()
		{
			// Get the model
			var model = GetModel();

			// Set select lists in ViewBag
			SetViewBag();

			// View
			return View("../Accounts/Documents", model);
		}

		#region Private
		private AgentGet GetModel()
		{
			// Get person from session
			var person = SessionManager.Login.Person;

			var agent = AgentRepo.Table.FirstOrDefault(a => a.PersonId == person.PersonId);

			if (agent == null) return new AgentGet();

			return GetAgent[agent.AgentId];
		}

		private void SetViewBag()
		{
			ViewBag.Title = "Documents";
		}

		#endregion
	}
}
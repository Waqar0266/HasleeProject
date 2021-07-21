using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Filters
{
	public class AgentFilter : AuthorizeAttribute
	{
		/// <summary>
		/// Session manager
		/// </summary>
		public ISessionManager SessionManager { get; set; }

		/// <summary>
		/// The Agent repo
		/// </summary>
		public IReadOnlyRepository<Agent> AgentRepo { get; set; }

		/// <summary>
		/// The Download repo
		/// </summary>
		public IReadOnlyRepository<Download> DownloadRepo { get; set; }

		#region Methods

		/// <inheritdoc />
		/// <summary>
		/// Perform a access control check on the currently logged in user
		/// </summary>
		/// <param name="filterContext"></param>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			return;
			// Do nothing when headers already sent
			if (filterContext.HttpContext.Response.HeadersWritten) return;

			// Ignore child actions
			if (filterContext.Controller.ControllerContext.IsChildAction) return;

			//Check if anonymous access is allowed on this action
			if (filterContext.ActionDescriptor.GetCustomAttributes(true).Any(a => a is AllowAnonymousAttribute)) return;

			//Get the request object
			var request = filterContext.HttpContext.Request;

			Agent agent = null;

			// Check for login
			if (!SessionManager.IsLoggedIn())
			{
				var id = request.QueryString["id"];
				if (!String.IsNullOrEmpty(id)) agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == id.ToLower());
			}
			else
				agent = AgentRepo.Table.FirstOrDefault(a => a.PersonId == SessionManager.Login.PersonId);

			// Check for agent
			if (agent == null) return;

			// Check for agent status
			if (agent.AgentStatus == AgentStatus.PendingSignature && !request.Url.AbsolutePath.Contains("agent/complete-signature"))
			{
				filterContext.Result = new RedirectResult($"/account/agent/complete-signature?id={agent.AgentGuid}");
				return;
			}

			if (agent.AgentStatus == AgentStatus.PendingRegistration && !request.Url.AbsolutePath.Contains("agent/complete-registration"))
			{
				filterContext.Result = new RedirectResult($"/account/agent/complete-registration?id={agent.AgentGuid}");
				return;
			}

			if (agent.AgentStatus == AgentStatus.PendingEaabRegistration && !request.Url.AbsolutePath.Contains("agent/complete-eaab"))
			{
				filterContext.Result = new RedirectResult($"/account/agent/complete-eaab?id={agent.AgentGuid}");
				return;
			}

			if (agent.AgentStatus == AgentStatus.PendingDocumentation && !request.Url.AbsolutePath.Contains("agent/complete-documentation"))
			{
				filterContext.Result = new RedirectResult($"/account/agent/complete-documentation?id={agent.AgentGuid}");
				return;
			}

			if (agent.AgentStatus == AgentStatus.PendingVetting && !request.Url.AbsolutePath.Contains("agent/pending-vetting"))
			{
				filterContext.Result = new RedirectResult($"/account/pending-vetting");
				return;
			}

			if (agent.AgentStatus == AgentStatus.Rejected)
			{
				filterContext.Result = new RedirectResult($"/account/logout");
				return;
			}

			//verify the eaab proof of payment date
			if (!agent.EaabProofOfPaymentId.HasValue)
			{
				filterContext.Result = new RedirectResult($"/account/agent/complete-eaab?id={agent.AgentGuid}");
				return;
			}

			var download = DownloadRepo.Table.FirstOrDefault(d => d.DownloadId == agent.EaabProofOfPaymentId.Value);
			var lastPaidDate = download.CreatedOn;

			if ((DateTime.Now - lastPaidDate).Days >= 365)
			{
				if (DateTime.Now > new DateTime(DateTime.Now.Year, 10, 30))
				{
					filterContext.Result = new RedirectResult($"/account/agent/complete-eaab?id={agent.AgentGuid}");
					return;
				}
				else
					filterContext.Controller.ViewBag.WarnEaab = true;
			}
			else
				filterContext.Controller.ViewBag.WarnEaab = false;
		}
		#endregion
	}
}

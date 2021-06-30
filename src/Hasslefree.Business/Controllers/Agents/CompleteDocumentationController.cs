using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
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
    public class CompleteDocumentationController : BaseController
    {
        #region Private Properties 

        //Repos
        private IReadOnlyRepository<Agent> AgentRepo { get; }
        private IDataRepository<AgentDocumentation> AgentDocumentation { get; }

        // Services
        private IUpdateAgentService UpdateAgentService { get; }
        private ILogoutService LogoutService { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteDocumentationController
        (
            //Repos
            IReadOnlyRepository<Agent> agentRepo,
            IDataRepository<AgentDocumentation> agentDocumentation,

            //Services
            IUpdateAgentService updateAgentService,
            ILogoutService logoutService,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            //Repos
            AgentRepo = agentRepo;
            AgentDocumentation = agentDocumentation;

            // Services
            UpdateAgentService = updateAgentService;
            LogoutService = logoutService;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/agent/complete-documentation")]
        public ActionResult CompleteDocumentation(string id)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/agent/complete-documentation?id={id}");
            }

            var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == id.ToLower());
            if (agent.AgentStatus == AgentStatus.PendingSignature) return Redirect($"/account/agent/complete-signature?id={agent.AgentGuid}");
            if (agent.AgentStatus == AgentStatus.PendingRegistration) return Redirect($"/account/agent/complete-registration?id={agent.AgentGuid}");
            if (agent.AgentStatus == AgentStatus.PendingEaabRegistration) return Redirect($"/account/agent/complete-eaab?id={agent.AgentGuid}");

            var model = new CompleteDocumentation
            {
                AgentGuid = agent.AgentGuid.ToString()
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteDocumentation", model);

            // Default
            return View("../Agents/CompleteDocumentation", model);
        }

        [HttpPost, Route("account/agent/complete-documentation")]
        public ActionResult CompleteDocumentation(CompleteDocumentation model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var agent = AgentRepo.Table.FirstOrDefault(a => a.AgentGuid.ToString().ToLower() == model.AgentGuid.ToLower());
                    if (agent.AgentStatus == AgentStatus.PendingSignature) return Redirect($"/account/agent/complete-signature?id={model.AgentGuid}");
                    if (agent.AgentStatus == AgentStatus.PendingRegistration) return Redirect($"/account/agent/complete-registration?id={model.AgentGuid}");
                    if (agent.AgentStatus == AgentStatus.PendingEaabRegistration) return Redirect($"/account/agent/complete-eaab?id={model.AgentGuid}");

                    foreach (var i in model.UploadIds.Split(','))
                    {
                        if (Int32.TryParse(i, out int id))
                        {
                            if (id > 0) AgentDocumentation.Insert(new Core.Domain.Agents.AgentDocumentation()
                            {
                                AgentId = agent.AgentId,
                                DownloadId = id
                            });
                        }
                    }

                    var success = UpdateAgentService.WithAgentId(agent.AgentId)
                    .Set(a => a.AgentStatus, AgentStatus.PendingSignature)
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
                        return Redirect($"/account/agent/complete-signature?id={agent.AgentGuid}");
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
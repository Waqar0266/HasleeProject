using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Plugin.Widgets.HassleFree.Domain;
using Nop.Plugin.Widgets.HassleFree.Models.Agents;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Agents
{
    public class CompleteAgentRegistrationController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private IGenericAttributeService _genericAttributeService { get; }
        private ICustomerService _customerService { get; }
        private IRepository<Agent> _agentRepo { get; }
        private IEmailAccountService _emailAccountService { get; }
        private IQueuedEmailService _queuedEmailService { get; }

        #endregion

        #region Ctor

        public CompleteAgentRegistrationController(IWorkContext workContext,
                                ICustomerService customerService,
                                IGenericAttributeService genericAttributeService,
                                IRepository<Agent> agentRepo,
                                IEmailAccountService emailAccountService,
                                IQueuedEmailService queuedEmailService)
        {
            _workContext = workContext;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _agentRepo = agentRepo;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Index(string uniqueId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            var roles = await _customerService.GetCustomerRolesAsync(customer);

            if (roles.Any(r => r.SystemName == "Director" || r.SystemName == "Mentor"))
                return Redirect("/customer/agents");

            var model = new CompleteAgentSignupViewModel() { AvailableRaces = new List<SelectListItem>(), AvailableTitles = new List<SelectListItem>(), AgentUniqueId = uniqueId };

            model.AvailableRaces.Add(new SelectListItem { Text = "Select Race", Value = "" });
            model.AvailableRaces.Add(new SelectListItem { Text = "African", Value = "African" });
            model.AvailableRaces.Add(new SelectListItem { Text = "White", Value = "White" });
            model.AvailableRaces.Add(new SelectListItem { Text = "Coloured", Value = "Coloured" });
            model.AvailableRaces.Add(new SelectListItem { Text = "Indian", Value = "Indian" });
            model.AvailableRaces.Add(new SelectListItem { Text = "Other", Value = "Other" });

            model.AvailableTitles.Add(new SelectListItem { Text = "Select Title", Value = "" });
            model.AvailableTitles.Add(new SelectListItem { Text = "Mr", Value = "Mr" });
            model.AvailableTitles.Add(new SelectListItem { Text = "Miss", Value = "Miss" });
            model.AvailableTitles.Add(new SelectListItem { Text = "Mrs", Value = "Mrs" });

            ViewBag.RequireForm = !await _genericAttributeService.GetAttributeAsync<bool>(customer, HassleFreeDefaults.Registration.FormComplete);

            return View("~/Plugins/Widgets.HassleFree/Views/Agents/CompleteAgentRegistration.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(CompleteAgentSignupViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = await _workContext.GetCurrentCustomerAsync();

                    if (!await _genericAttributeService.GetAttributeAsync<bool>(customer,
                        HassleFreeDefaults.Registration.FormComplete))
                    {
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Title, model.Title);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Race, model.Race);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Nationality, model.Nationality);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.PostalAddress1, model.PostalAddress1);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.PostalAddress2, model.PostalAddress2);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.PostalAddress3, model.PostalAddress3);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.PostalAddressCode, model.PostalAddressCode);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.PreviousEmployer, model.PreviousEmployer);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.FFC, model.FFC);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.FFCNumber, model.FFCNumber);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.FFCIssueDate, model.ParseFFCIssueDate());
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.EAABReference, model.Reference);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Dismissed, model.Dismissed);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Convicted, model.Convicted);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Insolvent, model.Insolvent);
                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.Withdrawn, model.Withdrawn);

                        await _genericAttributeService.SaveAttributeAsync(customer, HassleFreeDefaults.Registration.FormComplete, true);

                        //update the agent status
                        var agentUniqueId = Guid.Parse(model.AgentUniqueId);
                        var agent = _agentRepo.Table.FirstOrDefault(a => a.UniqueId.Equals(agentUniqueId));
                        agent.AgentStatus = AgentStatus.PendingDocumentation;
                        _agentRepo.UpdateAsync(agent);
                    }
                    else
                    {
                        ModelState.AddModelError("","");
                        return View("~/Plugins/Widgets.HassleFree/Views/Agents/CompleteAgentRegistration.cshtml", model);
                    }

                    return RedirectToRoute(HassleFreeDefaults.Agents.CompleteAgentRegistrationRouteName);
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Widgets.HassleFree/Views/Agents/CompleteAgentRegistration.cshtml", model);
        }

        #endregion
    }
}

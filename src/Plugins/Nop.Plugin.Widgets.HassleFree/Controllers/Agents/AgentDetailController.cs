using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Widgets.HassleFree.Domain;
using Nop.Plugin.Widgets.HassleFree.Models.Agents;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Agents
{
    public class AgentDetailController : BaseController
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

        public AgentDetailController(IWorkContext workContext,
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
        public async Task<IActionResult> Index(int id)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || (!await _customerService.IsInCustomerRoleAsync(customer, "Director") && !await _customerService.IsInCustomerRoleAsync(customer, "Mentor")))
                return Challenge();

            var agent = _agentRepo.Table.FirstOrDefault(a => a.Id == id);

            var model = new AgentViewModel()
            {
                Surname = agent.Surname,
                Name = agent.Name,
                IdNumber = agent.IdNumber,
                Mobile = agent.Mobile,
                AgentType = agent.AgentType,
                Email = agent.Email,
                Id = agent.Id,
                Province = agent.Province,
                CanUpdate = await _customerService.IsInCustomerRoleAsync(customer, "Director")
            };

            return View("~/Plugins/Widgets.HassleFree/Views/Agents/Details.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(int id, AgentViewModel model, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || (!await _customerService.IsInCustomerRoleAsync(customer, "Director") && !await _customerService.IsInCustomerRoleAsync(customer, "Mentor")))
                return Challenge();

            try
            {
                var existingAgent = _agentRepo.Table.FirstOrDefault(a => a.Id == model.Id);
                existingAgent.AgentType = model.AgentType;
                await _agentRepo.UpdateAsync(existingAgent);

                return RedirectToRoute(HassleFreeDefaults.Agents.ListAgentRouteName);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Widgets.HassleFree/Views/Agents/Details.cshtml", model);
        }

        #endregion
    }
}

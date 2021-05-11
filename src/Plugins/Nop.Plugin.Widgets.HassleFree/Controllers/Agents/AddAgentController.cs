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
    public class AddAgentController : BaseController
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

        public AddAgentController(IWorkContext workContext,
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
        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || (!await _customerService.IsInCustomerRoleAsync(customer, "Director") && !await _customerService.IsInCustomerRoleAsync(customer, "Mentor")))
                return Challenge();

            var model = new AgentViewModel();

            model.AvailableProvinces.Add(new SelectListItem { Text = "Select Province", Value = "" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Free State", Value = "Free State" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Gauteng", Value = "Gauteng" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "KwaZulu-Natal", Value = "KwaZulu-Natal" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Limpopo", Value = "Limpopo" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Mpumalanga", Value = "Mpumalanga" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "North West", Value = "North West" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Northern Cape", Value = "Northern Cape" });
            model.AvailableProvinces.Add(new SelectListItem { Text = "Western Cape", Value = "Western Cape" });

            ViewBag.AllowAgentType = await _customerService.IsInCustomerRoleAsync(customer, "Director");

            return View("~/Plugins/Widgets.HassleFree/Views/Agents/CRUD.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(AgentViewModel model, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || (!await _customerService.IsInCustomerRoleAsync(customer, "Director") && !await _customerService.IsInCustomerRoleAsync(customer, "Mentor")))
                return Challenge();

            //var newCustomer = new Customer()
            //{
            //    Email = model.Email,
            //    Active = true
            //};

            //await _customerService.InsertCustomerAsync(newCustomer);

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            try
            {
                if (ModelState.IsValid)
                {
                    //form fields
                    //await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.FirstNameAttribute, model.Name);
                    //await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.LastNameAttribute, model.Surname);
                    //await _genericAttributeService.SaveAttributeAsync(newCustomer, "IdNumber", model.IdNumber);
                    //await _genericAttributeService.SaveAttributeAsync(newCustomer, "Mobile", model.Mobile);

                    var agent = new Agent()
                    {
                        Province = model.Province,
                        CreatedOn = DateTime.Now,
                        //CustomerId = newCustomer.Id,
                        DirectorId = currentCustomer.Id,
                        ModifiedOn = DateTime.Now,
                        AgentType = model.AgentType,
                        Name = model.Name,
                        Surname = model.Surname,
                        AgentStatus = AgentStatus.PendingRegistration,
                        UniqueId = Guid.NewGuid(),
                        IdNumber = model.IdNumber,
                        Mobile = model.Mobile,
                        Email = model.Email
                    };
                    await _agentRepo.InsertAsync(agent);

                    var emailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();

                    var link = $"<a href=\"https://localhost:44369/agents/complete-sign-up/{agent.UniqueId.ToString()}\">Continue</a>";

                    var email = new QueuedEmail
                    {
                        Priority = QueuedEmailPriority.High,
                        From = emailAccounts.FirstOrDefault().Email,
                        FromName = emailAccounts.FirstOrDefault().DisplayName,
                        To = model.Email,
                        ToName = model.Name,
                        ReplyTo = emailAccounts.FirstOrDefault().Email,
                        ReplyToName = emailAccounts.FirstOrDefault().DisplayName,
                        Subject = "New Agent Registration",
                        Body = link,
                        CreatedOnUtc = DateTime.UtcNow,
                        EmailAccountId = emailAccounts.FirstOrDefault().Id
                    };

                    await _queuedEmailService.InsertQueuedEmailAsync(email);

                    return RedirectToRoute(HassleFreeDefaults.Agents.ListAgentRouteName);
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Widgets.HassleFree/Views/Agents/CRUD.cshtml", model);
        }

        #endregion
    }
}

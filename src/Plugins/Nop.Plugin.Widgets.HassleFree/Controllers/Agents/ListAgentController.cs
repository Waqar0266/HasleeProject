using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.HassleFree.Services.Agents;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Agents
{
    public class ListAgentController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private ICustomerService _customerService { get; }
        private IListAgentService _agentService { get; }

        #endregion

        #region Ctor

        public ListAgentController(IWorkContext workContext, ICustomerService customerService, IListAgentService agentService)
        {
            _workContext = workContext;
            _customerService = customerService;
            _agentService = agentService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || (!await _customerService.IsInCustomerRoleAsync(customer, "Director") && !await _customerService.IsInCustomerRoleAsync(customer, "Mentor")))
                return Challenge();

            var model = await _agentService.List();

            return View("~/Plugins/Widgets.HassleFree/Views/Agents/List.cshtml", model);
        }

        #endregion
    }
}

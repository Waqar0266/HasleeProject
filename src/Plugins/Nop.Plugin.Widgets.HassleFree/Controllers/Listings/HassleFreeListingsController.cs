using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.HassleFree.Services.Property24;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Listings
{
    public class HassleFreeListingsController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private ICustomerService _customerService { get; }
        private IGetProperty24Service _getProperty24Service { get; }

        #endregion

        #region Ctor

        public HassleFreeListingsController(IWorkContext workContext, ICustomerService customerService, IGetProperty24Service getProperty24Service)
        {
            _workContext = workContext;
            _customerService = customerService;
            _getProperty24Service = getProperty24Service;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || !await _customerService.IsInCustomerRoleAsync(customer, "Agent"))
                return Challenge();

            return View("~/Plugins/Widgets.HassleFree/Views/Listings/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Property24(string propertyId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || !await _customerService.IsInCustomerRoleAsync(customer, "Agent"))
                return Challenge();

            var model = await _getProperty24Service.WithPropertyId(propertyId).Get();

            return View("~/Plugins/Widgets.HassleFree/Views/Listings/Property24.cshtml", model);
        }

        #endregion
    }
}

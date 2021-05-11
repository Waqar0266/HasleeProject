using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Forms
{
    public class HassleFreeFormsTestController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private ICustomerService _customerService { get; }

        #endregion

        #region Ctor

        public HassleFreeFormsTestController(IWorkContext workContext, ICustomerService customerService)
        {
            _workContext = workContext;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || !await _customerService.IsInCustomerRoleAsync(customer, "Agent"))
                return Challenge();

            return View("~/Plugins/Widgets.HassleFree/Views/Forms/Test.cshtml");
        }

        #endregion
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Forms
{
    public class SignatureController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private ICustomerService _customerService { get; }

        #endregion

        #region Ctor

        public SignatureController(IWorkContext workContext, ICustomerService customerService)
        {
            _workContext = workContext;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> SignaturePad(string uniqueId = null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer) || !await _customerService.IsInCustomerRoleAsync(customer, "Agent"))
                return Challenge();

            if (uniqueId == null) uniqueId = Guid.NewGuid().ToString();

            return PartialView("~/Plugins/Widgets.HassleFree/Views/Forms/_SignaturePad.cshtml", uniqueId);
        }

        #endregion
    }
}

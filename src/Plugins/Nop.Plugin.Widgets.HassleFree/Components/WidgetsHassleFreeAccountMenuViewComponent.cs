using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.HassleFree.Components
{
    [ViewComponent(Name = "WidgetsHassleFreeAccountMenu")]
    public class WidgetsHassleFreeAccountMenuViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private ICustomerService _customerService { get; }
        private IWorkContext _workContext { get; }

        #endregion

        #region Ctor

        public WidgetsHassleFreeAccountMenuViewComponent(ILogger logger, ICustomerService customerService, IWorkContext workContext)
        {
            _logger = logger;
            _customerService = customerService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var routeData = Url.ActionContext.RouteData;
            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                var customer = await _workContext.GetCurrentCustomerAsync();
                var rolesAsync = await _customerService.GetCustomerRolesAsync(customer);
                var roles = rolesAsync.Select(r => r.Name).ToList();

                return View("~/Plugins/Widgets.HassleFree/Views/MenuItem.cshtml", roles);
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for Google eCommerce tracking", ex.ToString());
            }

            return View("~/Plugins/Widgets.HassleFree/Views/MenuItem.cshtml", new List<string>());

        }

        #endregion
    }
}
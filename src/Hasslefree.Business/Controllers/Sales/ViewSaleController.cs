using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Services.Sales.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Sales
{
    [AccessControlFilter(Permission = "Director,Landlord,Agent")]
    public class ViewSaleController : BaseController
    {
        #region Private Properties

        // Services
        private IGetSaleService GetSaleService { get; }

        // Other
        private IWebHelper WebHelper { get; }

        #endregion

        #region Constructor

        public ViewSaleController
        (
            //Services
            IGetSaleService getSaleService,

            //Other
            IWebHelper webHelper
        )
        {
            // Services
            GetSaleService = getSaleService; ;

            // Other
            WebHelper = webHelper;
        }

        #endregion

        [HttpGet, Route("account/sale")]
        public ActionResult View(int saleId)
        {
            // Model
            var rental = GetSaleService[saleId].Get();

            if (rental == null)
            {
                if (WebHelper.IsAjaxRequest())
                    return Json(new { }, JsonRequestBehavior.AllowGet);

                return RedirectToAction("Index", "ListSales");
            }

            // View bags
            PrepViewBag();

            // Ajax
            if (WebHelper.IsAjaxRequest())
                return PartialView("../Sales/View", rental);

            // Default
            return View("../Sales/View", rental);
        }

        #region Private 

        /// <summary>
        /// Set required view bags for action and the crud title
        /// </summary>
        private void PrepViewBag()
        {
            // Set current crud title to display
            ViewBag.Title = "View Sale";
        }

        #endregion
    }
}
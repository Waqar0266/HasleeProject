using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
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

    [AccessControlFilter(Permission = "Agent,Director,Landlord")]
    [AgentFilter]
    public class ListOtpsController : BaseController
    {
        private IWebHelper WebHelper { get; }

        public ListOtpsController(IWebHelper webHelper)
        {
            WebHelper = webHelper;
        }

        [HttpGet, Route("account/otps")]
        public ActionResult Index(string search = null, int page = 0, int pageSize = 50)
        {
            // Normal HTML
            return View("../Sales/OtpList");
        }

        #region Private Methods

        private void PrepViewBag(string search, int page, int pageSize, int totalRecords)
        {
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;

            ViewBag.Title = "OTPs";
        }

        #endregion
    }
}
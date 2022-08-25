using Hasslefree.Core;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Services.Sales.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.Rentals;
using Hasslefree.Web.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Sales
{
    public class SaleEmailsController : BaseController
    {
        #region Private Properties 

        //Services
        private IGetSaleService GetSale { get; }
        //private IGetExistingSaleService GetExistingSale { get; }

        // Other
        private IWebHelper WebHelper { get; }

        #endregion

        #region Constructor

        public SaleEmailsController
        (
            //Services
            IGetSaleService getSale,
            //IGetExistingSaleService getExistingSale,

            //Other
            IWebHelper webHelper
        )
        {
            //Services
            GetSale = getSale;
            //GetExistingSale = getExistingSale;

            // Other
            WebHelper = webHelper;
        }

        #endregion

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/sales/emails/seller-initial-email")]
        public ActionResult Email(int saleId, int sellerId)
        {
            var sale = GetSale[saleId].Get();
            var seller = sale.Sellers.FirstOrDefault(a => a.SellerId == sellerId);

            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{sale.UniqueId.ToString().ToLower()};{seller.UniqueId.ToString().ToLower()}"));

            var model = new SaleSellerEmail()
            {
                AgentName = sale.Agent.Person.FirstName,
                AgentSurname = sale.Agent.Person.Surname,
                Name = GetTempData(seller.Tempdata).Split(';')[0],
                Surname = GetTempData(seller.Tempdata).Split(';')[1],
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/sale/complete-sale?hash={hash}",
                Address = sale.Address,
                StandErf = sale.StandErf
            };

            return View("../Emails/Seller-Initial-Email", model);
        }

        #region Private Methods

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}
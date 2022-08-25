using Hasslefree.Core.Domain.Sales;
using Hasslefree.Data;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Sales
{
    [AccessControlFilter(Permission = "Agent,Director")]
    public class SaleAjaxController : BaseController
    {
        //Repos
        private IReadOnlyRepository<Sale> SaleRepo { get; }

        public SaleAjaxController(
            //Repos
            IReadOnlyRepository<Sale> saleRepo
            )
        {
            //Repos
            SaleRepo = saleRepo;
        }

        [HttpGet, Route("sales/query")]
        public ActionResult Index(string phrase)
        {
            var sales = SaleRepo.Table.ToList();
            var results = sales.Where(a => a.Address.ToLower().Contains(phrase)).Select(x => new { SaleId = x.SaleId, Name = x.Address }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}
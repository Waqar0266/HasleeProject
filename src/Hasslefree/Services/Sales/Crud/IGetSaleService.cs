using Hasslefree.Web.Models.Sales;

namespace Hasslefree.Services.Sales.Crud
{
    public interface IGetSaleService
    {
        IGetSaleService this[int saleId] { get; }
        IGetSaleService this[string saleGuid] { get; }
        SaleGet Get();
    }
}

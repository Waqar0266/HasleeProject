using Hasslefree.Core.Domain.Sales;
using System.Collections.Generic;

namespace Hasslefree.Services.Sales.Crud
{
    public interface ICreateSaleService
    {
        bool HasWarnings { get; }
        List<SaleWarning> Warnings { get; }
        int SaleId { get; }
        List<Seller> Sellers { get; }

        ICreateSaleService New(SaleType saleType);
        ICreateSaleService WithSeller(string idNumber, string name, string surname, string email, string mobile);
        ICreateSaleService WithAgentId(int agentId);
        bool Create();
    }
}

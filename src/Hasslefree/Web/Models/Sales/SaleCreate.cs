using Hasslefree.Core.Domain.Sales;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Sales
{
    public class SaleCreate
    {
        public SaleType SaleType { get; set; }
        public List<SaleCreateLandlord> Sellers { get; set; }
        public int? SaleId { get; set; }
    }

    public class SaleCreateLandlord
    {
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }
}

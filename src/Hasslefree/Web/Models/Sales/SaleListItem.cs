using System;

namespace Hasslefree.Web.Models.Sales
{
    public class SaleListItem
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
    }
}

using System;

namespace Hasslefree.Web.Models.RentalTs
{
    public class RentalTListItem
    {
        public int Id { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
    }
}

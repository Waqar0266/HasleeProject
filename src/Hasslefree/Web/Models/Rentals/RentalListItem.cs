using System;

namespace Hasslefree.Web.Models.Rentals
{
	public class RentalListItem
    {
        public int RentalId { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
		public DateTime ModifiedOn { get; set; }   
	}
}

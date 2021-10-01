using System;

namespace Hasslefree.Web.Models.Rentals
{
	public class RentalListItem
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
		public DateTime ModifiedOn { get; set; }
		public bool IsExisting { get; set; }
	}
}

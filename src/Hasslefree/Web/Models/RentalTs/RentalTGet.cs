using Hasslefree.Web.Models.Rentals;
using System;

namespace Hasslefree.Web.Models.RentalTs
{
	public class RentalTGet
	{
		public int RentalTId { get; set; }
		public Guid RentalTGuid { get; set; }
		public RentalGet Rental { get; set; }
	}
}

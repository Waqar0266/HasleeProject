using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalResolution : BaseEntity
	{
		public int RentalResolutionId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string HeldAt { get; set; }
		public DateTime HeldOn { get; set; }
		public string LeaseName { get; set; }
		public string AuthorizedName { get; set; }
		public string AuthorizedSurname { get; set; }
	}
}

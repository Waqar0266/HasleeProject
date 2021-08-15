using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Core.Domain.Rentals
{
	public class LandlordAddress : BaseEntity
	{
		public int LandlordAddressId { get; set; }
		public int RentalLandlordId { get; set; }
		public RentalLandlord RentalLandlord { get; set; }
		public int AddressId { get; set; }
		public Address Address { get; set; }
	}
}

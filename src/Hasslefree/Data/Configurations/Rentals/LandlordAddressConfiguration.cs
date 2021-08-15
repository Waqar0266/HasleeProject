using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class LandlordAddressConfiguration : EntityTypeConfiguration<LandlordAddress>
	{
		public LandlordAddressConfiguration()
		{
			// Table
			ToTable("LandlordAddress");

			// Primary Key
			HasKey(a => a.LandlordAddressId);

			HasRequired(a => a.Address)
			.WithMany()
			.HasForeignKey(a => a.AddressId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.RentalLandlord)
			.WithMany()
			.HasForeignKey(a => a.RentalLandlordId)
			.WillCascadeOnDelete(false);
		}
	}
}

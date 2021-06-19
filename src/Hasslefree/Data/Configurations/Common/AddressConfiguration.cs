using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Common
{
	public class AddressConfiguration : EntityTypeConfiguration<Core.Domain.Common.Address>
	{
		public AddressConfiguration()
		{
			// Table
			ToTable("Address");

			// Primary Key
			HasKey(a => a.AddressId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Country).HasMaxLength(64);

			Property(a => a.Address1).HasMaxLength(128);
			Property(a => a.Address2).HasMaxLength(128);
			Property(a => a.Address3).HasMaxLength(128);

			Property(a => a.Town).HasMaxLength(64);
			Property(a => a.Code).HasMaxLength(24);
			Property(a => a.RegionName).HasMaxLength(32);
			Property(a => a.Latitude).HasMaxLength(24);
			Property(a => a.Longitude).HasMaxLength(24);

			Property(a => a.TypeEnum).IsOptional().HasMaxLength(16);

			Ignore(a => a.Type);
		}
	}
}

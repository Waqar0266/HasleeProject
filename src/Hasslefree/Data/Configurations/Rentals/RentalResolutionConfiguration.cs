using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalResolutionConfiguration : EntityTypeConfiguration<RentalResolution>
	{
		public RentalResolutionConfiguration()
		{
			// Table
			ToTable("RentalResolution");

			// Primary Key
			HasKey(a => a.RentalResolutionId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.AuthorizedName).IsRequired().HasMaxLength(100);
			Property(a => a.AuthorizedSurname).IsRequired().HasMaxLength(100);
			Property(a => a.HeldAt).IsRequired().HasMaxLength(100);
			Property(a => a.HeldOn).IsRequired();
			Property(a => a.LeaseName).IsRequired().HasMaxLength(150);
		}
	}
}

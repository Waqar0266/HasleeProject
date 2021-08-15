using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class LandlordDocumentationConfiguration : EntityTypeConfiguration<LandlordDocumentation>
	{
		public LandlordDocumentationConfiguration()
		{
			// Table
			ToTable("LandlordDocumentation");

			// Primary Key
			HasKey(a => a.LandlordDocumentationId);

			HasRequired(a => a.Download)
			.WithMany()
			.HasForeignKey(a => a.DownloadId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.RentalLandlord)
			.WithMany()
			.HasForeignKey(a => a.RentalLandlordId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
		}
	}
}

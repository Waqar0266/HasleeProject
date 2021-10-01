using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalFormConfiguration : EntityTypeConfiguration<RentalForm>
	{
		public RentalFormConfiguration()
		{
			// Table
			ToTable("RentalForm");

			// Primary Key
			HasKey(a => a.RentalFormId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Download)
			.WithMany()
			.HasForeignKey(a => a.DownloadId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.RentalFormNameEnum).IsRequired().HasMaxLength(100);

			// Ignore
			Ignore(a => a.RentalFormName);
		}
	}
}

using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class ExistingRentalFormConfiguration : EntityTypeConfiguration<ExistingRentalForm>
	{
		public ExistingRentalFormConfiguration()
		{
			// Table
			ToTable("ExistingRentalForm");

			// Primary Key
			HasKey(a => a.ExistingRentalFormId);

			HasRequired(a => a.ExistingRental)
			.WithMany()
			.HasForeignKey(a => a.ExistingRentalId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Download)
			.WithMany()
			.HasForeignKey(a => a.DownloadId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ExistingRentalFormNameEnum).IsRequired().HasMaxLength(100);

			// Ignore
			Ignore(a => a.ExistingRentalFormName);
		}
	}
}

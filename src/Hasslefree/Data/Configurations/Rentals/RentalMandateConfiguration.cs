using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalMandateConfiguration : EntityTypeConfiguration<RentalMandate>
	{
		public RentalMandateConfiguration()
		{
			// Table
			ToTable("RentalMandate");

			// Primary Key
			HasKey(a => a.RentalMandateId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.AddendumAmendment).IsOptional().HasMaxLength(2500);
			Property(a => a.AddendumEndDate).IsOptional();
			Property(a => a.AddendumManagement).IsOptional().HasPrecision(15, 5);
			Property(a => a.AddendumProcurement).IsOptional().HasPrecision(15, 5);
			Property(a => a.AddendumStartDate).IsOptional();
			Property(a => a.ManagementAmount).IsOptional().HasPrecision(15,5);
			Property(a => a.ManagementPercentage).IsOptional().HasPrecision(15,5);
			Property(a => a.Procurement1Amount).IsOptional().HasPrecision(15,5);
			Property(a => a.Procurement1Percentage).IsOptional().HasPrecision(15,5);
			Property(a => a.Procurement2Amount).IsOptional().HasPrecision(15,5);
			Property(a => a.Procurement2Percentage).IsOptional().HasPrecision(15,5);
			Property(a => a.Procurement3Amount).IsOptional().HasPrecision(15, 5);
			Property(a => a.Procurement3Percentage).IsOptional().HasPrecision(15,5);
			Property(a => a.SaleAmount).IsOptional().HasPrecision(15,5);
			Property(a => a.SalePercentage).IsOptional().HasPrecision(15,5);
		}
	}
}

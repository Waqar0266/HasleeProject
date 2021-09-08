using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class ExistingRentalConfiguration : EntityTypeConfiguration<ExistingRental>
	{
		public ExistingRentalConfiguration()
		{
			// Table
			ToTable("ExistingRental");

			// Primary Key
			HasKey(a => a.ExistingRentalId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness1Signature)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness1SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness2Signature)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness2SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness1Signature)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness1SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness2Signature)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness2SignatureId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.EndDate).IsOptional();
			Property(a => a.StartDate).IsOptional();
			Property(a => a.AmendedAddendum).IsOptional().HasMaxLength(3000);
			Property(a => a.MaterialChanges).IsOptional().HasMaxLength(1500);
			Property(a => a.ParkingBays).IsOptional().HasMaxLength(500);
			Property(a => a.RenewalCommencementDate).IsOptional();
			Property(a => a.RenewalPeriod).IsOptional().HasMaxLength(150);
			Property(a => a.RenewalTerminationDate).IsOptional();
			Property(a => a.RenewLease).IsOptional();
			Property(a => a.TerminationDate).IsOptional();
			Property(a => a.ExistingRentalTypeEnum).IsRequired().HasMaxLength(30);

			Ignore(a => a.ExistingRentalType);
		}
	}
}

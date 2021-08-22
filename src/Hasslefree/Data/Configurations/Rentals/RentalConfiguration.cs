using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalConfiguration : EntityTypeConfiguration<Rental>
	{
		public RentalConfiguration()
		{
			// Table
			ToTable("Rental");

			// Primary Key
			HasKey(a => a.RentalId);

			HasRequired(a => a.Agent)
			.WithMany()
			.HasForeignKey(a => a.AgentId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Address).HasMaxLength(255);
			Property(a => a.AskLandlordConsent).IsRequired();
			Property(a => a.ContactLandlord).IsRequired();
			Property(a => a.Deposit).IsOptional().HasPrecision(15, 5);
			Property(a => a.Explaining).IsRequired();
			Property(a => a.IncomingSnaglist).IsRequired();
			Property(a => a.Informing).IsRequired();
			Property(a => a.LeaseTypeEnum).IsRequired().HasMaxLength(55);
			Property(a => a.Management).IsRequired();
			Property(a => a.Marketing).IsRequired();
			Property(a => a.MonthlyRental).IsOptional().HasPrecision(15, 5);
			Property(a => a.MonthlyPaymentDate).IsOptional();
			Property(a => a.DepositPaymentDate).IsOptional();
			Property(a => a.Negotiating).IsRequired();
			Property(a => a.OutgoingSnaglist).IsRequired();
			Property(a => a.PayingLandlord).IsRequired();
			Property(a => a.Premises).IsOptional().HasMaxLength(255);
			Property(a => a.ProcureDepositLandlord).IsRequired();
			Property(a => a.ProcureDepositPreviousRentalAgent).IsRequired();
			Property(a => a.ProcureDepositOther).IsOptional().HasMaxLength(255);
			Property(a => a.Procurement).IsRequired();
			Property(a => a.ProvideLandlord).IsRequired();
			Property(a => a.RentalTypeEnum).IsRequired().HasMaxLength(55);
			Property(a => a.SpecialConditions).IsOptional().HasMaxLength(2500);
			Property(a => a.SpecificRequirements).IsOptional().HasMaxLength(2500);
			Property(a => a.StandErf).IsOptional().HasMaxLength(255);
			Property(a => a.Township).IsOptional().HasMaxLength(255);
			Property(a => a.TransferDeposit).IsRequired();
			Property(a => a.RentalStatusEnum).IsRequired().HasMaxLength(55);

			// Ignore
			Ignore(a => a.LeaseType);
			Ignore(a => a.RentalType);
			Ignore(a => a.RentalStatus);
		}
	}
}

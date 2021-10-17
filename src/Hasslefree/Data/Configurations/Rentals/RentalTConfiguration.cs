using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
    public class RentalTConfiguration : EntityTypeConfiguration<RentalT>
    {
        public RentalTConfiguration()
        {
            // Table
            ToTable("RentalT");

            // Primary Key
            HasKey(a => a.RentalTId);

            HasOptional(a => a.Agent)
            .WithMany()
            .HasForeignKey(a => a.AgentId)
            .WillCascadeOnDelete(false);

            // Columns
            Property(a => a.UniqueId).IsRequired();
            Property(a => a.CreatedOn).IsRequired();
            Property(a => a.ModifiedOn).IsRequired();
            Property(a => a.Adults).IsOptional();
            Property(a => a.AllowPets).IsRequired();
            Property(a => a.ChildAge1).IsOptional();
            Property(a => a.ChildAge2).IsOptional();
            Property(a => a.ChildAge3).IsOptional();
            Property(a => a.ChildAge4).IsOptional();
            Property(a => a.Children).IsOptional();
            Property(a => a.City).IsOptional().HasMaxLength(100);
            Property(a => a.CommencementKeys).IsOptional();
            Property(a => a.CommencementRemotes).IsOptional();
            Property(a => a.CommencementSecurityTags).IsOptional();
            Property(a => a.ComplexName).IsOptional().HasMaxLength(100);
            Property(a => a.CreditCheckFee).IsOptional().HasPrecision(15, 5);
            Property(a => a.DebtReview).IsRequired();
            Property(a => a.DebtReviewDescription).IsOptional().HasMaxLength(800);
            Property(a => a.Defaults).IsRequired();
            Property(a => a.DefaultsDescription).IsOptional().HasMaxLength(800);
            Property(a => a.Deposit).IsOptional().HasPrecision(15, 5);
            Property(a => a.DepositInterestAgent).IsRequired();
            Property(a => a.DepositInterestTenant).IsRequired();
            Property(a => a.DirectMarketing).IsRequired();
            Property(a => a.ElectricityDeposit).IsOptional().HasPrecision(15, 5);
            Property(a => a.FirstMonthRent).IsOptional().HasPrecision(15, 5);
            Property(a => a.InitialPeriod).IsOptional();
            Property(a => a.InspectionFee).IsOptional().HasPrecision(15, 5);
            Property(a => a.KeyDeposit).IsOptional().HasPrecision(15, 5);
            Property(a => a.KeyReturnDateTime).IsOptional();
            Property(a => a.LeaseAdminFee).IsOptional().HasPrecision(15, 5);
            Property(a => a.LeaseCommencementDate).IsOptional();
            Property(a => a.LeaseFee).IsOptional().HasPrecision(15, 5);
            Property(a => a.LeaseTerminationDate).IsOptional();
            Property(a => a.LeaseTypeEnum).IsRequired().HasMaxLength(50);
            Property(a => a.MaximumCancellationMonthRent).IsOptional().HasPrecision(15, 5);
            Property(a => a.MaximumOccupants).IsOptional();
            Property(a => a.MinimumCancellationMonths).IsOptional();
            Property(a => a.OccupantIdentityNumbers).IsOptional().HasMaxLength(100);
            Property(a => a.Occupants).IsOptional().HasMaxLength(255);
            Property(a => a.ParkingBayNumber).IsOptional().HasMaxLength(100);
            Property(a => a.ParkingFees).IsOptional().HasPrecision(15, 5);
            Property(a => a.PayDepositInAdvance).IsRequired();
            Property(a => a.PaymentMethod).IsOptional().HasMaxLength(100);
            Property(a => a.PermanentVehicles).IsOptional().HasMaxLength(255);
            Property(a => a.Pets).IsOptional();
            Property(a => a.PetTypes).IsOptional().HasMaxLength(255);
            Property(a => a.PostalCode).IsOptional().HasMaxLength(10);
            Property(a => a.ProRataRent).IsOptional().HasPrecision(15, 5);
            Property(a => a.Province).IsOptional().HasMaxLength(50);
            Property(a => a.RentalEscalationPercentage).IsOptional().HasPrecision(15, 5);
            Property(a => a.RentalTStatusEnum).IsRequired().HasMaxLength(50);
            Property(a => a.ReturnKeys).IsOptional();
            Property(a => a.ReturnRemotes).IsOptional();
            Property(a => a.ReturnSecurityTags).IsOptional();
            Property(a => a.SalesCommision).IsOptional().HasPrecision(15, 5);
            Property(a => a.SchoolNames).IsOptional().HasMaxLength(255);
            Property(a => a.Smoking).IsRequired();
            Property(a => a.SpecialConditions).IsOptional().HasMaxLength(800);
            Property(a => a.StreetName).IsOptional().HasMaxLength(100);
            Property(a => a.StreetNumber).IsOptional().HasMaxLength(10);
            Property(a => a.Suburb).IsOptional().HasMaxLength(100);
            Property(a => a.Surcharge).IsOptional().HasPrecision(15, 5);
            Property(a => a.TenantFinancialBenefits).IsOptional().HasMaxLength(800);
            Property(a => a.UnitNumber).IsOptional().HasMaxLength(10);
            Property(a => a.Vehicle1Registration).IsOptional().HasMaxLength(20);
            Property(a => a.Vehicle1Type).IsOptional().HasMaxLength(50);
            Property(a => a.Vehicle2Registration).IsOptional().HasMaxLength(20);
            Property(a => a.Vehicle2Type).IsOptional().HasMaxLength(50);
            Property(a => a.Vehicle3Registration).IsOptional().HasMaxLength(20);
            Property(a => a.Vehicle3Type).IsOptional().HasMaxLength(50);

            // Ignore
            Ignore(a => a.LeaseType);
            Ignore(a => a.RentalTStatus);
        }
    }
}

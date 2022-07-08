using Hasslefree.Core.Domain.Sales;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Sales
{
    public class SaleConfiguration : EntityTypeConfiguration<Sale>
    {
        public SaleConfiguration()
        {
            // Table
            ToTable("Sale");

            // Primary Key
            HasKey(a => a.SaleId);

            HasOptional(a => a.Agent)
            .WithMany()
            .HasForeignKey(a => a.AgentId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.Property)
            .WithMany()
            .HasForeignKey(a => a.PropertyId)
            .WillCascadeOnDelete(false);

            // Columns
            Property(a => a.UniqueId).IsRequired();
            Property(a => a.CreatedOn).IsRequired();
            Property(a => a.ModifiedOn).IsRequired();
            Property(a => a.Address).HasMaxLength(255);
            Property(a => a.StandErf).IsOptional().HasMaxLength(255);
            Property(a => a.Township).IsOptional().HasMaxLength(255);
            Property(a => a.AgentCommissionAmount).IsOptional().HasPrecision(15, 5);
            Property(a => a.AgentCommissionPercentage).IsOptional().HasPrecision(15, 5);
            Property(a => a.AlarmSystems).IsRequired();
            Property(a => a.AlarmSystemWorking).IsRequired();
            Property(a => a.Alterations).IsRequired();
            Property(a => a.ApproximateArea).IsOptional().HasMaxLength(100);
            Property(a => a.ArmedResponse).IsRequired();
            Property(a => a.Blinds).IsRequired();
            Property(a => a.BoreholePumps).IsRequired();
            Property(a => a.BrokenWindowsGlassDoors).IsRequired();
            Property(a => a.BuildingPlans).IsRequired();
            Property(a => a.BuiltInHeaters).IsRequired();
            Property(a => a.DoorKeys).IsRequired();
            Property(a => a.ElectricalPlugsLightFittingsOperational).IsRequired();
            Property(a => a.ElectricityAndWater).IsOptional().HasPrecision(15, 5);
            Property(a => a.ExcludingFixtures).IsOptional().HasMaxLength(2000);
            Property(a => a.ExistingLightFittings).IsRequired();
            Property(a => a.Fences).IsRequired();
            Property(a => a.FittedCarpets).IsRequired();
            Property(a => a.FreeholdSectionalTitle).IsOptional().HasMaxLength(255);
            Property(a => a.GarageDoorsWorking).IsRequired();
            Property(a => a.GateMotors).IsRequired();
            Property(a => a.GatesWorking).IsRequired();
            Property(a => a.Generators).IsRequired();
            Property(a => a.HobPlatesElementsWorking).IsRequired();
            Property(a => a.IncludingFixtures).IsOptional().HasMaxLength(2500);
            Property(a => a.Leaks).IsRequired();
            Property(a => a.Levies).IsOptional().HasPrecision(15, 5);
            Property(a => a.MandateCommencementDate).IsOptional();
            Property(a => a.MandateDuration).IsOptional().HasMaxLength(50);
            Property(a => a.MandateExpiryDate).IsOptional();
            Property(a => a.MarketingAdvertisingStrategy).IsOptional().HasMaxLength(1500);
            Property(a => a.MissingKeys).IsRequired();
            Property(a => a.MissingLocks).IsRequired();
            Property(a => a.OccupationalRental).IsOptional().HasPrecision(15, 5);
            Property(a => a.OccupationDate).IsOptional();
            Property(a => a.OtherDefects).IsOptional().HasMaxLength(1500);
            Property(a => a.PelmentsAndCurtainFittings).IsRequired();
            Property(a => a.PermissionSaleBoard).IsRequired();
            Property(a => a.PermissionShowHouseBoard).IsRequired();
            Property(a => a.PermissionSoldBoard).IsRequired();
            Property(a => a.PlumbingDefects).IsRequired();
            Property(a => a.PoolCleaningEquipment).IsRequired();
            Property(a => a.PoolEquipment).IsRequired();
            Property(a => a.PoolEquipmentWorking).IsRequired();
            Property(a => a.PoolPumpFilterLightWorking).IsRequired();
            Property(a => a.Pumps).IsRequired();
            Property(a => a.PurchasePrice).IsOptional().HasPrecision(15, 5);
            Property(a => a.RatesAndTaxes).IsOptional().HasPrecision(15, 5);
            Property(a => a.RisingDampMould).IsRequired();
            Property(a => a.SaleTypeEnum).IsRequired().HasMaxLength(30);
            Property(a => a.SatelliteDishes).IsRequired();
            Property(a => a.SolarPanels).IsRequired();
            Property(a => a.SpecialConditions).IsOptional().HasMaxLength(1000);
            Property(a => a.StandErf).IsOptional().HasMaxLength(255);
            Property(a => a.Stoves).IsRequired();
            Property(a => a.StoveWorking).IsRequired();
            Property(a => a.SuspensiveConditions).IsOptional().HasMaxLength(2500);
            Property(a => a.TelevisionAerials).IsRequired();
            Property(a => a.Township).IsOptional().HasMaxLength(255);
            Property(a => a.ValuationSchedule).IsOptional().HasMaxLength(2500);
            Property(a => a.SaleStatusEnum).IsRequired().HasMaxLength(50);

            // Ignore
            Ignore(a => a.SaleType);
            Ignore(a => a.SaleStatus);
        }
    }
}

using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Properties;
using Hasslefree.Core.Domain.Sales;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Sales
{
    public class SaleGet
    {
        public int SaleId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public SaleType SaleType { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public Agent Agent { get; set; }
        public decimal? AgentCommissionPercentage { get; set; }
        public decimal? AgentCommissionAmount { get; set; }
        public bool ExistingLightFittings { get; set; }
        public bool PoolCleaningEquipment { get; set; }
        public bool PelmentsAndCurtainFittings { get; set; }
        public bool FittedCarpets { get; set; }
        public bool Fences { get; set; }
        public bool Generators { get; set; }
        public bool TelevisionAerials { get; set; }
        public bool Stoves { get; set; }
        public bool Blinds { get; set; }
        public bool Pumps { get; set; }
        public bool PoolEquipment { get; set; }
        public bool BoreholePumps { get; set; }
        public bool SatelliteDishes { get; set; }
        public bool BuiltInHeaters { get; set; }
        public bool DoorKeys { get; set; }
        public bool GateMotors { get; set; }
        public bool AlarmSystems { get; set; }
        public bool SolarPanels { get; set; }
        public string IncludingFixtures { get; set; }
        public string ExcludingFixtures { get; set; }
        public string Address { get; set; }
        public string FreeholdSectionalTitle { get; set; }
        public string StandErf { get; set; }
        public string Township { get; set; }
        public string ApproximateArea { get; set; }
        public string MandateDuration { get; set; }
        public DateTime? MandateCommencementDate { get; set; }
        public DateTime? MandateExpiryDate { get; set; }
        public decimal? RatesAndTaxes { get; set; }
        public decimal? Levies { get; set; }
        public decimal? ElectricityAndWater { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? OccupationDate { get; set; }
        public decimal? OccupationalRental { get; set; }
        public string SuspensiveConditions { get; set; }
        public string SpecialConditions { get; set; }
        public bool PermissionSaleBoard { get; set; }
        public bool PermissionShowHouseBoard { get; set; }
        public bool PermissionSoldBoard { get; set; }
        public bool AlarmSystemWorking { get; set; }
        public bool MissingLocks { get; set; }
        public bool PoolPumpFilterLightWorking { get; set; }
        public bool ArmedResponse { get; set; }
        public bool BrokenWindowsGlassDoors { get; set; }
        public bool BuildingPlans { get; set; }
        public bool ElectricalPlugsLightFittingsOperational { get; set; }
        public bool GatesWorking { get; set; }
        public bool GarageDoorsWorking { get; set; }
        public bool Alterations { get; set; }
        public bool HobPlatesElementsWorking { get; set; }
        public bool StoveWorking { get; set; }
        public bool RisingDampMould { get; set; }
        public bool Leaks { get; set; }
        public bool MissingKeys { get; set; }
        public bool PlumbingDefects { get; set; }
        public bool PoolEquipmentWorking { get; set; }
        public string OtherDefects { get; set; }
        public string MarketingAdvertisingStrategy { get; set; }
        public string ValuationSchedule { get; set; }
        public Property Property { get; set; }
        public List<Seller> Sellers { get; set; }
    }
}

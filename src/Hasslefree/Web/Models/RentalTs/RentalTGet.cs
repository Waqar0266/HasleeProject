using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
	public class RentalTGet
	{
		public RentalTGet()
		{
			this.Tenants = new List<Tenant>();
		}

		public int RentalTId { get; set; }
		public Guid RentalTGuid { get; set; }
        public RentalTStatus Status { get; set; }
        public RentalTType RentalTType { get; set; }
        public int? Children { get; set; }
        public int? Adults { get; set; }
        public int? ChildAge1 { get; set; }
        public int? ChildAge2 { get; set; }
        public int? ChildAge3 { get; set; }
        public int? ChildAge4 { get; set; }
        public string SchoolNames { get; set; }
        public int? Pets { get; set; }
        public string PetTypes { get; set; }
        public string Vehicle1Type { get; set; }
        public string Vehicle1Registration { get; set; }
        public string Vehicle2Type { get; set; }
        public string Vehicle2Registration { get; set; }
        public string Vehicle3Type { get; set; }
        public string Vehicle3Registration { get; set; }
        public bool Defaults { get; set; }
        public string DefaultsDescription { get; set; }
        public bool DebtReview { get; set; }
        public string DebtReviewDescription { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? KeyDeposit { get; set; }
        public decimal? ElectricityDeposit { get; set; }
        public decimal? LeaseFee { get; set; }
        public decimal? ProRataRent { get; set; }
        public decimal? FirstMonthRent { get; set; }
        public string UnitNumber { get; set; }
        public string ComplexName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string ParkingBayNumber { get; set; }
        public bool Smoking { get; set; }
        public bool AllowPets { get; set; }
        public string PaymentMethod { get; set; }
        public bool DepositInterestAgent { get; set; }
        public bool DepositInterestTenant { get; set; }
        public bool PayDepositInAdvance { get; set; }
        public decimal? ParkingFees { get; set; }
        public decimal? LeaseAdminFee { get; set; }
        public decimal? CreditCheckFee { get; set; }
        public decimal? RentalEscalationPercentage { get; set; }
        public decimal? InspectionFee { get; set; }
        public decimal? Surcharge { get; set; }
        public int? InitialPeriod { get; set; }
        public DateTime? LeaseCommencementDate { get; set; }
        public DateTime? LeaseTerminationDate { get; set; }
        public string TenantFinancialBenefits { get; set; }
        public DateTime? KeyReturnDateTime { get; set; }
        public bool DirectMarketing { get; set; }
        public int? MaximumOccupants { get; set; }
        public string PermanentVehicles { get; set; }
        public string Occupants { get; set; }
        public string OccupantIdentityNumbers { get; set; }
        public int? MinimumCancellationMonths { get; set; }
        public decimal? MaximumCancellationMonthRent { get; set; }
        public decimal? SalesCommision { get; set; }
        public string SpecialConditions { get; set; }
        public int? CommencementKeys { get; set; }
        public int? CommencementRemotes { get; set; }
        public int? CommencementSecurityTags { get; set; }
        public int? ReturnKeys { get; set; }
        public int? ReturnRemotes { get; set; }
        public int? ReturnSecurityTags { get; set; }
        public RentalGet Rental { get; set; }
		public List<Tenant> Tenants { get; set; }
	}
}

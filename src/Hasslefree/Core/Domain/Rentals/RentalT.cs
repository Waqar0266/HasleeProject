using Hasslefree.Core.Domain.Agents;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalT : BaseEntity
	{
		public RentalT()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
			this.Tenants = new HashSet<Tenant>();
			this.Applicants = new HashSet<RentalTJuristicApplicant>();
		}

		public int RentalTId { get; set; }
		public Guid UniqueId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int AgentId { get; set; }
		public Agent Agent { get; set; }
		public string LeaseTypeEnum { get; set; }
		public LeaseType LeaseType
		{
			get => (LeaseType)Enum.Parse(typeof(LeaseType), LeaseTypeEnum);
			set => LeaseTypeEnum = value.ToString();
		}
		public string RentalTStatusEnum { get; set; }
		public RentalTStatus RentalTStatus
		{
			get => (RentalTStatus)Enum.Parse(typeof(RentalTStatus), RentalTStatusEnum);
			set => RentalTStatusEnum = value.ToString();
		}
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
		public ICollection<Tenant> Tenants { get; set; }
		public ICollection<RentalTJuristicApplicant> Applicants { get; set; }
	}
}

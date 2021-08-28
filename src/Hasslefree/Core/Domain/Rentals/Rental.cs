using Hasslefree.Core.Domain.Agents;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Rentals
{
    public class Rental: BaseEntity
	{
		public Rental()
        {
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.Landlords = new HashSet<RentalLandlord>();
			this.UniqueId = Guid.NewGuid();
        }

		public int RentalId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int AgentId { get; set; }
		public Agent Agent { get; set; }
		public string RentalTypeEnum { get; set; }
		public RentalType RentalType
		{
			get => (RentalType)Enum.Parse(typeof(RentalType), RentalTypeEnum);
			set => RentalTypeEnum = value.ToString();
		}
		public string LeaseTypeEnum { get; set; }
		public LeaseType LeaseType
		{
			get => (LeaseType)Enum.Parse(typeof(LeaseType), LeaseTypeEnum);
			set => LeaseTypeEnum = value.ToString();
		}
		public string RentalStatusEnum { get; set; }
		public RentalStatus RentalStatus
		{
			get => (RentalStatus)Enum.Parse(typeof(RentalStatus), RentalStatusEnum);
			set => RentalStatusEnum = value.ToString();
		}
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Township { get; set; }
		public string Address { get; set; }
		public decimal? MonthlyRental { get; set; }
		public decimal? Deposit { get; set; }
		public DateTime? MonthlyPaymentDate { get; set; }
		public DateTime? DepositPaymentDate { get; set; }
		public bool Marketing { get; set; }
		public bool Procurement { get; set; }
		public bool Management { get; set; }
		public string SpecificRequirements { get; set; }
		public string SpecialConditions { get; set; }
		public bool Negotiating { get; set; }
		public bool Informing { get; set; }
		public bool IncomingSnaglist { get; set; }
		public bool OutgoingSnaglist { get; set; }
		public bool Explaining { get; set; }
		public bool PayingLandlord { get; set; }
		public bool ContactLandlord { get; set; }
		public bool ProvideLandlord { get; set; }
		public bool AskLandlordConsent { get; set; }
		public bool ProcureDepositLandlord { get; set; }
		public bool ProcureDepositPreviousRentalAgent { get; set; }
		public string ProcureDepositOther { get; set; }
		public bool TransferDeposit { get; set; }
		public bool PowerOfAttorney { get; set; }
		public ICollection<RentalLandlord> Landlords { get; set; }
    }
}

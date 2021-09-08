using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class ExistingRental : BaseEntity
	{
		public ExistingRental()
		{
			this.CreatedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int ExistingRentalId { get; set; }
		public Guid UniqueId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string ExistingRentalTypeEnum { get; set; }
		public ExistingRentalType ExistingRentalType
		{
			get => (ExistingRentalType)Enum.Parse(typeof(ExistingRentalType), ExistingRentalTypeEnum);
			set => ExistingRentalTypeEnum = value.ToString();
		}
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string AmendedAddendum { get; set; }
		public int? AgentWitness1SignatureId { get; set; }
		public Picture AgentWitness1Signature { get; set; }
		public int? AgentWitness2SignatureId { get; set; }
		public Picture AgentWitness2Signature { get; set; }
		public int? LandlordWitness1SignatureId { get; set; }
		public Picture LandlordWitness1Signature { get; set; }
		public int? LandlordWitness2SignatureId { get; set; }
		public Picture LandlordWitness2Signature { get; set; }
		public string ParkingBays { get; set; }
		public DateTime? TerminationDate { get; set; }
		public bool RenewLease { get; set; }
		public string RenewalPeriod { get; set; }
		public DateTime? RenewalCommencementDate { get; set; }
		public DateTime? RenewalTerminationDate { get; set; }
		public string MaterialChanges { get; set; }
	}
}

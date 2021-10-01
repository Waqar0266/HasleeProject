using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Domain.Rentals;
using System;

namespace Hasslefree.Web.Models.Rentals
{
	public class ExistingRentalGet
	{
		public int ExistingRentalId { get; set; }
		public Guid ExistingRentalGuid { get; set; }
		public int RentalId { get; set; }
		public ExistingRentalType ExistingRentalType { get; set; }
		public RentalGet Rental { get; set; }
		public ExistingRentalStatus Status { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string AmendedAddendum { get; set; }
		public string AgentWitness1Name { get; set; }
		public string AgentWitness1Surname { get; set; }
		public string AgentWitness1Email { get; set; }
		public int? AgentWitness1SignatureId { get; set; }
		public Picture AgentWitness1Signature { get; set; }
		public string AgentWitness2Name { get; set; }
		public string AgentWitness2Surname { get; set; }
		public string AgentWitness2Email { get; set; }
		public int? AgentWitness2SignatureId { get; set; }
		public Picture AgentWitness2Signature { get; set; }
		public string LandlordWitness1Name { get; set; }
		public string LandlordWitness1Surname { get; set; }
		public string LandlordWitness1Email { get; set; }
		public int? LandlordWitness1SignatureId { get; set; }
		public Picture LandlordWitness1Signature { get; set; }
		public string LandlordWitness2Name { get; set; }
		public string LandlordWitness2Surname { get; set; }
		public string LandlordWitness2Email { get; set; }
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

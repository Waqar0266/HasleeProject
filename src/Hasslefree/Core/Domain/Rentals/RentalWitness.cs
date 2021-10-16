using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalWitness : BaseEntity
	{
		public RentalWitness()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int RentalWitnessId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid UniqueId { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string LandlordWitness1Name { get; set; }
		public string LandlordWitness1Surname { get; set; }
		public string LandlordWitness1Email { get; set; }
		public string LandlordWitness1Mobile { get; set; }
		public string LandlordWitness2Name { get; set; }
		public string LandlordWitness2Surname { get; set; }
		public string LandlordWitness2Email { get; set; }
		public string LandlordWitness2Mobile { get; set; }
		public string AgentWitness1Name { get; set; }
		public string AgentWitness1Surname { get; set; }
		public string AgentWitness1Email { get; set; }
		public string AgentWitness1Mobile { get; set; }
		public string AgentWitness2Name { get; set; }
		public string AgentWitness2Surname { get; set; }
		public string AgentWitness2Email { get; set; }
		public string AgentWitness2Mobile { get; set; }
		public int? AgentWitness1SignatureId { get; set; }
		public Picture AgentWitness1Signature { get; set; }
		public int? AgentWitness1InitialsId { get; set; }
		public Picture AgentWitness1Initials { get; set; }
		public int? AgentWitness2SignatureId { get; set; }
		public Picture AgentWitness2Signature { get; set; }
		public int? AgentWitness2InitialsId { get; set; }
		public Picture AgentWitness2Initials { get; set; }
		public int? LandlordWitness1SignatureId { get; set; }
		public Picture LandlordWitness1Signature { get; set; }
		public int? LandlordWitness1InitialsId { get; set; }
		public Picture LandlordWitness1Initials { get; set; }
		public int? LandlordWitness2SignatureId { get; set; }
		public Picture LandlordWitness2Signature { get; set; }
		public int? LandlordWitness2InitialsId { get; set; }
		public Picture LandlordWitness2Initials { get; set; }
		public string WitnessStatusEnum { get; set; }
		public WitnessStatus WitnessStatus
		{
			get => (WitnessStatus)Enum.Parse(typeof(WitnessStatus), WitnessStatusEnum);
			set => WitnessStatusEnum = value.ToString();
		}
		public string LandlordWitness1SignedAt { get; set; }
		public string LandlordWitness2SignedAt { get; set; }
		public string AgentWitness1SignedAt { get; set; }
		public string AgentWitness2SignedAt { get; set; }
		public DateTime? LandlordWitness1SignedOn { get; set; }
		public DateTime? LandlordWitness2SignedOn { get; set; }
		public DateTime? AgentWitness1SignedOn { get; set; }
		public DateTime? AgentWitness2SignedOn { get; set; }
	}
}
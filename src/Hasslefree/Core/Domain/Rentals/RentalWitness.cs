using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalWitness : BaseEntity
	{
		public RentalWitness()
		{
			this.CreatedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int RentalWitnessId { get; set; }
		public DateTime CreatedOn { get; set; }
		public Guid UniqueId { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string Witness1Name { get; set; }
		public string Witness1Surname { get; set; }
		public string Witness1Email { get; set; }
		public string Witness1Mobile { get; set; }
		public string Witness2Name { get; set; }
		public string Witness2Surname { get; set; }
		public string Witness2Email { get; set; }
		public string Witness2Mobile { get; set; }
		public int AgentWitness1Id { get; set; }
		public Picture AgentWitness1 { get; set; }
		public int AgentWitness2Id { get; set; }
		public Picture AgentWitness2 { get; set; }
		public int LandlordWitness1Id { get; set; }
		public Picture LandlordWitness1 { get; set; }
		public int LandlordWitness2Id { get; set; }
		public Picture LandlordWitness2 { get; set; }
		public string WitnessStatusEnum { get; set; }
		public WitnessStatus WitnessStatus
		{
			get => (WitnessStatus)Enum.Parse(typeof(WitnessStatus), WitnessStatusEnum);
			set => WitnessStatusEnum = value.ToString();
		}
	}
}

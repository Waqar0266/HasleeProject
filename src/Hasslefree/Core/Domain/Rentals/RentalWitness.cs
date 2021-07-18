using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalWitness : BaseEntity
	{
		public int RentalWitnessId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public int AgentWitness1Id { get; set; }
		public Picture AgentWitness1 { get; set; }
		public int AgentWitness2Id { get; set; }
		public Picture AgentWitness2 { get; set; }
		public int LandlordWitness1Id { get; set; }
		public Picture LandlordWitness1 { get; set; }
		public int LandlordWitness2Id { get; set; }
		public Picture LandlordWitness2 { get; set; }
	}
}

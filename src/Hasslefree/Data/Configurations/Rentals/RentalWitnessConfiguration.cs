using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalWitnessConfiguration : EntityTypeConfiguration<RentalWitness>
	{
		public RentalWitnessConfiguration()
		{
			// Table
			ToTable("RentalWitness");

			// Primary Key
			HasKey(a => a.RentalWitnessId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.AgentWitness1)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness1Id)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.AgentWitness2)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness2Id)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.LandlordWitness1)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness1Id)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.LandlordWitness2)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness2Id)
			.WillCascadeOnDelete(false);
		}
	}
}

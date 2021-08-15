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

			Property(a => a.UniqueId).IsRequired();

			Property(a => a.Witness1Name).IsRequired().HasMaxLength(55);
			Property(a => a.Witness1Surname).IsRequired().HasMaxLength(100);
			Property(a => a.Witness1Email).IsRequired().HasMaxLength(155);
			Property(a => a.Witness1Mobile).IsRequired().HasMaxLength(30);

			Property(a => a.Witness2Name).IsRequired().HasMaxLength(55);
			Property(a => a.Witness2Surname).IsRequired().HasMaxLength(100);
			Property(a => a.Witness2Email).IsRequired().HasMaxLength(155);
			Property(a => a.Witness2Mobile).IsRequired().HasMaxLength(30);

			Property(a => a.WitnessStatusEnum).IsRequired().HasMaxLength(20);

			Ignore(a => a.WitnessStatus);
		}
	}
}

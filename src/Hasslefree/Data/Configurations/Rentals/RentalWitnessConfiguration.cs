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

			HasOptional(a => a.AgentWitness1)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness1Id)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness2)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness2Id)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness1)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness1Id)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness2)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness2Id)
			.WillCascadeOnDelete(false);

			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();

			Property(a => a.UniqueId).IsRequired();

			Property(a => a.LandlordWitness1Name).IsOptional().HasMaxLength(55);
			Property(a => a.LandlordWitness1Surname).IsOptional().HasMaxLength(100);
			Property(a => a.LandlordWitness1Email).IsOptional().HasMaxLength(155);
			Property(a => a.LandlordWitness1Mobile).IsOptional().HasMaxLength(30);

			Property(a => a.LandlordWitness2Name).IsOptional().HasMaxLength(55);
			Property(a => a.LandlordWitness2Surname).IsOptional().HasMaxLength(100);
			Property(a => a.LandlordWitness2Email).IsOptional().HasMaxLength(155);
			Property(a => a.LandlordWitness2Mobile).IsOptional().HasMaxLength(30);

			Property(a => a.AgentWitness1Name).IsOptional().HasMaxLength(55);
			Property(a => a.AgentWitness1Surname).IsOptional().HasMaxLength(100);
			Property(a => a.AgentWitness1Email).IsOptional().HasMaxLength(155);
			Property(a => a.AgentWitness1Mobile).IsOptional().HasMaxLength(30);

			Property(a => a.AgentWitness2Name).IsOptional().HasMaxLength(55);
			Property(a => a.AgentWitness2Surname).IsOptional().HasMaxLength(100);
			Property(a => a.AgentWitness2Email).IsOptional().HasMaxLength(155);
			Property(a => a.AgentWitness2Mobile).IsOptional().HasMaxLength(30);

			Property(a => a.WitnessStatusEnum).IsRequired().HasMaxLength(20);

			Ignore(a => a.WitnessStatus);
		}
	}
}

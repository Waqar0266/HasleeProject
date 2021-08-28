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

			HasOptional(a => a.AgentWitness1Signature)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness1SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness1Initials)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness1InitialsId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness2Signature)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness2SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.AgentWitness2Initials)
			.WithMany()
			.HasForeignKey(a => a.AgentWitness2InitialsId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness1Signature)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness1SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness1Initials)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness1InitialsId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness2Signature)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness2SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.LandlordWitness2Initials)
			.WithMany()
			.HasForeignKey(a => a.LandlordWitness2InitialsId)
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

			Property(a => a.AgentWitness1SignedAt).IsOptional().HasMaxLength(100);
			Property(a => a.AgentWitness2SignedAt).IsOptional().HasMaxLength(100);
			Property(a => a.LandlordWitness1SignedAt).IsOptional().HasMaxLength(100);
			Property(a => a.LandlordWitness2SignedAt).IsOptional().HasMaxLength(100);
			Property(a => a.AgentWitness1SignedOn).IsOptional();
			Property(a => a.AgentWitness2SignedOn).IsOptional();
			Property(a => a.LandlordWitness1SignedOn).IsOptional();
			Property(a => a.LandlordWitness2SignedOn).IsOptional();

			Ignore(a => a.WitnessStatus);
		}
	}
}

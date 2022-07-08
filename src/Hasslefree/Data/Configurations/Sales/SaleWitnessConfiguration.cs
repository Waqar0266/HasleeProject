using Hasslefree.Core.Domain.Sales;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Sales
{
    public class SaleWitnessConfiguration : EntityTypeConfiguration<SaleWitness>
    {
        public SaleWitnessConfiguration()
        {
            // Table
            ToTable("SaleWitness");

            // Primary Key
            HasKey(a => a.SaleWitnessId);

            HasRequired(a => a.Sale)
            .WithMany()
            .HasForeignKey(a => a.SaleId)
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

            HasOptional(a => a.SellerWitness1Signature)
            .WithMany()
            .HasForeignKey(a => a.SellerWitness1SignatureId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.SellerWitness1Initials)
            .WithMany()
            .HasForeignKey(a => a.SellerWitness1InitialsId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.SellerWitness2Signature)
            .WithMany()
            .HasForeignKey(a => a.SellerWitness2SignatureId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.SellerWitness2Initials)
            .WithMany()
            .HasForeignKey(a => a.SellerWitness2InitialsId)
            .WillCascadeOnDelete(false);

            Property(a => a.CreatedOn).IsRequired();
            Property(a => a.ModifiedOn).IsRequired();

            Property(a => a.UniqueId).IsRequired();

            Property(a => a.SellerWitness1Name).IsOptional().HasMaxLength(55);
            Property(a => a.SellerWitness1Surname).IsOptional().HasMaxLength(100);
            Property(a => a.SellerWitness1Email).IsOptional().HasMaxLength(155);
            Property(a => a.SellerWitness1Mobile).IsOptional().HasMaxLength(30);

            Property(a => a.SellerWitness2Name).IsOptional().HasMaxLength(55);
            Property(a => a.SellerWitness2Surname).IsOptional().HasMaxLength(100);
            Property(a => a.SellerWitness2Email).IsOptional().HasMaxLength(155);
            Property(a => a.SellerWitness2Mobile).IsOptional().HasMaxLength(30);

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
            Property(a => a.SellerWitness1SignedAt).IsOptional().HasMaxLength(100);
            Property(a => a.SellerWitness2SignedAt).IsOptional().HasMaxLength(100);
            Property(a => a.AgentWitness1SignedOn).IsOptional();
            Property(a => a.AgentWitness2SignedOn).IsOptional();
            Property(a => a.SellerWitness1SignedOn).IsOptional();
            Property(a => a.SellerWitness2SignedOn).IsOptional();

            Ignore(a => a.WitnessStatus);
        }
    }
}

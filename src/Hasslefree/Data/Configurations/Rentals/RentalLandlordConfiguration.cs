using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
    public class RentalLandlordConfiguration : EntityTypeConfiguration<RentalLandlord>
    {
        public RentalLandlordConfiguration()
        {
            // Table
            ToTable("RentalLandlord");

            // Primary Key
            HasKey(a => a.RentalLandlordId);

            HasRequired(a => a.Rental)
            .WithMany(a => a.Landlords)
            .HasForeignKey(a => a.RentalId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.Initials)
            .WithMany()
            .HasForeignKey(a => a.InitialsId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.Person)
            .WithMany()
            .HasForeignKey(a => a.PersonId)
            .WillCascadeOnDelete(false);

            HasOptional(a => a.Signature)
            .WithMany()
            .HasForeignKey(a => a.SignatureId)
            .WillCascadeOnDelete(false);

            // Columns
            Property(a => a.CreatedOn).IsRequired();
            Property(a => a.ModifiedOn).IsRequired();
            Property(a => a.IdNumber).IsRequired().HasMaxLength(30);
            Property(a => a.IncomeTaxNumber).IsOptional().HasMaxLength(30);
            Property(a => a.Tempdata).IsOptional().HasMaxLength(1000);
            Property(a => a.VatNumber).IsOptional().HasMaxLength(50);
            Property(a => a.SignedAt).IsOptional().HasMaxLength(100);
            Property(a => a.SignedOn).IsOptional();
        }
    }
}

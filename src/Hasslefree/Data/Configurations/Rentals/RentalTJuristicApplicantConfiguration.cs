using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalTJuristicApplicantConfiguration : EntityTypeConfiguration<RentalTJuristicApplicant>
	{
		public RentalTJuristicApplicantConfiguration()
		{
			// Table
			ToTable("RentalTJuristicApplicant");

			// Primary Key
			HasKey(a => a.RentalTJuristicApplicantId);

			HasRequired(a => a.RentalTJuristic)
			.WithMany()
			.HasForeignKey(a => a.RentalTJuristicId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Initials)
			.WithMany()
			.HasForeignKey(a => a.InitialsId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Signature)
			.WithMany()
			.HasForeignKey(a => a.SignatureId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Email).IsOptional().HasMaxLength(100);
			Property(a => a.Fax).IsOptional().HasMaxLength(20);
			Property(a => a.IdNumber).IsOptional().HasMaxLength(50);
			Property(a => a.Mobile).IsOptional().HasMaxLength(20);
			Property(a => a.Name).IsOptional().HasMaxLength(50);
			Property(a => a.Nationality).IsOptional().HasMaxLength(50);
			Property(a => a.Position).IsOptional().HasMaxLength(50);
			Property(a => a.SignedAt).IsOptional().HasMaxLength(50);
			Property(a => a.SignedOn).IsOptional();
			Property(a => a.Surname).IsOptional().HasMaxLength(100);
			Property(a => a.TelHome).IsOptional().HasMaxLength(20);
			Property(a => a.TelWork).IsOptional().HasMaxLength(20);
		}
	}
}

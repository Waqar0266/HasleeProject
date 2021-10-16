using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalTLandlordConfiguration : EntityTypeConfiguration<RentalTLandlord>
	{
		public RentalTLandlordConfiguration()
		{
			// Table
			ToTable("RentalTLandlord");

			// Primary Key
			HasKey(a => a.RentalTLandlordId);

			HasRequired(a => a.RentalT)
			.WithMany()
			.HasForeignKey(a => a.RentalTId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Initials)
			.WithMany()
			.HasForeignKey(a => a.InitialsId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Signature)
			.WithMany()
			.HasForeignKey(a => a.SignatureId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Person)
			.WithMany()
			.HasForeignKey(a => a.PersonId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.AccountNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Bank).IsOptional().HasMaxLength(50);
			Property(a => a.BankReference).IsOptional().HasMaxLength(50);
			Property(a => a.Branch).IsOptional().HasMaxLength(50);
			Property(a => a.BranchCode).IsOptional().HasMaxLength(10);
			Property(a => a.Email).IsOptional().HasMaxLength(100);
			Property(a => a.Fax).IsOptional().HasMaxLength(20);
			Property(a => a.IdNumber).IsOptional().HasMaxLength(30);
			Property(a => a.IncomeTaxNumber).IsOptional().HasMaxLength(50);
			Property(a => a.Mobile).IsOptional().HasMaxLength(30);
			Property(a => a.SignedAt).IsOptional().HasMaxLength(50);
			Property(a => a.SignedOn).IsOptional();
			Property(a => a.TelHome).IsOptional().HasMaxLength(20);
			Property(a => a.TelWork).IsOptional().HasMaxLength(20);
			Property(a => a.Tempdata).IsOptional().HasMaxLength(255);
			Property(a => a.TypeOfAccount).IsOptional().HasMaxLength(30);
			Property(a => a.VatNumber).IsOptional().HasMaxLength(50);
		}
	}
}

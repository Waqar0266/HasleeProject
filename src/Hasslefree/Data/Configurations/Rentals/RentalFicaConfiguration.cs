using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalFicaConfiguration : EntityTypeConfiguration<RentalFica>
	{
		public RentalFicaConfiguration()
		{
			// Table
			ToTable("RentalFica");

			// Primary Key
			HasKey(a => a.RentalFicaId);

			HasOptional(a => a.BranchAddress)
			.WithMany()
			.HasForeignKey(a => a.BranchAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.HeadOfficeAddress)
			.WithMany()
			.HasForeignKey(a => a.HeadOfficeAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner1Address)
			.WithMany()
			.HasForeignKey(a => a.Partner1AddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner2Address)
			.WithMany()
			.HasForeignKey(a => a.Partner2AddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner3Address)
			.WithMany()
			.HasForeignKey(a => a.Partner3AddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.RegisteredAddress)
			.WithMany()
			.HasForeignKey(a => a.RegisteredAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner1Signature)
			.WithMany()
			.HasForeignKey(a => a.Partner1SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner2Signature)
			.WithMany()
			.HasForeignKey(a => a.Partner2SignatureId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Partner3Signature)
			.WithMany()
			.HasForeignKey(a => a.Partner3SignatureId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.CompanyTypeEnum).IsOptional().HasMaxLength(100);
			Property(a => a.Email).IsOptional().HasMaxLength(100);
			Property(a => a.Fax).IsOptional().HasMaxLength(30);
			Property(a => a.Mobile).IsOptional().HasMaxLength(30);
			Property(a => a.Partner1Email).IsOptional().HasMaxLength(100);
			Property(a => a.Partner1Fax).IsOptional().HasMaxLength(30);
			Property(a => a.Partner1IdNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Partner1Mobile).IsOptional().HasMaxLength(30);
			Property(a => a.Partner1Name).IsOptional().HasMaxLength(50);
			Property(a => a.Partner1Nationality).IsOptional().HasMaxLength(55);
			Property(a => a.Partner1Phone).IsOptional().HasMaxLength(30);
			Property(a => a.Partner1Surname).IsOptional().HasMaxLength(50);
			Property(a => a.Partner1Work).IsOptional().HasMaxLength(30);
			Property(a => a.Partner2Email).IsOptional().HasMaxLength(100);
			Property(a => a.Partner2Fax).IsOptional().HasMaxLength(30);
			Property(a => a.Partner2IdNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Partner2Mobile).IsOptional().HasMaxLength(30);
			Property(a => a.Partner2Name).IsOptional().HasMaxLength(50);
			Property(a => a.Partner2Nationality).IsOptional().HasMaxLength(55);
			Property(a => a.Partner2Phone).IsOptional().HasMaxLength(30);
			Property(a => a.Partner2Surname).IsOptional().HasMaxLength(50);
			Property(a => a.Partner2Work).IsOptional().HasMaxLength(30);
			Property(a => a.Partner3Email).IsOptional().HasMaxLength(100);
			Property(a => a.Partner3Fax).IsOptional().HasMaxLength(30);
			Property(a => a.Partner3IdNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Partner3Mobile).IsOptional().HasMaxLength(30);
			Property(a => a.Partner3Name).IsOptional().HasMaxLength(50);
			Property(a => a.Partner3Nationality).IsOptional().HasMaxLength(55);
			Property(a => a.Partner3Phone).IsOptional().HasMaxLength(30);
			Property(a => a.Partner3Surname).IsOptional().HasMaxLength(50);
			Property(a => a.Partner3Work).IsOptional().HasMaxLength(30);
			Property(a => a.Phone).IsOptional().HasMaxLength(30);
			Property(a => a.RegisteredBusinessName).IsOptional().HasMaxLength(255);
			Property(a => a.RegistrationNumber).IsOptional().HasMaxLength(50);
			Property(a => a.StaffMember).IsOptional().HasMaxLength(100);
			Property(a => a.TradeName).IsOptional().HasMaxLength(100);
			Property(a => a.TransactionType).IsOptional().HasMaxLength(100);
			Property(a => a.Work).IsOptional().HasMaxLength(30);
			Property(a => a.HeldAt).IsOptional().HasMaxLength(100);
			Property(a => a.HeldOn).IsOptional();
		}
	}
}

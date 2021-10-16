using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalTJuristicConfiguration : EntityTypeConfiguration<RentalTJuristic>
	{
		public RentalTJuristicConfiguration()
		{
			// Table
			ToTable("RentalTJuristic");

			// Primary Key
			HasKey(a => a.RentalTJuristicId);

			HasRequired(a => a.RentalT)
			.WithMany()
			.HasForeignKey(a => a.RentalTId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.BusinessAddress)
			.WithMany()
			.HasForeignKey(a => a.BusinessAddressId)
			.WillCascadeOnDelete(false);

			HasRequired(a => a.PostalAddress)
			.WithMany()
			.HasForeignKey(a => a.PostalAddressId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.AccountNumber).IsOptional().HasMaxLength(50);
			Property(a => a.AccountType).IsOptional().HasMaxLength(50);
			Property(a => a.Address).IsOptional().HasMaxLength(255);
			Property(a => a.Bank).IsOptional().HasMaxLength(50);
			Property(a => a.BranchCode).IsOptional().HasMaxLength(10);
			Property(a => a.BranchName).IsOptional().HasMaxLength(20);
			Property(a => a.ContactPersonFirstName).IsOptional().HasMaxLength(50);
			Property(a => a.ContactPersonPosition).IsOptional().HasMaxLength(50);
			Property(a => a.ContactPersonSurname).IsOptional().HasMaxLength(50);
			Property(a => a.DateOfIncorporation).IsOptional();
			Property(a => a.Email).IsOptional().HasMaxLength(100);
			Property(a => a.EntityName).IsOptional().HasMaxLength(100);
			Property(a => a.Fax).IsOptional().HasMaxLength(20);
			Property(a => a.FinancialYearEnd).IsOptional();
			Property(a => a.Mobile).IsOptional().HasMaxLength(20);
			Property(a => a.NatureOfBusiness).IsOptional().HasMaxLength(50);
			Property(a => a.PeriodInBusiness).IsOptional().HasMaxLength(30);
			Property(a => a.RegisteredName).IsOptional().HasMaxLength(100);
			Property(a => a.RegistrationNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Tel).IsOptional().HasMaxLength(20);
			Property(a => a.TradingName).IsOptional().HasMaxLength(50);
			Property(a => a.VatRegistrationNumber).IsOptional().HasMaxLength(30);
		}
	}
}

using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class LandlordBankAccountConfiguration : EntityTypeConfiguration<LandlordBankAccount>
	{
		public LandlordBankAccountConfiguration()
		{
			// Table
			ToTable("LandlordBankAccount");

			// Primary Key
			HasKey(a => a.LandlordBankAccountId);

			HasRequired(a => a.Rental)
			.WithMany()
			.HasForeignKey(a => a.RentalId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.AccountHolder).IsRequired().HasMaxLength(50);
			Property(a => a.AccountNumber).IsRequired().HasMaxLength(30);
			Property(a => a.Bank).IsRequired().HasMaxLength(100);
			Property(a => a.BankReference).IsRequired().HasMaxLength(100);
			Property(a => a.Branch).IsRequired().HasMaxLength(100);
			Property(a => a.BranchCode).IsRequired().HasMaxLength(10);
		}
	}
}

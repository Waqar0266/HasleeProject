using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class TenantConfiguration : EntityTypeConfiguration<Tenant>
	{
		public TenantConfiguration()
		{
			// Table
			ToTable("Tenant");

			// Primary Key
			HasKey(a => a.TenantId);

			HasRequired(a => a.RentalT)
			.WithMany()
			.HasForeignKey(a => a.RentalTId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.EmployerAddress)
			.WithMany()
			.HasForeignKey(a => a.EmployerAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Initials)
			.WithMany()
			.HasForeignKey(a => a.InitialsId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.PhysicalAddress)
			.WithMany()
			.HasForeignKey(a => a.PhysicalAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.PostalAddress)
			.WithMany()
			.HasForeignKey(a => a.PostalAddressId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Signature)
			.WithMany()
			.HasForeignKey(a => a.SignatureId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.AccountNumber).IsOptional().HasMaxLength(30);
			Property(a => a.Bank).IsOptional().HasMaxLength(30);
			Property(a => a.BankReference).IsOptional().HasMaxLength(50);
			Property(a => a.Branch).IsOptional().HasMaxLength(50);
			Property(a => a.BranchCode).IsOptional().HasMaxLength(10);
			Property(a => a.CurrentEmployer).IsOptional().HasMaxLength(100);
			Property(a => a.CurrentMonthlyExpenses).IsOptional().HasPrecision(15, 5);
			Property(a => a.Email).IsOptional().HasMaxLength(100);
			Property(a => a.Tempdata).IsOptional().HasMaxLength(800);
			Property(a => a.Fax).IsOptional().HasMaxLength(20);
			Property(a => a.GrossMonthlySalary).IsOptional().HasPrecision(15, 5);
			Property(a => a.MainApplicant).IsRequired();
			Property(a => a.Married).IsRequired();
			Property(a => a.MarriedType).IsOptional().HasMaxLength(50);
			Property(a => a.Mobile).IsOptional().HasMaxLength(20);
			Property(a => a.Nationality).IsOptional().HasMaxLength(50);
			Property(a => a.NetMonthlySalary).IsOptional().HasPrecision(15, 5);
			Property(a => a.NextOfKin).IsOptional().HasMaxLength(100);
			Property(a => a.Occupation).IsOptional().HasMaxLength(100);
			Property(a => a.OwnerOfProperty).IsRequired();
			Property(a => a.PeriodOfEmployment).IsOptional().HasMaxLength(50);
			Property(a => a.PreviousLandlord).IsOptional().HasMaxLength(100);
			Property(a => a.PreviousLandlordContactNumber).IsOptional().HasMaxLength(20);
			Property(a => a.PreviousRentPaid).IsOptional().HasPrecision(15, 5);
			Property(a => a.PreviousStayDuration).IsOptional().HasMaxLength(50);
			Property(a => a.SalaryPaymentDate).IsOptional();
			Property(a => a.SelfEmployed).IsOptional();
			Property(a => a.TelWork).IsOptional().HasMaxLength(20);
			Property(a => a.TypeOfAccount).IsOptional().HasMaxLength(50);
			Property(a => a.VatNumber).IsOptional().HasMaxLength(30);
		}
	}
}

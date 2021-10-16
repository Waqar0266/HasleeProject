using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class Tenant : BaseEntity
	{
		public Tenant()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int TenantId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid UniqueId { get; set; }
		public int RentalTId { get; set; }
		public RentalT RentalT { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string MaidenName { get; set; }
		public string IdNumber { get; set; }
		public string VatNumber { get; set; }
		public string Nationality { get; set; }
		public bool Married { get; set; }
		public string MarriedType { get; set; }
		public string TelHome { get; set; }
		public string TelWork { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public int? PhysicalAddressId { get; set; }
		public Address PhysicalAddress { get; set; }
		public int? PostalAddressId { get; set; }
		public Address PostalAddress { get; set; }
		public string NextOfKin { get; set; }
		public bool OwnerOfProperty { get; set; }
		public decimal? PreviousRentPaid { get; set; }
		public string PreviousStayDuration { get; set; }
		public string PreviousLandlord { get; set; }
		public string PreviousLandlordContactNumber { get; set; }
		public string Bank { get; set; }
		public string Branch { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string TypeOfAccount { get; set; }
		public string BankReference { get; set; }
		public bool SelfEmployed { get; set; }
		public string Occupation { get; set; }
		public string CurrentEmployer { get; set; }
		public int? EmployerAddressId { get; set; }
		public Address EmployerAddress { get; set; }
		public string PeriodOfEmployment { get; set; }
		public decimal? GrossMonthlySalary { get; set; }
		public decimal? NetMonthlySalary { get; set; }
		public DateTime? SalaryPaymentDate { get; set; }
		public decimal? CurrentMonthlyExpenses { get; set; }
		public bool MainApplicant { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public string SignedAt { get; set; }
		public DateTime? SignedOn { get; set; }
	}
}

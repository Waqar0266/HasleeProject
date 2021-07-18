using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class LandlordBankAccount : BaseEntity
	{
		public int LandlordBankAccountId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string AccountHolder { get; set; }
		public string Bank { get; set; }
		public string Branch { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string BankReference { get; set; }
	}
}
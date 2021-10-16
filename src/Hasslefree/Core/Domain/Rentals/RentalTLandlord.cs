using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTLandlord : BaseEntity
	{
		public RentalTLandlord()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int RentalTLandlordId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid UniqueId { get; set; }
		public string Tempdata { get; set; }
		public string IdNumber { get; set; }
		public int? PersonId { get; set; }
		public Person Person { get; set; }
		public int RentalTId { get; set; }
		public RentalT RentalT { get; set; }
		public string VatNumber { get; set; }
		public string IncomeTaxNumber { get; set; }
		public string Bank { get; set; }
		public string Branch { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string TypeOfAccount { get; set; }
		public string BankReference { get; set; }
		public string TelHome { get; set; }
		public string TelWork { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
		public string SignedAt { get; set; }
		public DateTime? SignedOn { get; set; }
	}
}

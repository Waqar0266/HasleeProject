using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalLandlord : BaseEntity
	{
		public RentalLandlord()
        {
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
        }

		public int RentalLandlordId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Tempdata { get; set; }
		public string IdNumber { get; set; }
		public int? PersonId { get; set; }
		public Person Person { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string VatNumber { get; set; }
		public string IncomeTaxNumber { get; set; }
		public int? PhysicalAddressId { get; set; }
		public Address PhysicalAddress { get; set; }
		public int? PostalAddressId { get; set; }
		public Address PostalAddress { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
	}
}

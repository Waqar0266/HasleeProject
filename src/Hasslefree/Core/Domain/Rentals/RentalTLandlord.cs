using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTLandlord : BaseEntity
	{
		public int RentalTLandlordId { get; set; }
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
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
		public string SignedAt { get; set; }
		public DateTime? SignedOn { get; set; }
	}
}

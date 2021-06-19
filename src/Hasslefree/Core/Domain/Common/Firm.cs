using System;

namespace Hasslefree.Core.Domain.Common
{
	public class Firm : BaseEntity
	{
		public int FirmId { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string BusinessName { get; set; }
		public string TradeName { get; set; }
		public int PhysicalAddressId { get; set; }
		public Address PhysicalAddress { get; set; }
		public int PostalAddressId { get; set; }
		public Address PostalAddress { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string ReferenceNumber { get; set; }
		public string AiNumber { get; set; }
	}
}

using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalFica : BaseEntity
	{
		public RentalFica()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int RentalFicaId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public string RegisteredBusinessName { get; set; }
		public string RegistrationNumber { get; set; }
		public string CompanyTypeEnum { get; set; }
		public CompanyType CompanyType
		{
			get => (CompanyType)Enum.Parse(typeof(CompanyType), CompanyTypeEnum ?? CompanyType.ClosedCorporation.ToString());
			set => CompanyTypeEnum = value.ToString();
		}
		public int? RegisteredAddressId { get; set; }
		public Address RegisteredAddress { get; set; }
		public int? HeadOfficeAddressId { get; set; }
		public Address HeadOfficeAddress { get; set; }
		public int? BranchAddressId { get; set; }
		public Address BranchAddress { get; set; }
		public string TradeName { get; set; }
		public string Phone { get; set; }
		public string Work { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string Partner1Name { get; set; }
		public string Partner1Surname { get; set; }
		public string Partner1IdNumber { get; set; }
		public string Partner1Nationality { get; set; }
		public int? Partner1AddressId { get; set; }
		public Address Partner1Address { get; set; }
		public string Partner1Phone { get; set; }
		public string Partner1Work { get; set; }
		public string Partner1Fax { get; set; }
		public string Partner1Mobile { get; set; }
		public string Partner1Email { get; set; }
		public int? Partner1SignatureId { get; set; }
		public Picture Partner1Signature { get; set; }
		public string Partner2Name { get; set; }
		public string Partner2Surname { get; set; }
		public string Partner2IdNumber { get; set; }
		public string Partner2Nationality { get; set; }
		public int? Partner2AddressId { get; set; }
		public Address Partner2Address { get; set; }
		public string Partner2Phone { get; set; }
		public string Partner2Work { get; set; }
		public string Partner2Fax { get; set; }
		public string Partner2Mobile { get; set; }
		public string Partner2Email { get; set; }
		public int? Partner2SignatureId { get; set; }
		public Picture Partner2Signature { get; set; }
		public string Partner3Name { get; set; }
		public string Partner3Surname { get; set; }
		public string Partner3IdNumber { get; set; }
		public string Partner3Nationality { get; set; }
		public int? Partner3AddressId { get; set; }
		public Address Partner3Address { get; set; }
		public string Partner3Phone { get; set; }
		public string Partner3Work { get; set; }
		public string Partner3Fax { get; set; }
		public string Partner3Mobile { get; set; }
		public string Partner3Email { get; set; }
		public int? Partner3SignatureId { get; set; }
		public Picture Partner3Signature { get; set; }
		public string StaffMember { get; set; }
		public string TransactionType { get; set; }
	}
}

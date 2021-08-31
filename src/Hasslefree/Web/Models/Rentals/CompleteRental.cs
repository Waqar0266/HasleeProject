using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRental
	{
		public CompleteRental()
		{
			this.Titles = new SelectList(new[]
							   {
						   new SelectListItem {Text = "Mr", Value = "Mr"},
						   new SelectListItem {Text = "Miss", Value = "Miss"},
						   new SelectListItem {Text = "Mrs", Value = "Mrs"},
						   new SelectListItem {Text = "Advocate", Value = "Advocate"},
						   new SelectListItem {Text = "Professor", Value = "Professor"},
						   new SelectListItem {Text = "Doctor", Value = "Doctor"},
						   new SelectListItem {Text = "Other", Value = "Other"}
					   }, "Text", "Value");

			this.Genders = new SelectList(new[]
			{
						   new SelectListItem {Text = "Male", Value = "Male"},
						   new SelectListItem {Text = "Female", Value = "Female" }
					   }, "Text", "Value");

			this.Members = new List<CompleteRentalMember>();
		}

		public string RentalGuid { get; set; }
		public int RentalId { get; set; }
		public LeaseType LeaseType { get; set; }
		public string RentalLandlordId { get; set; }
		public string Title { get; set; }
		public Gender Gender { get; set; }
		public SelectList Titles { get; set; }
		public SelectList Genders { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string IdNumber { get; set; }
		public string Mobile { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public string VatNumber { get; set; }
		public string IncomeTaxNumber { get; set; }
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Township { get; set; }
		public string Address { get; set; }
		public decimal MonthlyRental { get; set; }
		public decimal Deposit { get; set; }
		public decimal Rental { get; set; }
		public DateTime? RentalPaymentDate { get; set; }
		public DateTime? DepositPaymentDate { get; set; }
		public decimal? Procurement1Percentage { get; set; }
		public decimal? Procurement1Amount { get; set; }
		public decimal? Procurement2Percentage { get; set; }
		public decimal? Procurement2Amount { get; set; }
		public decimal? Procurement3Percentage { get; set; }
		public decimal? Procurement3Amount { get; set; }
		public decimal? ManagementPercentage { get; set; }
		public decimal? ManagementAmount { get; set; }
		public decimal? SalePercentage { get; set; }
		public decimal? SaleAmount { get; set; }
		public bool PowerOfAttorney { get; set; }
		public bool Marketing { get; set; }
		public string AccountHolder { get; set; }
		public string Bank { get; set; }
		public string Branch { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string BankReference { get; set; }
		public bool Procurement { get; set; }
		public bool Management { get; set; }
		public bool Negotiating { get; set; }
		public bool Informing { get; set; }
		public bool IncomingSnaglist { get; set; }
		public bool OutgoingSnaglist { get; set; }
		public bool Explaining { get; set; }
		public bool PayingLandlord { get; set; }
		public bool ContactLandlord { get; set; }
		public bool ProvideLandlord { get; set; }
		public bool AskLandlordConsent { get; set; }
		public bool ProcureDepositLandlord { get; set; }
		public bool ProcureDepositPreviousRentalAgent { get; set; }
		public string ProcureDepositOther { get; set; }
		public bool TransferDeposit { get; set; }

		public string SpecificRequirements { get; set; }
		public string SpecialConditions { get; set; }
		//Residential Address
		public string ResidentialAddress1 { get; set; }
		public string ResidentialAddress2 { get; set; }
		public string ResidentialAddress3 { get; set; }
		public string ResidentialAddressTown { get; set; }
		public string ResidentialAddressCode { get; set; }
		public string ResidentialAddressCountry { get; set; }
		public string ResidentialAddressProvince { get; set; }

		//Postal Address
		public string PostalAddress1 { get; set; }
		public string PostalAddress2 { get; set; }
		public string PostalAddress3 { get; set; }
		public string PostalAddressTown { get; set; }
		public string PostalAddressCode { get; set; }
		public string PostalAddressCountry { get; set; }
		public string PostalAddressProvince { get; set; }

		//Close Corporation
		public string RegisteredBusinessName { get; set; }
		public string RegistrationNumber { get; set; }
		public CompanyType CompanyType { get; set; }
		public string RegisteredAddress1 { get; set; }
		public string RegisteredAddress2 { get; set; }
		public string RegisteredAddress3 { get; set; }
		public string RegisteredAddressTown { get; set; }
		public string RegisteredAddressCode { get; set; }
		public string RegisteredAddressCountry { get; set; }
		public string RegisteredAddressProvince { get; set; }
		public string TradeName { get; set; }
		public string HeldAt { get; set; }
		public string HeldOn { get; set; }
		public string LeaseName { get; set; }
		public string AuthorizedName { get; set; }
		public string AuthorizedSurname { get; set; }
		public string HeadOfficeAddress1 { get; set; }
		public string HeadOfficeAddress2 { get; set; }
		public string HeadOfficeAddress3 { get; set; }
		public string HeadOfficeAddressTown { get; set; }
		public string HeadOfficeAddressCode { get; set; }
		public string HeadOfficeAddressCountry { get; set; }
		public string HeadOfficeAddressProvince { get; set; }
		public string BranchAddress1 { get; set; }
		public string BranchAddress2 { get; set; }
		public string BranchAddress3 { get; set; }
		public string BranchAddressTown { get; set; }
		public string BranchAddressCode { get; set; }
		public string BranchAddressCountry { get; set; }
		public string BranchAddressProvince { get; set; }
		public string Partner1Name { get; set; }
		public string Partner1Surname { get; set; }
		public string Partner1IdNumber { get; set; }
		public string Partner1Nationality { get; set; }
		public string Partner1Address1 { get; set; }
		public string Partner1Address2 { get; set; }
		public string Partner1Address3 { get; set; }
		public string Partner1AddressCity { get; set; }
		public string Partner1AddressProvince { get; set; }
		public string Partner1AddressCountry { get; set; }
		public string Partner1AddressPostalCode { get; set; }
		public string Partner1Phone { get; set; }
		public string Partner1Work { get; set; }
		public string Partner1Fax { get; set; }
		public string Partner1Mobile { get; set; }
		public string Partner1Email { get; set; }
		public string Partner2Name { get; set; }
		public string Partner2Surname { get; set; }
		public string Partner2IdNumber { get; set; }
		public string Partner2Nationality { get; set; }
		public string Partner2Address1 { get; set; }
		public string Partner2Address2 { get; set; }
		public string Partner2Address3 { get; set; }
		public string Partner2AddressCity { get; set; }
		public string Partner2AddressProvince { get; set; }
		public string Partner2AddressCountry { get; set; }
		public string Partner2AddressPostalCode { get; set; }
		public string Partner2Phone { get; set; }
		public string Partner2Work { get; set; }
		public string Partner2Fax { get; set; }
		public string Partner2Mobile { get; set; }
		public string Partner2Email { get; set; }
		public string Partner3Name { get; set; }
		public string Partner3Surname { get; set; }
		public string Partner3IdNumber { get; set; }
		public string Partner3Nationality { get; set; }
		public string Partner3Address1 { get; set; }
		public string Partner3Address2 { get; set; }
		public string Partner3Address3 { get; set; }
		public string Partner3AddressCity { get; set; }
		public string Partner3AddressProvince { get; set; }
		public string Partner3AddressCountry { get; set; }
		public string Partner3AddressPostalCode { get; set; }
		public string Partner3Phone { get; set; }
		public string Partner3Work { get; set; }
		public string Partner3Fax { get; set; }
		public string Partner3Mobile { get; set; }
		public string Partner3Email { get; set; }
		public string StaffMember { get; set; }
		public string TransactionType { get; set; }
		public List<CompleteRentalMember> Members { get; set; }
	}

	public class CompleteRentalMember
	{
		public string IdNumber { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
	}
}

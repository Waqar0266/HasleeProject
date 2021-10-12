using Hasslefree.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTJuristic : BaseEntity
	{
		public RentalTJuristic()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
		}

		public int RentalTJuristicId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string EntityName { get; set; }
		public string Address { get; set; }
		public string RegisteredName { get; set; }
		public string TradingName { get; set; }
		public string RegistrationNumber { get; set; }
		public string VatRegistrationNumber { get; set; }
		public DateTime? DateOfIncorporation { get; set; }
		public string NatureOfBusiness { get; set; }
		public string PeriodInBusiness { get; set; }
		public DateTime? FinancialYearEnd { get; set; }
		public int? BusinessAddressId { get; set; }
		public Address BusinessAddress { get; set; }
		public int? PostalAddressId { get; set; }
		public Address PostalAddress { get; set; }
		public string ContactPersonFirstName { get; set; }
		public string ContactPersonSurname { get; set; }
		public string ContactPersonPosition { get; set; }
		public string Tel { get; set; }
		public string Mobile { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string Bank { get; set; }
		public string BranchName { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string AccountType { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalAgent
	{
		public string RentalGuid { get; set; }
		public string AgentGuid { get; set; }
		public string VatNumber { get; set; }
		public string FfcNumber { get; set; }
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Township { get; set; }
		public string Address { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal MonthlyRental { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal Deposit { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal Rental { get; set; }
		public DateTime? RentalPaymentDate { get; set; }
		public DateTime? DepositPaymentDate { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement1Percentage { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement1Amount { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement2Percentage { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement2Amount { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement3Percentage { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? Procurement3Amount { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? ManagementPercentage { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? ManagementAmount { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? SalePercentage { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public decimal? SaleAmount { get; set; }
		public bool Marketing { get; set; }
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
		public string Witness1Name { get; set; }
		public string Witness1Surname { get; set; }
		public string Witness1Email { get; set; }
		public string Witness1Mobile { get; set; }
		public string Witness2Name { get; set; }
		public string Witness2Surname { get; set; }
		public string Witness2Email { get; set; }
		public string Witness2Mobile { get; set; }
	}
}

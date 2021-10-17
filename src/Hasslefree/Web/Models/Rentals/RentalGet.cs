using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Rentals
{
	public class RentalGet
	{
		public int RentalId { get; set; }
		public Guid RentalGuid { get; set; }
		public RentalStatus RentalStatus { get; set; }
		public string Address { get; set; }
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Township { get; set; }
		public List<RentalLandlord> RentalLandlords { get; set; }
		public List<LandlordBankAccount> LandlordBankAccounts { get; set; }
		public List<RentalDocumentModel> LandlordDocumentation { get; set; }
		public List<RentalFormModel> Forms { get; set; }
		public RentalWitness RentalWitness { get; set; }
		public RentalMandate RentalMandate { get; set; }
		public RentalFica RentalFica { get; set; }
		public LeaseType LeaseType { get; set; }
		public Agent Agent { get; set; }
		public int? AgentId { get; set; }
		public bool Procurement { get; set; }
		public bool Marketing { get; set; }
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
		public decimal Deposit { get; set; }
		public DateTime DepositPaymentDate { get; set; }
		public decimal MonthlyRental { get; set; }
		public DateTime MonthlyPaymentDate { get; set; }
		public Person AgentPerson { get; set; }
		public Address LandlordPhysicalAddress { get; set; }
		public Address LandlordPostalAddress { get; set; }
		public Address AgentPhysicalAddress { get; set; }
		public Address AgentPostalAddress { get; set; }
		public bool PowerOfAttorney { get; set; }
		public RentalResolution RentalResolution { get; set; }
	}
}

using Hasslefree.Core.Domain.Rentals;
using System;

namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteExistingRental
	{
		public int ExistingRentalId { get; set; }
		public ExistingRentalType Option { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string AmendedAddendum { get; set; }
		public string Tenant { get; set; }
		public string ParkingBays { get; set; }
		public DateTime? TerminationDate { get; set; }
		public string RenewalPeriod { get; set; }
		public DateTime? RenewalCommencementDate { get; set; }
		public DateTime? RenewalTerminationDate { get; set; }
		public string Rent { get; set; }
		public string Deposit { get; set; }
		public string FurtherMaterialChanges { get; set; }
		public string Premises { get; set; }
		public string Witness1Name { get; set; }
		public string Witness1Surname { get; set; }
		public string Witness1Email { get; set; }
		public string Witness2Name { get; set; }
		public string Witness2Surname { get; set; }
		public string Witness2Email { get; set; }
	}
}

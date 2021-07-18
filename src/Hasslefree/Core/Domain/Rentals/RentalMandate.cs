using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalMandate : BaseEntity
	{
		public int RentalMandateId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
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
		public decimal? AddendumProcurement { get; set; }
		public decimal? AddendumManagement { get; set; }
		public DateTime? AddendumStartDate { get; set; }
		public DateTime? AddendumEndDate { get; set; }
		public string AddendumAmendment { get; set; }
	}
}

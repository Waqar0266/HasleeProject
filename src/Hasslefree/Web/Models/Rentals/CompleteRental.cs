using Hasslefree.Core.Domain.Common;
using System;
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
		}

		public string RentalGuid { get; set; }
		public int RentalId { get; set; }
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
		public string AccountHolder { get; set; }
		public string Bank { get; set; }
		public string Branch { get; set; }
		public string BranchCode { get; set; }
		public string AccountNumber { get; set; }
		public string BankReference { get; set; }
		public bool Procurement { get; set; }
		public bool Management { get; set; }
		public string SpecificRequirements { get; set; }
		public string SpecialConditions { get; set; }
		public string ProcureDepositOther { get; set; }
	}
}

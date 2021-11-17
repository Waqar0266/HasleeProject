using Hasslefree.Core.Domain.Common;
using System.Web.Mvc;

namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalT
    {
		public CompleteRentalT()
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

		public int RentalTId { get; set; }
        public int TenantId { get; set; }
        public string Title { get; set; }
		public Gender Gender { get; set; }
		public SelectList Titles { get; set; }
		public SelectList Genders { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
        public string MaidenName { get; set; }
        public string Email { get; set; }
		public string IdNumber { get; set; }
		public string Mobile { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		
		//Rental
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Township { get; set; }
		public string Address { get; set; }
	}
}

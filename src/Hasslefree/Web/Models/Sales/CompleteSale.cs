using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Sales;
using System.Web.Mvc;

namespace Hasslefree.Web.Models.Sales
{
    public class CompleteSale
    {
        public CompleteSale()
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

        public string SaleGuid { get; set; }
        public int SaleId { get; set; }
        public SaleType SaleType { get; set; }
        public string SaleSellerId { get; set; }
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

    }
}

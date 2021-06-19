using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;
using System;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class RegisterApproved  : IEmailDefinition
	{
		public string Title => "Registration Approved";

		public Object Example =>
			new
			{
				Email = new SharedEmailModel
				{
					Title = "Email Title",
					Subject = "Subject",
					FromName = "The Store",
					ReplyTo = "email@domain.com"
				},
				Store = new SharedStoreModel
				{
					Name = "Store name",
					Phone = "+0 000 0000 000",
					Email = "email@domain.com",
					Url = "https://store.Hasslefree7.co.za/",
					LogoUrl = null,
					CurrencyCode = "ZAR",
					CurrencySymbol = "R"
				},
				Customer = new CustomerEmailModel
				{
					Id = 234,
					Name = "Name",
					Surname = "Surname",
					Alias = "Dude",
					Email =  "email@domain.com",
					Birthday = "1979-08-30",
					Phone = "000 0000 000",
					AccountNo = "DSA3421SDF",
					AccountType = "Individual",
					AccountStatus = "Active",
 
					/* The following properties are only applicable to corporate accounts. */
					CompanyName = "Warp Development",
					CompanyNo = "2016/67/2341234",
					TaxNo = "41456789234"
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = true,
			From = "",
			Subject = "Registration Application Approved",
			Recipient = "",
			Url = "/account/register-application-approved"
		};
	}
}

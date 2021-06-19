using System;
using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class Coupon : IEmailDefinition
	{
		public string Title => "Coupon";

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
				Coupon = new CouponEmailModel()
				{
					RefNumber = "REF123456",
					To = "Name Surname",
					ImageUrl = null,
					ProductName = "Product",
					ProductDescription = "Product description goes here.",
					Hash = "AH7XKK890",
					Value = "R100.00"
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = true,
			From = "",
			Subject = "You received a Coupon!",
			Recipient = "",
			Url = "/order/coupon-email/?voucherId={0}"
		};
	}
}

using System;
using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class Voucher : IEmailDefinition
	{
		public string Title => "Gift Voucher";

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
				Voucher = new VoucherEmailModel()
				{
					RefNumber = "REF123456",
					To = null,
					From = "Name Surname",
					ImageUrl = null,
					Message = "voucher message goes here",
					Value = "R100.00"
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = true,
			From = "",
			Subject = "You received a Gift Card!",
			Recipient = "",
			Url = "/order/giftcard-email/?voucherId={0}"
		};
	}
}

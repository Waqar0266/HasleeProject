using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;
using System;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class ForgotPassword  : IEmailDefinition
	{
		public string Title => "Forgot Password";

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
				Otp = new OtpEmailModel()
				{
					Pin = 12345
				}
			};

		public InstallDefinition Install => new InstallDefinition()
		{
			Send = true,
			From = "",
			Subject = "You requested a password reset",
			Recipient = "",
			Url = "/account/forgot-email/?otp={0}"
		};
	}
}

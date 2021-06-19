using System;
using Hasslefree.Web.Models.Emails;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.Definitions.Customers
{
	public class RegisterOtp : IEmailDefinition
	{
		public string Title => "Registration OTP";

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
			Subject = "Registration - Verify Account",
			Recipient = "",
			Url = "/account/register-otp-email?otp={0}"
		};
	}
}

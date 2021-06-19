using System;

namespace Hasslefree.Web.Models.Emails
{
	public class SharedEmailModel
	{
		public String Title { get; set; }
		public String Subject { get; set; }
		public String FromName { get;set; }
		public String ReplyTo { get;set; }
	}

	public class SharedStoreModel
	{
		public String Name { get; set; }
		public String Phone { get; set; }
		public String Email { get; set; }
		public String Url { get; set; }
		public String LogoUrl { get; set; }
		public String CurrencyCode { get; set; }
		public String CurrencySymbol { get; set; }
	}
}

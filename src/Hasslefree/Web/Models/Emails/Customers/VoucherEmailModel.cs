using System;

namespace Hasslefree.Web.Models.Emails.Customers
{
	public class VoucherEmailModel: IEmailModel
	{
		public String RefNumber { get; set; }
		public String To { get; set; }
		public String From { get; set; }
		public String ImageUrl { get; set; }
		public String Message { get; set; }
		public String Value { get; set; }
	}
}

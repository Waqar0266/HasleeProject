using System;

namespace Hasslefree.Web.Models.Emails.Customers
{
	public class CouponEmailModel : IEmailModel
	{
		public String RefNumber { get; set; }
		public String To { get; set; }
		public String ImageUrl { get; set; }
		public String Value { get; set; }
		public String Hash { get; set; }
		public String ProductName { get; set; }
		public String ProductDescription { get; set; }

	}
}

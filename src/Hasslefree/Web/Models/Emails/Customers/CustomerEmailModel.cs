using System;

namespace Hasslefree.Web.Models.Emails.Customers
{
	public class CustomerEmailModel
	{
		public Int32 Id { get; set; }
		public String Name { get; set; }
		public String Surname { get; set; }
		public String Alias { get; set; }
		public String Email { get; set; }
		public String Birthday { get; set; }
		public String Phone { get; set; }
		public String AccountNo { get; set; }
		public String AccountType { get; set; }
		public String AccountStatus { get; set; }

		public String CompanyName { get; set; }
		public String CompanyNo { get; set; }
		public String TaxNo { get; set; }
	}
}

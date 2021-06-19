using System;

namespace Hasslefree.Web.Models.Emails
{
	public class EmailModel<T> where T : IEmailModel
	{
		public StoreEmailModel Store { get; set; }

		public T Model { get; set; }
	}

	public class StoreEmailModel
	{
		public String Name { get; set; }
		public String Logo {get; set; }
		public Boolean HasLogo  => !String.IsNullOrEmpty(Logo);
		public String Url { get; set; }
	}
}

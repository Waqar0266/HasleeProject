using System;

namespace Hasslefree.Web.Models.Emails
{
	public class EmailSettingsModel
	{
		public EmailSettingsModel() { }

		public Int32 Id { get; set; }
		public String Title { get; set; }
		public String From { get; set; }
		public String Subject { get; set; }
		public String Recipient { get; set; }
		public bool Send { get; set; }
		public String Type { get; set; }
	}
}

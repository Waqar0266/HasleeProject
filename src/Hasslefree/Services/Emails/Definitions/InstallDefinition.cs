using System;

namespace Hasslefree.Services.Emails.Definitions
{
	public class InstallDefinition
	{
		public Boolean Send { get; set; }
		public String From { get; set; }
		public String Subject { get; set; }
		public String Url { get; set; }
		public String Recipient { get; set; }
	}
}

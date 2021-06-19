using System;

namespace Hasslefree.Core.Infrastructure.Email.Models
{
	public class ShippingRequired
	{
		public Int32 DocumentId { get; set; }
		public String Ref { get; set; }
		public String HostName { get; set; }
		public String Hash { get; set; }
		
	}
}

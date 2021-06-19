using Hasslefree.Core.Crypto;
using System;

namespace Hasslefree.Core.Domain.Security
{
	public class Session : BaseEntity
	{
		/* CTOR */
		public Session()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			Reference = BaseX.GenerateString(16);
		}

		/* Properties */
		public int SessionId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Reference { get; set; }
		public string IpAddress { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int? LoginId { get; set; }
		public DateTime? ExpiresOn { get; set; }

		/* Navigation Properties */
		public Login Login { get; set; }
	}
}

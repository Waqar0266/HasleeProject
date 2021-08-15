using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class LandlordDocumentation : BaseEntity
	{
		public LandlordDocumentation()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int LandlordDocumentationId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalLandlordId { get; set; }
		public RentalLandlord RentalLandlord { get; set; }
		public int DownloadId { get; set; }
		public Download Download { get; set; }
	}
}

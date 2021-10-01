using System;

namespace Hasslefree.Web.Models.Rentals
{
	public class RentalFormModel
	{
		public int DownloadId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public double Size { get; set; }
		public string Path { get; set; }
		public string MimeType { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}

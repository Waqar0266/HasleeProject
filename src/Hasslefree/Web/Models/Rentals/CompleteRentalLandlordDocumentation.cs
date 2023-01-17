using System.Collections.Generic;

namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalLandlordDocumentation
	{
		public int RentalId { get; set; }
		public string LandlordGuid { get; set; }
		public string UploadIds { get; set; }
		public List<string> DocumentsToUpload { get; set; }
	}
}

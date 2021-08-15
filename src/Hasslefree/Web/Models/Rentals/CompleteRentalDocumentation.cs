using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalDocumentation
	{
		public string RentalGuid { get; set; }
		public string LandlordGuid { get; set; }
		public string UploadIds { get; set; }
		public List<string> DocumentsToUpload { get; set; }
	}
}

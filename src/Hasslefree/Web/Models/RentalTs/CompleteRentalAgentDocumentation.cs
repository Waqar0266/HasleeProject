using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalAgentDocumentation
    {
        public int RentalTId { get; set; }
        public List<string> DocumentsToUpload { get; set; }
        public string UploadIds { get; set; }
    }
}

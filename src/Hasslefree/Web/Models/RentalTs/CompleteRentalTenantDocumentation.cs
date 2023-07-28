using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalTenantDocumentation
    {
        public int RentalTId { get; set; }
        public int TenantId { get; set; }
        public string UploadIds { get; set; }
        public List<string> DocumentsToUpload { get; set; }
    }
}

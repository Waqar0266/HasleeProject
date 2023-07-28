using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
    public class TenantDocumentation : BaseEntity
    {
        public TenantDocumentation()
        {
            this.CreatedOn = DateTime.Now;
        }

        public int TenantDocumentationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int DownloadId { get; set; }
        public Download Download { get; set; }
    }
}

using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
    public class RentalTAgentDocumentation : BaseEntity
    {
        public RentalTAgentDocumentation()
        {
            this.CreatedOn = DateTime.Now;
        }

        public int RentalTAgentDocumentationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Agent Agent { get; set; }
        public int AgentId { get; set; }
        public RentalT RentalT { get; set; }
        public int RentalTId { get; set; }
        public Download Download { get; set; }
        public int DownloadId { get; set; }
    }
}

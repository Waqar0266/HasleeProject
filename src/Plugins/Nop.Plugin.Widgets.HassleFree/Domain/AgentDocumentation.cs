using System;
using Nop.Core;

namespace Nop.Plugin.Widgets.HassleFree.Domain
{
    public class AgentDocumentation : BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public int DownloadId { get; set; }
        public int AgentId { get; set; }
    }
}

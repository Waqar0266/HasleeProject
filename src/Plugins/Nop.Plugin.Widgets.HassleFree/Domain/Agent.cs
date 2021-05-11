using System;
using Nop.Core;

namespace Nop.Plugin.Widgets.HassleFree.Domain
{
    public class Agent : BaseEntity
    {
        public Agent()
        {
            this.UniqueId = Guid.NewGuid();
        }

        public int? CustomerId { get; set; }
        public Guid UniqueId { get; set; }
        public int AgentTypeId { get; set; }
        public AgentType AgentType
        {
            get => (AgentType)AgentTypeId;
            set => AgentTypeId = (int)value;
        }

        public int AgentStatusId { get; set; }
        public AgentStatus AgentStatus
        {
            get => (AgentStatus)AgentStatusId;
            set => AgentStatusId = (int)value;
        }

        public int? DirectorId { get; set; }
        public string Province { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
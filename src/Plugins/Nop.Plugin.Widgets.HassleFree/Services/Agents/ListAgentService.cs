using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.HassleFree.Domain;
using Nop.Plugin.Widgets.HassleFree.Models.Agents;

namespace Nop.Plugin.Widgets.HassleFree.Services.Agents
{
    public class ListAgentService : IListAgentService
    {
        private IRepository<Agent> _agentRepo { get; }

        public ListAgentService(IRepository<Agent> agentRepo)
        {
            _agentRepo = agentRepo;
        }

        public async Task<List<AgentViewModel>> List()
        {
            var agents = _agentRepo.Table.ToList();

            return agents.Select(a => new AgentViewModel()
            {
                Mobile = a.Mobile,
                Name = a.Name,
                Surname = a.Surname,
                IdNumber = a.IdNumber,
                Email = a.Email,
                AgentType = a.AgentType,
                Id = a.Id,
                Province = a.Province
            }).ToList();
        }
    }
}

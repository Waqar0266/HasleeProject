using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.HassleFree.Models.Agents;

namespace Nop.Plugin.Widgets.HassleFree.Services.Agents
{
    public interface IListAgentService
    {
        Task<List<AgentViewModel>> List();
    }
}

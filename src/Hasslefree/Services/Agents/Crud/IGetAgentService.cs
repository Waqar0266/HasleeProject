using Hasslefree.Web.Models.Agents;

namespace Hasslefree.Services.Agents.Crud
{
	public interface IGetAgentService
	{
		AgentWarning Warning { get; }

		AgentGet this[int agentId] { get; }
	}
}

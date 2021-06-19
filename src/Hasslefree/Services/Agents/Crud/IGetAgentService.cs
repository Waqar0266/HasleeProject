using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Web.Models.Catalog.Categories.Get;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Agents.Crud
{
	public interface IGetAgentService
	{
		AgentWarning Warning { get; }

		AgentGet this[int agentId, bool includeDates = true, bool includeProducts = false] { get; }

		QueryFutureValue<Agent> FutureValue(int agentId);
	}
}

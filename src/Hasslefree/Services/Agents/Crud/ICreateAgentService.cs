using Hasslefree.Core.Domain.Agents;
using System.Collections.Generic;

namespace Hasslefree.Services.Agents.Crud
{
	public interface ICreateAgentService
	{
		bool HasWarnings { get; }
		List<AgentWarning> Warnings { get; }
		int AgentId { get; }

		ICreateAgentService New(AgentType agentType, string title, string name, string surname, string idNumber, string email, string mobile);
		bool Create();
	}
}

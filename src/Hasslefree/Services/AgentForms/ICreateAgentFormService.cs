using Hasslefree.Core.Domain.Agents;
using System.Collections.Generic;

namespace Hasslefree.Services.AgentForms
{
	public interface ICreateAgentFormService
	{
		bool HasWarnings { get; }
		List<AgentFormWarning> Warnings { get; }
		int AgentFormId { get; }

		ICreateAgentFormService New(FormName formName, int agentId, int dowloadId);
		bool Create();
	}
}

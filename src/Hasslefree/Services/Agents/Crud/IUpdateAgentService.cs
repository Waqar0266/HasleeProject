using Hasslefree.Core.Domain.Agents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Agents.Crud
{
	public interface IUpdateAgentService
	{
		bool HasWarnings { get; }
		List<AgentWarning> Warnings { get; }

		IUpdateAgentService this[int agentId] { get; }
		IUpdateAgentService WithAgentId(int agentId);

		IUpdateAgentService Set<T>(Expression<Func<Agent, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
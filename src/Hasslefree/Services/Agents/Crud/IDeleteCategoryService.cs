using System.Collections.Generic;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface IDeleteAgentService
	{
		bool HasWarnings { get; }
		List<AgentWarning> Warnings { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <returns></returns>
		IDeleteAgentService this[int agentId] { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentIds"></param>
		/// <returns></returns>
		IDeleteAgentService this[List<int> agentIds] { get; }

		IDeleteAgentService RemoveImages(bool removeFiles = false);

		bool Remove(bool saveChanges = true);
	}
}
using Hasslefree.Core;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Agents.Crud
{
	public class UpdateAgentService : IUpdateAgentService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "AgentId", "CreatedOnUtc" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Agent> AgentRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private Agent _agent;

		#endregion

		#region Constructor

		public UpdateAgentService
		(
			IDataRepository<Agent> agentRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			AgentRepo = agentRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateAgentService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<AgentWarning> Warnings { get; } = new List<AgentWarning>();

		public IUpdateAgentService this[int agentId]
		{
			get
			{
				if (agentId <= 0)
					return this;

				_agent = AgentQuery(agentId);

				return this;
			}
		}

		public IUpdateAgentService WithAgentId(int agentId) => this[agentId];

		public IUpdateAgentService Set<T>(Expression<Func<Agent, T>> lambda, object value)
		{
			_agent?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings)
				return false;

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_agent.ModifiedOn = DateTime.Now;
				AgentRepo.Edit(_agent);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			//clear the cache
			Cache.RemoveByPattern(CacheKeys.Server.Agents.Path);

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private Agent AgentQuery(int agentId)
		{
			var cFuture = (from c in AgentRepo.Table
						   where c.AgentId == agentId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		private bool IsValid()
		{
			if (_agent == null)
			{
				Warnings.Add(new AgentWarning(AgentWarningCode.AgentNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
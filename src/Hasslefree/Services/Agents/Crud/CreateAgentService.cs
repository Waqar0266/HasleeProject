using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using static System.String;

namespace Hasslefree.Services.Agents.Crud
{
	public class CreateAgentService : ICreateAgentService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<Agent> AgentRepo { get; }

		#endregion

		#region Fields

		private Agent _agent;

		#endregion

		#region Constructor

		public CreateAgentService
		(
			IDataRepository<Agent> agentRepo
			)
		{
			// Repos
			AgentRepo = agentRepo;
		}

		#endregion

		#region ICreateAgentService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<AgentWarning> Warnings { get; } = new List<AgentWarning>();

		public int AgentId { get; private set; }

		public ICreateAgentService New(AgentType agentType, string title, string name, string surname, string idNumber, string email, string mobile)
		{
			_agent = new Agent
			{
				AgentType = agentType,
				IdNumber = idNumber,
				TempData = BuildTempData(title, name, surname, email, mobile)
			};

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				AgentRepo.Insert(_agent);

				scope.Complete();
			}

			// Set property object
			AgentId = _agent.AgentId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_agent == null)
			{
				Warnings.Add(new AgentWarning(AgentWarningCode.AgentNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		private string BuildTempData(string title, string name, string surname, string email, string mobile)
		{
			return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{title};{name};{surname};{email};{mobile}"));
		}

		#endregion
	}
}
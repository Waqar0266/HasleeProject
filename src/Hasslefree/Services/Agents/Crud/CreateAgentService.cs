using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

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

		public ICreateAgentService New(AgentType agentType, string title, string name, string surname, string email, string mobile, string idNumber)
		{
			_agent = new Agent
			{
				AgentType = agentType,
				TempData = BuildTempData(title, name, surname, email, mobile, idNumber)
			};

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			try
			{

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
			catch (Exception e)
			{
				Core.Logging.Logger.LogError(e, "Error creating the agent");
				return false;
			}
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

		private string BuildTempData(string title, string name, string surname, string email, string mobile, string idNumber)
		{
			return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{title};{name};{surname};{email};{mobile};{idNumber}"));
		}

		#endregion
	}
}
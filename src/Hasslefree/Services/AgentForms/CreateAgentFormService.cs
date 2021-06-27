using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.AgentForms
{
	public class CreateAgentFormService : ICreateAgentFormService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<AgentForm> AgentFormRepo { get; }

		#endregion

		#region Fields

		private AgentForm _agentForm;

		#endregion

		#region Constructor

		public CreateAgentFormService
		(
			IDataRepository<AgentForm> agentFormRepo
			)
		{
			// Repos
			AgentFormRepo = agentFormRepo;
		}

		#endregion

		#region ICreateAgentFormService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<AgentFormWarning> Warnings { get; } = new List<AgentFormWarning>();

		public int AgentFormId { get; private set; }

		public ICreateAgentFormService New(FormName formName, int agentId, int downloadId)
		{
			_agentForm = new AgentForm
			{
				FormName = formName,
				AgentId = agentId,
				DownloadId = downloadId
			};

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				AgentFormRepo.Insert(_agentForm);

				scope.Complete();
			}

			// Set property object
			AgentFormId = _agentForm.AgentFormId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_agentForm == null)
			{
				Warnings.Add(new AgentFormWarning(AgentFormWarningCode.AgentNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
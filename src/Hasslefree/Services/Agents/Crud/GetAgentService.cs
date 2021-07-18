using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Agents;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Agents.Crud
{
	public class GetAgentService : IGetAgentService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<AgentDocumentation> AgentDocumentationRepo { get; }
		private IReadOnlyRepository<AgentForm> AgentFormRepo { get; }

		#endregion

		#region Constructor

		public GetAgentService
		(
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<AgentDocumentation> agentDocumentationRepo,
			IReadOnlyRepository<AgentForm> agentFormRepo
		)
		{
			// Repos
			AgentRepo = agentRepo;
			PersonRepo = personRepo;
			AgentDocumentationRepo = agentDocumentationRepo;
			AgentFormRepo = agentFormRepo;
		}

		#endregion

		#region IGetAgentService

		public AgentWarning Warning { get; private set; }

		public AgentGet this[int agentId]
		{
			get
			{
				if (agentId <= 0) return AgentNotFound();

				var agent = AgentQuery(agentId);

				if (agent == null) return AgentNotFound();

				return agent;
			}
		}

		#endregion

		#region Private Methods

		private dynamic AgentNotFound()
		{
			Warning = new AgentWarning(AgentWarningCode.AgentNotFound);
			return null;
		}

		private AgentGet AgentQuery(int agentId)
		{
			var agent = (from a in AgentRepo.Table.Include(a => a.EaabProofOfPayment)
						 join person in PersonRepo.Table on a.PersonId equals person.PersonId into sr
						 from p in sr.DefaultIfEmpty()
						 where a.AgentId == agentId
						 select new AgentGet()
						 {
							 AgentId = a.AgentId,
							 AgentTypeEnum = a.AgentTypeEnum,
							 AgentStatusEnum = a.AgentStatusEnum,
							 Email = p != null ? p.Email : "",
							 IdNumber = a.IdNumber,
							 Mobile = p != null ? p.Mobile : "",
							 Name = p != null ? p.FirstName : "",
							 Surname = p != null ? p.Surname : "",
							 Title = p != null ? p.Title : "",
							 EaabProofOfPayment = a.EaabProofOfPaymentId.HasValue ? new AgentDocumentModel()
							 {
								 CreatedOn = a.EaabProofOfPayment.CreatedOn,
								 DownloadId = a.EaabProofOfPaymentId.Value,
								 Name = a.EaabProofOfPayment.FileName,
								 Path = a.EaabProofOfPayment.RelativeFolderPath,
								 Size = (a.EaabProofOfPayment.Size / 1024 / 1024)
							 } : null
						 }).FirstOrDefault();

			if (agent == null) return null;

			//get the agent documents
			var agentDocuments = AgentDocumentationRepo.Table.Where(a => a.AgentId == agentId).Select(a => new AgentDocumentModel()
			{
				DownloadId = a.DownloadId,
				Name = a.Download.FileName,
				Path = a.Download.RelativeFolderPath,
				Size = (a.Download.Size / 1024 / 1024),
				CreatedOn = a.CreatedOn
			}).ToList();

			var agentForms = AgentFormRepo.Table.Where(a => a.AgentId == agentId).Select(a => new AgentDocumentModel()
			{
				DownloadId = a.DownloadId,
				Name = a.Download.FileName,
				Path = a.Download.RelativeFolderPath,
				Size = (a.Download.Size / 1024 / 1024),
				CreatedOn = a.CreatedOn
			}).ToList();

			agent.Documents = agentDocuments;
			agent.Forms = agentForms;

			return agent;
		}

		#endregion
	}
}
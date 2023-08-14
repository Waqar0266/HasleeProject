using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Transactions;

namespace Hasslefree.Services.Tenants.Crud
{
    public class CreateTenantAgentDocumentationService : ICreateTenantAgentDocumentationService, IInstancePerRequest
    {
        #region Private Properties

        // Repos
        private IDataRepository<RentalTAgentDocumentation> Repo { get; }

        #endregion

        #region Fields

        private List<RentalTAgentDocumentation> _entities;

        #endregion

        #region Constructor

        public CreateTenantAgentDocumentationService
        (
            IDataRepository<RentalTAgentDocumentation> repo
            )
        {
            // Repos
            Repo = repo;
        }

        #endregion

        #region ICreateTenantAgentDocumentationService

        public ICreateTenantAgentDocumentationService Add(int rentalTId, int agentId, int downloadId)
        {
            if (_entities == null) _entities = new List<RentalTAgentDocumentation>();

            _entities.Add(new RentalTAgentDocumentation()
            {
                RentalTId = rentalTId,
                AgentId = agentId,
                DownloadId = downloadId
            });

            return this;
        }

        public bool Process()
        {
            // Use Transaction
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                Repo.Insert(_entities);

                scope.Complete();
            }

            return true;
        }

        #endregion
    }
}

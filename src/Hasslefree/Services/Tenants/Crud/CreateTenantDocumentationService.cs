using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Transactions;

namespace Hasslefree.Services.Tenants.Crud
{
    public class CreateTenantDocumentationService : ICreateTenantDocumentationService, IInstancePerRequest
    {
        #region Private Properties

        // Repos
        private IDataRepository<TenantDocumentation> Repo { get; }

        #endregion

        #region Fields

        private List<TenantDocumentation> _entities;

        #endregion

        #region Constructor

        public CreateTenantDocumentationService
        (
            IDataRepository<TenantDocumentation> repo
            )
        {
            // Repos
            Repo = repo;
        }

        #endregion

        #region ICreateTenantDocumentationService

        public ICreateTenantDocumentationService Add(int tenantId, int downloadId)
        {
            if (_entities == null) _entities = new List<TenantDocumentation>();

            _entities.Add(new TenantDocumentation()
            {
                TenantId = tenantId,
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

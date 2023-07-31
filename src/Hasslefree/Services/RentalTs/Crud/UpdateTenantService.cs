using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
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

namespace Hasslefree.Services.RentalTs.Crud
{
    public class UpdateTenantService : IUpdateTenantService, IInstancePerRequest
    {
        #region Constants

        private readonly string[] _restrictedProperties = { "TenantId", "CreatedOnUtc" };

        #endregion

        #region Private Properties

        // Repos
        private IDataRepository<Tenant> TenantRepo { get; }

        // Other
        private IDataContext Database { get; }
        private ICacheManager Cache { get; }

        #endregion

        #region Fields

        private Tenant _tenant;

        #endregion

        #region Constructor

        public UpdateTenantService
        (
            IDataRepository<Tenant> tenantRepo,
            IDataContext database,
            ICacheManager cache
        )
        {
            // Repos
            TenantRepo = tenantRepo;

            // Other
            Database = database;
            Cache = cache;
        }

        #endregion

        #region IUpdateTenantService

        public bool HasWarnings
        {
            get
            {
                Warnings.Clear();
                return !IsValid();
            }
        }

        public List<TenantWarning> Warnings { get; } = new List<TenantWarning>();

        public IUpdateTenantService this[int tenantId]
        {
            get
            {
                if (tenantId <= 0)
                    return this;

                _tenant = TenantQuery(tenantId);

                return this;
            }
        }

        public IUpdateTenantService Set<T>(Expression<Func<Tenant, T>> lambda, object value)
        {
            _tenant?.SetPropertyValue(lambda, value, _restrictedProperties);

            return this;
        }

        public bool Update(bool saveChanges = true)
        {
            if (HasWarnings)
                return false;

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _tenant.ModifiedOn = DateTime.Now;
                TenantRepo.Edit(_tenant);

                // Use Transaction
                if (saveChanges) Database.SaveChanges();

                scope.Complete();
            }

            Cache.RemoveByPattern(CacheKeys.Server.RentalTs.Path);

            // Success
            return true;
        }

        #endregion

        #region Private Methods

        private Tenant TenantQuery(int tenantId)
        {
            var cFuture = (from c in TenantRepo.Table
                           where c.TenantId == tenantId
                           select c).DeferredFirstOrDefault().FutureValue();

            return cFuture.Value;
        }

        private bool IsValid()
        {
            if (_tenant == null)
            {
                Warnings.Add(new TenantWarning(TenantWarningCode.TenantNotFound));
                return false;
            }

            return !Warnings.Any();
        }

        #endregion
    }
}

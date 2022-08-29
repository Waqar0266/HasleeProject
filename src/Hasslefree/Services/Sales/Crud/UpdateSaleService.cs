using Hasslefree.Core;
using Hasslefree.Core.Domain.Sales;
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

namespace Hasslefree.Services.Sales.Crud
{
    public class UpdateSaleService : IUpdateSaleService, IInstancePerRequest
    {
        #region Constants

        private readonly string[] _restrictedProperties = { "SaleId", "CreatedOnUtc" };

        #endregion

        #region Private Properties

        // Repos
        private IDataRepository<Sale> SaleRepo { get; }

        // Other
        private IDataContext Database { get; }
        private ICacheManager Cache { get; }

        #endregion

        #region Fields

        private Sale _sale;

        #endregion

        #region Constructor

        public UpdateSaleService
        (
            IDataRepository<Sale> saleRepo,
            IDataContext database,
            ICacheManager cache
        )
        {
            // Repos
            SaleRepo = saleRepo;

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

        public List<SaleWarning> Warnings { get; } = new List<SaleWarning>();

        public IUpdateSaleService this[int saleId]
        {
            get
            {
                if (saleId <= 0)
                    return this;

                _sale = SaleQuery(saleId);

                return this;
            }
        }

        public IUpdateSaleService Set<T>(Expression<Func<Sale, T>> lambda, object value)
        {
            _sale?.SetPropertyValue(lambda, value, _restrictedProperties);

            return this;
        }

        public bool Update(bool saveChanges = true)
        {
            if (HasWarnings)
                return false;

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _sale.ModifiedOn = DateTime.Now;
                SaleRepo.Edit(_sale);

                // Use Transaction
                if (saveChanges) Database.SaveChanges();

                scope.Complete();
            }

            Cache.RemoveByPattern(CacheKeys.Server.Sales.Path);

            // Success
            return true;
        }

        #endregion

        #region Private Methods

        private Sale SaleQuery(int saleId)
        {
            var cFuture = (from c in SaleRepo.Table
                           where c.SaleId == saleId
                           select c).DeferredFirstOrDefault().FutureValue();

            return cFuture.Value;
        }

        private bool IsValid()
        {
            if (_sale == null)
            {
                Warnings.Add(new SaleWarning(SaleWarningCode.SaleNotFound));
                return false;
            }

            return !Warnings.Any();
        }

        #endregion
    }
}

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
    public class UpdateSellerService: IUpdateSellerService, IInstancePerRequest
    {
        #region Constants

        private readonly string[] _restrictedProperties = { "SellerId", "CreatedOnUtc" };

        #endregion

        #region Private Properties

        // Repos
        private IDataRepository<Seller> SellerRepo { get; }

        // Other
        private IDataContext Database { get; }
        private ICacheManager Cache { get; }

        #endregion

        #region Fields

        private Seller _seller;

        #endregion

        #region Constructor

        public UpdateSellerService
        (
            IDataRepository<Seller> sellerRepo,
            IDataContext database,
            ICacheManager cache
        )
        {
            // Repos
            SellerRepo = sellerRepo;

            // Other
            Database = database;
            Cache = cache;
        }

        #endregion

        #region IUpdateSellerService

        public bool HasWarnings
        {
            get
            {
                Warnings.Clear();
                return !IsValid();
            }
        }

        public List<SellerWarning> Warnings { get; } = new List<SellerWarning>();

        public IUpdateSellerService this[int sellerId]
        {
            get
            {
                if (sellerId <= 0)
                    return this;

                _seller = SellerQuery(sellerId);

                return this;
            }
        }

        public IUpdateSellerService Set<T>(Expression<Func<Seller, T>> lambda, object value)
        {
            _seller?.SetPropertyValue(lambda, value, _restrictedProperties);

            return this;
        }

        public bool Update(bool saveChanges = true)
        {
            if (HasWarnings)
                return false;

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _seller.ModifiedOn = DateTime.Now;
                SellerRepo.Edit(_seller);

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

        private Seller SellerQuery(int sellerId)
        {
            var cFuture = (from c in SellerRepo.Table
                           where c.SellerId == sellerId
                           select c).DeferredFirstOrDefault().FutureValue();

            return cFuture.Value;
        }

        private bool IsValid()
        {
            if (_seller == null)
            {
                Warnings.Add(new SellerWarning(SellerWarningCode.SellerNotFound));
                return false;
            }

            return !Warnings.Any();
        }

        #endregion
    }
}

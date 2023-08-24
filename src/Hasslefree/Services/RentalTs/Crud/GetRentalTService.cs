using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.RentalTs.Crud
{
    public class GetRentalTService : IGetRentalTService, IInstancePerRequest
    {
        #region Private Properties

        // Repos
        private IReadOnlyRepository<RentalT> RentalTRepo { get; }

        //Services
        private IGetRentalService GetRental { get; }

        //Managers
        private ICacheManager Cache { get; }

        #endregion

        #region Fields

        private RentalT _rentalT;

        #endregion

        #region Constructor

        public GetRentalTService
        (
            //Repos
            IReadOnlyRepository<RentalT> rentalTRepo,

            //Service
            IGetRentalService getRental,

            //Managers
            ICacheManager cache
        )
        {
            // Repos
            RentalTRepo = rentalTRepo;

            //Services
            GetRental = getRental;

            //Managers
            Cache = cache;
        }

        #endregion

        #region IGetRentalService

        public IGetRentalTService this[int rentalTId]
        {
            get
            {
                if (rentalTId <= 0)
                    return this;

                _rentalT = RentalTQuery(rentalTId);

                return this;
            }
        }

        public IGetRentalTService this[string rentalTGuid]
        {
            get
            {
                if (String.IsNullOrEmpty(rentalTGuid))
                    return this;

                _rentalT = RentalTQuery(rentalTGuid);

                return this;
            }
        }

        public RentalTGet Get()
        {
            if (_rentalT == null) return null;

            return new RentalTGet
            {
                RentalTGuid = _rentalT.UniqueId,
                RentalTId = _rentalT.RentalTId,
                Tenants = _rentalT.Tenants.ToList(),
                Rental = GetRental[_rentalT.RentalId].Get(),
                Status = _rentalT.RentalTStatus,
                DeclineReason = _rentalT.DeclineReason
            };
        }

        #endregion

        #region Private Methods

        private RentalT RentalTQuery(int rentalTId)
        {
            return Cache.Get(CacheKeys.Server.RentalTs.RentalTById(rentalTId), CacheKeys.Time.LongTime, () =>
            {
                var cFuture = (from c in RentalTRepo.Table.Include("Tenants.Person").Include("Rental.Agent.Person")
                               where c.RentalTId == rentalTId
                               select c).DeferredFirstOrDefault().FutureValue();

                return cFuture.Value;

            });
        }

        private RentalT RentalTQuery(string rentalTGuid)
        {
            return Cache.Get(CacheKeys.Server.RentalTs.RentalTByGuid(rentalTGuid), CacheKeys.Time.LongTime, () =>
            {
                var cFuture = (from c in RentalTRepo.Table.Include(x => x.Tenants)
                               where c.UniqueId.ToString().ToLower() == rentalTGuid.ToLower()
                               select c).DeferredFirstOrDefault().FutureValue();

                return cFuture.Value;

            });
        }

        #endregion
    }
}

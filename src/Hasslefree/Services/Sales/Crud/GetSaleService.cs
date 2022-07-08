using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Sales;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Sales;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Sales.Crud
{
    public class GetSaleService : IGetSaleService, IInstancePerRequest
    {
        #region Private Properties

        // Repos
        private IReadOnlyRepository<Sale> SaleRepo { get; }
        private IReadOnlyRepository<Seller> SellerRepo { get; }
        private IReadOnlyRepository<SaleWitness> SaleWitnessRepo { get; }
        private IReadOnlyRepository<Agent> AgentRepo { get; }
        private IReadOnlyRepository<Person> PersonRepo { get; }
        private IReadOnlyRepository<Address> AddressRepo { get; }
        private IReadOnlyRepository<AgentAddress> AgentAddressRepo { get; }

        //Managers
        private ICacheManager Cache { get; }

        #endregion

        #region Fields

        private Sale _sale;

        #endregion

        #region Constructor

        public GetSaleService
        (
            //Repos
            IReadOnlyRepository<Sale> saleRepo,
            IReadOnlyRepository<Seller> sellerRepo,
            IReadOnlyRepository<SaleWitness> saleWitnessRepo,
            IReadOnlyRepository<Agent> agentRepo,
            IReadOnlyRepository<Person> personRepo,
            IReadOnlyRepository<Address> addressRepo,
            IReadOnlyRepository<AgentAddress> agentAddressRepo,

            //Managers
            ICacheManager cache
        )
        {
            // Repos
            SaleRepo = saleRepo;
            SellerRepo = sellerRepo;
            SaleWitnessRepo = saleWitnessRepo;
            AgentRepo = agentRepo;
            PersonRepo = personRepo;
            AddressRepo = addressRepo;
            AgentAddressRepo = agentAddressRepo;

            //Managers
            Cache = cache;
        }

        #endregion

        #region IGetRentalService

        public IGetSaleService this[int saleId]
        {
            get
            {
                if (saleId <= 0)
                    return this;

                _sale = SaleQuery(saleId);

                return this;
            }
        }

        public IGetSaleService this[string saleGuid]
        {
            get
            {
                if (String.IsNullOrEmpty(saleGuid))
                    return this;

                _sale = SaleQuery(saleGuid);

                return this;
            }
        }

        public SaleGet Get()
        {
            if (_sale == null) return null;

            var sellers = Cache.Get(CacheKeys.Server.Sales.GetSellers(_sale.SaleId), CacheKeys.Time.LongTime, () => SellerRepo.Table.Include(a => a.Person).Include(a => a.Initials).Include(a => a.Signature).Where(a => a.SaleId == _sale.SaleId).ToList());
            var agent = Cache.Get(CacheKeys.Server.Sales.GetAgent(_sale.SaleId), CacheKeys.Time.LongTime, () => AgentRepo.Table.Include(a => a.Signature).Include(a => a.Initials).Include(x => x.Person).FirstOrDefault(a => a.AgentId == _sale.AgentId));
            var saleWitness = Cache.Get(CacheKeys.Server.Sales.GetWitness(_sale.SaleId), CacheKeys.Time.LongTime, () =>
            SaleWitnessRepo
            .Table
            .Include(a => a.AgentWitness1Signature)
            .Include(a => a.AgentWitness1Initials)
            .Include(a => a.AgentWitness2Signature)
            .Include(a => a.AgentWitness2Initials)
            .Include(a => a.SellerWitness1Signature)
            .Include(a => a.SellerWitness1Initials)
            .Include(a => a.SellerWitness2Signature)
            .Include(a => a.SellerWitness2Initials)
            .FirstOrDefault(a => a.SaleId == _sale.SaleId));

            var agentPerson = Cache.Get(CacheKeys.Server.Sales.GetAgentPerson(agent.PersonId ?? 0), CacheKeys.Time.LongTime, () => PersonRepo.Table.FirstOrDefault(a => a.PersonId == (agent.PersonId ?? 0)));
            var agentAddresses = Cache.Get(CacheKeys.Server.Sales.GetAgentAddresses(agent.AgentId), CacheKeys.Time.LongTime, () => AgentAddressRepo.Table.Include(a => a.Address).Where(a => a.AgentId == agent.AgentId).ToList());
            var physical = AddressType.Residential.ToString();
            var postal = AddressType.Residential.ToString();
            var agentPhysicalAddressId = agentAddresses?.FirstOrDefault(a => a.Address.TypeEnum == physical) ?? null;
            var agentPostalAddressId = agentAddresses?.FirstOrDefault(a => a.Address.TypeEnum == postal) ?? null;
            var agentPhysicalAddress = Cache.Get(CacheKeys.Server.Sales.GetAgentAddress(agentPhysicalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
            {
                if (agentPhysicalAddressId == null) return null;
                return AddressRepo.Table.FirstOrDefault(a => a.AddressId == agentPhysicalAddressId.AddressId);
            });
            var agentPostalAddress = Cache.Get(CacheKeys.Server.Sales.GetAgentAddress(agentPostalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
            {
                if (agentPostalAddressId == null) return null;
                return AddressRepo.Table.FirstOrDefault(a => a.AddressId == agentPostalAddressId.AddressId);
            });

            return new SaleGet
            {
                SaleId = _sale.SaleId,
                UniqueId = _sale.UniqueId,
                Sellers = sellers,
                Address = _sale.Address,
                StandErf = _sale.StandErf,
                Township = _sale.Township,
                SaleType = _sale.SaleType,
                Agent = agent,
                SpecialConditions = _sale.SpecialConditions
            };
        }

        #endregion

        #region Private Methods

        private Sale SaleQuery(int saleId)
        {
            return Cache.Get(CacheKeys.Server.Sales.SaleById(saleId), CacheKeys.Time.LongTime, () =>
            {
                var cFuture = (from c in SaleRepo.Table
                               where c.SaleId == saleId
                               select c).DeferredFirstOrDefault().FutureValue();

                return cFuture.Value;

            });
        }

        private Sale SaleQuery(string saleGuid)
        {
            return Cache.Get(CacheKeys.Server.Sales.SaleByGuid(saleGuid), CacheKeys.Time.LongTime, () =>
            {
                var cFuture = (from c in SaleRepo.Table
                               where c.UniqueId.ToString().ToLower() == saleGuid.ToLower()
                               select c).DeferredFirstOrDefault().FutureValue();

                return cFuture.Value;

            });
        }

        #endregion
    }
}

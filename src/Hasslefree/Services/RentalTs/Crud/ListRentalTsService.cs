using EntityFramework.Extensions;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static System.String;

namespace Hasslefree.Services.RentalTs.Crud
{
    public class ListRentalTsService : IListRentalTsService, IInstancePerRequest
    {
        #region Private Properties

        private IReadOnlyRepository<RentalT> RentalTRepo { get; }
        private IReadOnlyRepository<Rental> RentalRepo { get; }

        #endregion

        #region Fields

        private DateTime? _createdAfter;
        private DateTime? _createdBefore;

        private string _search;

        private int _page;
        private int? _pageSize;
        private int _totalRecords;

        private IQueryable<RentalT> _rentalTs;

        #endregion

        #region Constructor

        public ListRentalTsService
        (
            IReadOnlyRepository<RentalT> rentalTRepo,
            IReadOnlyRepository<Rental> rentalRepo
        )
        {
            RentalTRepo = rentalTRepo;
            RentalRepo = rentalRepo;
        }

        #endregion

        #region IListAgentsService

        public IListRentalTsService CreatedBefore(DateTime? createdBefore)
        {
            _createdBefore = createdBefore;
            return this;
        }

        public IListRentalTsService CreatedAfter(DateTime? createdAfter)
        {
            _createdAfter = createdAfter;
            return this;
        }

        public IListRentalTsService WithSearch(string search)
        {
            _search = search;
            return this;
        }

        public IListRentalTsService WithPaging(int page = 0, int pageSize = 50)
        {
            _page = page;
            _pageSize = pageSize;

            return this;
        }

        public RentalTList List()
        {
            _rentalTs = RentalTQuery();

            FilterCreatedBefore();
            FilterCreatedAfter();
            FilterSearch();

            GetTotalRecords();
            GetPaging();

            var rentalIds = _rentalTs.Select(r => r.RentalId);

            var rentalAddresses = RentalRepo.Table.Where(r=> rentalIds.Contains(r.RentalId)).ToDictionary(r => r.RentalId, r => r.Address);

            var list = _rentalTs.Select(r => new RentalTListItem()
            {
                Id = r.RentalTId,
                Address = rentalAddresses[r.RentalId],
                ModifiedOn = r.ModifiedOn,
                Status = r.RentalTStatus.ResolveStatus(),
                StatusDescription = r.RentalTStatus.ResolveStatusDescription()
            }).ToList();

            return new RentalTList
            {
                Page = _page,
                PageSize = _pageSize ?? _totalRecords,
                TotalRecords = _totalRecords,
                Items = list
            };
        }

        #endregion

        #region Private Methods

        private IQueryable<RentalT> RentalTQuery()
        {
            var cFuture = (from c in RentalTRepo.Table.Include(r => r.Rental) select c).Future();
            return cFuture.AsQueryable();
        }

        private void FilterSearch()
        {
            if (IsNullOrWhiteSpace(_search)) return;

            string searchQuery = _search.ToLower().Trim();

            _rentalTs = _rentalTs.Where(c => SearchHelper(c.StreetName).Contains(searchQuery));
        }

        private string SearchHelper(string property) => property?.Replace("/", "").ToLower();

        private void FilterCreatedAfter()
        {
            if (!_createdAfter.HasValue) return;

            _rentalTs = _rentalTs.Where(a => a.CreatedOn >= _createdAfter.Value);
        }

        private void FilterCreatedBefore()
        {
            if (!_createdBefore.HasValue) return;

            _rentalTs = _rentalTs.Where(a => a.CreatedOn < _createdBefore.Value);
        }

        private void GetTotalRecords()
        {
            _totalRecords = _rentalTs.Select(c => c.RentalTId).Count();
        }

        private void GetPaging()
        {
            if (!_pageSize.HasValue) _pageSize = _totalRecords;

            _rentalTs = _rentalTs.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
        }

        #endregion
    }

    public static class RentalTExtensions
    {
        public static string ResolveStatus(this RentalTStatus s)
        {
            string status = "N/A";

            switch (s)
            {
                case RentalTStatus.PendingNew:
                    status = "Pending Tenant(s) Registration";
                    break;

                case RentalTStatus.PendingTenantDocumentation:
                    status = "Pending Tenant(s) Documentation";
                    break;

                case RentalTStatus.PendingTenantSignature:
                    status = "Pending Tenant(s) Signature";
                    break;

                default:
                    status = "N/A";
                    break;
            }

            return status;
        }

        public static string ResolveStatusDescription(this RentalTStatus s)
        {
            string status = "N/A";

            switch (s)
            {
                case RentalTStatus.PendingNew:
                    status = "Waiting for the Tenant(s) to complete their fields";
                    break;

                case RentalTStatus.PendingTenantDocumentation:
                    status = "Waiting for the Tenant(s) to upload their documentation";
                    break;

                case RentalTStatus.PendingTenantSignature:
                    status = "Waiting for the Tenant(s) to submit their signature";
                    break;

                default:
                    status = "N/A";
                    break;
            }

            return status;
        }
    }
}

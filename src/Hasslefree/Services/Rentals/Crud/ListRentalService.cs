using EntityFramework.Extensions;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Linq;
using static System.String;

namespace Hasslefree.Services.Rentals.Crud
{
    public class ListRentalService : IListRentalService, IInstancePerRequest
    {
        #region Private Properties

        private IReadOnlyRepository<Rental> RentalRepo { get; }

        #endregion

        #region Fields

        private DateTime? _createdAfter;
        private DateTime? _createdBefore;

        private string _search;

        private int _page;
        private int? _pageSize;
        private int _totalRecords;

        private IQueryable<Rental> _rentals;

        #endregion

        #region Constructor

        public ListRentalService
        (
            IReadOnlyRepository<Rental> rentalRepo
        )
        {
            RentalRepo = rentalRepo;
        }

        #endregion

        #region IListAgentsService

        public IListRentalService CreatedBefore(DateTime? createdBefore)
        {
            _createdBefore = createdBefore;
            return this;
        }

        public IListRentalService CreatedAfter(DateTime? createdAfter)
        {
            _createdAfter = createdAfter;
            return this;
        }

        public IListRentalService WithSearch(string search)
        {
            _search = search;
            return this;
        }

        public IListRentalService WithPaging(int page = 0, int pageSize = 50)
        {
            _page = page;
            _pageSize = pageSize;

            return this;
        }

        public RentalList List()
        {
            _rentals = RentalQuery();

            FilterCreatedBefore();
            FilterCreatedAfter();
            FilterSearch();

            GetTotalRecords();
            GetPaging();

            return new RentalList
            {
                Page = _page,
                PageSize = _pageSize ?? _totalRecords,
                TotalRecords = _totalRecords,
                Items = _rentals.AsEnumerable().Select(c => new RentalListItem
                {
                    RentalId = c.RentalId,
                    Type = c.RentalTypeEnum
                }).ToList()
            };
        }

        #endregion

        #region Private Methods

        private IQueryable<Rental> RentalQuery()
        {
            var cFuture = (from c in RentalRepo.Table select c).Future();
            return cFuture.AsQueryable();
        }

        private void FilterSearch()
        {
            if (IsNullOrWhiteSpace(_search)) return;

            string searchQuery = _search.ToLower().Trim();

            _rentals = _rentals.Where(c => AgentSearchHelper(c.Premises).Contains(searchQuery));
        }

        private string AgentSearchHelper(string property) => property?.Replace("/", "").ToLower();

        private void FilterCreatedAfter()
        {
            if (!_createdAfter.HasValue) return;

            _rentals = _rentals.Where(a => a.CreatedOn >= _createdAfter.Value);
        }

        private void FilterCreatedBefore()
        {
            if (!_createdBefore.HasValue) return;

            _rentals = _rentals.Where(a => a.CreatedOn < _createdBefore.Value);
        }

        private void GetTotalRecords()
        {
            _totalRecords = _rentals.Select(c => c.AgentId).Count();
        }

        private void GetPaging()
        {
            if (!_pageSize.HasValue) _pageSize = _totalRecords;

            _rentals = _rentals.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
        }

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}

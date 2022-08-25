using EntityFramework.Extensions;
using Hasslefree.Core.Domain.Sales;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.String;

namespace Hasslefree.Services.Sales.Crud
{
    public class ListSalesService : IListSalesService, IInstancePerRequest
    {
        #region Private Properties

        private IReadOnlyRepository<Sale> SaleRepo { get; }

        #endregion

        #region Fields

        private DateTime? _createdAfter;
        private DateTime? _createdBefore;

        private string _search;

        private int _page;
        private int? _pageSize;
        private int _totalRecords;

        private IQueryable<Sale> _sales;

        #endregion

        #region Constructor

        public ListSalesService
        (
            IReadOnlyRepository<Sale> saleRepo
        )
        {
            SaleRepo = saleRepo;
        }

        #endregion

        #region IListAgentsService

        public IListSalesService CreatedBefore(DateTime? createdBefore)
        {
            _createdBefore = createdBefore;
            return this;
        }

        public IListSalesService CreatedAfter(DateTime? createdAfter)
        {
            _createdAfter = createdAfter;
            return this;
        }

        public IListSalesService WithSearch(string search)
        {
            _search = search;
            return this;
        }

        public IListSalesService WithPaging(int page = 0, int pageSize = 50)
        {
            _page = page;
            _pageSize = pageSize;

            return this;
        }

        public SaleList List()
        {
            _sales = SaleQuery();

            FilterCreatedBefore();
            FilterCreatedAfter();
            FilterSearch();

            GetTotalRecords();
            GetPaging();

            var list = new List<SaleListItem>();
            if (_sales.Any()) list.AddRange(_sales.AsEnumerable().Select(c => new SaleListItem
            {
                Id = c.SaleId,
                Status = c.SaleStatus.ResolveStatus(),
                StatusDescription = c.SaleStatus.ResolveStatus(),
                ModifiedOn = c.ModifiedOn,
                Type = c.SaleType.ResolveType(),
                Address = c.Address ?? "N/A"
            }));

            return new SaleList
            {
                Page = _page,
                PageSize = _pageSize ?? _totalRecords,
                TotalRecords = _totalRecords,
                Items = list
            };
        }

        #endregion

        #region Private Methods

        private IQueryable<Sale> SaleQuery()
        {
            var cFuture = (from c in SaleRepo.Table select c).Future();
            return cFuture.AsQueryable();
        }

        private void FilterSearch()
        {
            if (IsNullOrWhiteSpace(_search)) return;

            string searchQuery = _search.ToLower().Trim();

            _sales = _sales.Where(c => SearchHelper(c.Address).Contains(searchQuery));
        }

        private string SearchHelper(string property) => property?.Replace("/", "").ToLower();

        private void FilterCreatedAfter()
        {
            if (!_createdAfter.HasValue) return;

            _sales = _sales.Where(a => a.CreatedOn >= _createdAfter.Value);
        }

        private void FilterCreatedBefore()
        {
            if (!_createdBefore.HasValue) return;

            _sales = _sales.Where(a => a.CreatedOn < _createdBefore.Value);
        }

        private void GetTotalRecords()
        {
            _totalRecords = _sales.Select(c => c.SaleId).Count();
        }

        private void GetPaging()
        {
            if (!_pageSize.HasValue) _pageSize = _totalRecords;

            _sales = _sales.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
        }

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }

    public static class SaleExtensions
    {
        public static string ResolveStatus(this SaleStatus s)
        {
            string status = "N/A";

            switch (s)
            {
                case SaleStatus.PendingNew:
                    status = "Pending (New)";
                    break;
                case SaleStatus.PendingSellerDocumentation:
                    status = "Pending Seller(s) Documentation Upload";
                    break;
                case SaleStatus.PendingSellerRegistration:
                    status = "Pending Seller(s) Registration";
                    break;
                case SaleStatus.PendingSellerSignature:
                    status = "Pending Seller(s) Signature";
                    break;
                case SaleStatus.PendingAgentDocumentation:
                    status = "Pending Agent Documentation Upload";
                    break;
                case SaleStatus.PendingAgentRegistration:
                    status = "Pending Agent Registration";
                    break;
                case SaleStatus.PendingAgentSignature:
                    status = "Pending Agent Signature";
                    break;
                case SaleStatus.PendingAgentWitnessSignature:
                    status = "Pending Agent Witness Signatures";
                    break;
                case SaleStatus.PendingSellerWitnessSignature:
                    status = "Pending Seller(s) Witness Signatures";
                    break;
                case SaleStatus.PendingProperty24:
                    status = "Pending Property 24 link";
                    break;
                case SaleStatus.PendingMemberSignatures:
                    status = "Pending Member(s) Signatures";
                    break;
                case SaleStatus.Completed:
                    status = "Completed";
                    break;

                default:
                    status = "N/A";
                    break;
            }

            return status;
        }

        public static string ResolveStatusDescription(this SaleStatus s)
        {
            string status = "N/A";

            switch (s)
            {
                case SaleStatus.PendingNew:
                    status = "Waiting for the Seller(s) to complete their registration";
                    break;
                case SaleStatus.PendingSellerDocumentation:
                    status = "Waiting for Seller(s) to upload their documentation";
                    break;
                case SaleStatus.PendingSellerRegistration:
                    status = "Waiting for the Seller(s) to complete their registration";
                    break;
                case SaleStatus.PendingSellerSignature:
                    status = "Waiting for the Seller(s) to complete their signatures";
                    break;
                case SaleStatus.PendingAgentDocumentation:
                    status = "Waiting for the Agent to upload their documentation";
                    break;
                case SaleStatus.PendingAgentRegistration:
                    status = "Waiting for the Agent to complete their registration";
                    break;
                case SaleStatus.PendingAgentSignature:
                    status = "Waiting for the Agent to complete their signature";
                    break;
                case SaleStatus.PendingAgentWitnessSignature:
                    status = "Waiting for the Agent Witnesses to complete their signatures";
                    break;
                case SaleStatus.PendingSellerWitnessSignature:
                    status = "Waiting for the Seller(s) Witnesses to complete their signatures";
                    break;
                case SaleStatus.PendingProperty24:
                    status = "Waiting for the Agent to link the listing with Property 24";
                    break;
                case SaleStatus.PendingMemberSignatures:
                    status = "Waiting for the Member(s) to complete their signatures";
                    break;
                case SaleStatus.Completed:
                    status = "Completed and active";
                    break;

                default:
                    status = "N/A";
                    break;
            }

            return status;
        }

        public static string ResolveType(this SaleType t)
        {
            string status = "N/A";

            switch (t)
            {
                case SaleType.MultiListing:
                    status = "Multi Listing";
                    break;
                case SaleType.OpenMandate:
                    status = "Open Mandate";
                    break;
                case SaleType.SoleMandate:
                    status = "Sole Mandate";
                    break;

                default:
                    status = "N/A";
                    break;
            }

            return status;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Web.Models.Addresses;
using Hasslefree.Web.Models.Common.Address;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Common.Addresses
{
	public class ListAddressesService : IListAddressesService
	{
		#region Constants

		private const Int32 DefaultPageSize = 50;
		private const AddressSortBy DefaultSort = AddressSortBy.LabelAsc;

		#endregion

		#region Private Properties

		private IReadOnlyRepository<Address> AddressRepo { get; }

		#endregion

		#region Fields

		private AddressSortBy _sortBy = DefaultSort;
		private String _search;

		private Int32 _page;
		private Int32 _pageSize = DefaultPageSize;

		private IQueryable<AddressViewModel> _query;

		#endregion

		#region Constructor

		public ListAddressesService(
				IReadOnlyRepository<Address> addressRepo
			)
		{
			AddressRepo = addressRepo;
		}

		#endregion

		#region IListAddressesService

		/// <summary>
		/// Set a column to sort by
		/// </summary>
		/// <param name="sortBy"></param>
		/// <returns></returns>
		public IListAddressesService SortBy(AddressSortBy sortBy)
		{
			_sortBy = sortBy;

			return this;
		}

		/// <summary>
		/// Set a string to search the name on
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		public IListAddressesService Search(String search)
		{
			_search = search;

			return this;
		}

		/// <summary>
		/// Set the current page and page size
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public IListAddressesService SetPaging(Int32 page, Int32 pageSize)
		{
			_page = page - 1;
			_pageSize = pageSize;

			return this;
		}

		/// <summary>
		/// Get the list of addresses
		/// </summary>
		/// <returns></returns>
		public AddressListModel List()
		{
			Query();

			if (_query == null)
				return null;

			Sort();

			Search();

			var list = Get();

			var model = new AddressListModel()
			{
				SortBy = _sortBy,
				Search = _search,
				Page = _page,
				PageSize = _pageSize,
				List = list.Items,
				TotalItems = list.TotalItems
			};

			Clear();
			return model;
		}

		#endregion

		#region Private Methods

		private void Clear()
		{
			_sortBy = DefaultSort;
			_search = "";

			_page = 0;
			_pageSize = DefaultPageSize;
			_query = null;
		}

		private void Query()
		{
			_query = (from p in AddressRepo.Table
					  select new AddressViewModel()
					  {
						  AddressId = p.AddressId,
						  Address1 = p.Address1,
						  Address2 = p.Address2,
						  Address3 = p.Address3,
						  RegionName = p.RegionName,
						  City = p.Town,
						  Code = p.Code,
						  Country = p.Country,
						  Type = p.Type
					  });
		}

		private void Sort()
		{
			switch (_sortBy)
			{
				case AddressSortBy.CountryAsc:
					_query = _query.OrderBy(a => a.Country);
					break;
				case AddressSortBy.CountryDesc:
					_query = _query.OrderByDescending(a => a.Country);
					break;
			}
		}

		private void Search()
		{
			if (IsNullOrWhiteSpace(_search))
				return;

			_query = _query.Where(q =>
				q.Address1.Contains(_search) ||
				q.Address2.Contains(_search) ||
				q.Address3.Contains(_search) ||
				q.RegionName.Contains(_search) ||
				q.City.Contains(_search) ||
				q.Country.Contains(_search));
		}

		private (List<AddressViewModel> Items, Int32 TotalItems) Get()
		{
			// Get total items
			var totalItemsFut = _query.Select(a => a.AddressId).DeferredCount();

			// Get all items
			if (_page < 0)
				_page = 0;
			if (_page == 0 && _pageSize == 0)
				_pageSize = Int32.MaxValue;

			// Get the page list
			var list = _query.Skip(_page * _pageSize).Take(_pageSize).ToList();

			return (list, totalItemsFut.Execute());
		}

		#endregion
	}
}

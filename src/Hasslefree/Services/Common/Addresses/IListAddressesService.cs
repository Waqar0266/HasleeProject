using System;
using Hasslefree.Web.Models.Common.Address;

namespace Hasslefree.Services.Common.Addresses
{
	public interface IListAddressesService
	{
		/// <summary>
		/// Set a column to sort by
		/// </summary>
		/// <param name="sortBy"></param>
		/// <returns></returns>
		IListAddressesService SortBy(AddressSortBy sortBy);

		/// <summary>
		/// Set a string to search the name on
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IListAddressesService Search(String search);

		/// <summary>
		/// Set the current page and page size
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		IListAddressesService SetPaging(Int32 page, Int32 pageSize);


		/// <summary>
		/// Get the list of addresses
		/// </summary>
		/// <returns></returns>
		AddressListModel List();
	}
}

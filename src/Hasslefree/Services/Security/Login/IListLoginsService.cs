using Hasslefree.Web.Models.Security.Login.List;
using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Security.Login
{
	public interface IListLoginsService
	{
		/// <summary>
		/// Filter records for records created after the parameter
		/// </summary>
		/// <param name="createdAfter">Created after parameter</param>
		/// <returns></returns>
		IListLoginsService CreatedAfter(DateTime? createdAfter);

		/// <summary>
		/// Filter records for records created before the parameter
		/// </summary>
		/// <param name="createdBefore">Created before parameter</param>
		/// <returns></returns>
		IListLoginsService CreatedBefore(DateTime? createdBefore);

		/// <summary>
		/// Search for records that match the parameter
		/// </summary>
		/// <param name="search">Search term parameter</param>
		/// <returns>Service instance</returns>
		IListLoginsService WithSearch(string search);

		/// <summary>
		/// Filter the records that match the parameter
		/// </summary>
		/// <param name="filterBy">Filter by parameter</param>
		/// <returns>Service instance</returns>
		IListLoginsService FilterBy(string filterBy);

		/// <summary>
		/// Sort the records by the parameter
		/// </summary>
		/// <param name="sortBy">Sort by parameter</param>
		/// <returns>Service instance</returns>
		IListLoginsService SortBy(string sortBy);

		/// <summary>
		/// Filter records to those that match the parameter
		/// </summary>
		/// <param name="ids">Security Group identifiers parameter</param>
		/// <returns>Service instance</returns>
		IListLoginsService WithSecurityGroupIds(List<int> ids);

		/// <summary>
		/// Set pagination sizes for the returned records
		/// </summary>
		/// <param name="page">Set which subset to return</param>
		/// <param name="pageSize">Set number of records returned</param>
		/// <returns>Service instance</returns>
		IListLoginsService WithPaging(int page = 0, int pageSize = 50);

		/// <summary>
		/// Execute the retrieval of the records that match the filter(s)
		/// </summary>
		/// <param name="includeDates">Include DateTime properties in return</param>
		/// <returns>Model list populated with the base entity properties</returns>
		LoginList List(bool includeDates = true);
	}
}

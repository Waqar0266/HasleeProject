using System;
using System.Collections.Generic;
using Hasslefree.Web.Models.Security.SecurityGroups.List;

namespace Hasslefree.Services.Security.Groups
{
	public interface IListSecurityGroupsService
	{
		/// <summary>
		/// Filter records for records created after the parameter
		/// </summary>
		/// <param name="createdAfter">Created after parameter</param>
		/// <returns></returns>
		IListSecurityGroupsService CreatedAfter(DateTime? createdAfter);

		/// <summary>
		/// Filter records for records created before the parameter
		/// </summary>
		/// <param name="createdBefore">Created before parameter</param>
		/// <returns></returns>
		IListSecurityGroupsService CreatedBefore(DateTime? createdBefore);

		/// <summary>
		/// Search for Discounts records that match the parameter
		/// </summary>
		/// <param name="search">Search term parameter</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService WithSearch(string search);

		/// <summary>
		/// Sort the records by the parameter
		/// </summary>
		/// <param name="sortBy">Sort by parameter</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService SortBy(string sortBy);

		/// <summary>
		/// Filter the records by the parameter
		/// </summary>
		/// <param name="filterBy">Filter by parameter</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService FilterBy(string filterBy);

		/// <summary>
		/// Set pagination sizes for the returned records
		/// </summary>
		/// <param name="page">Set which subset to return</param>
		/// <param name="pageSize">Set number of records returned</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService WithPaging(int page = 0, int pageSize = 50);

		/// <summary>
		/// Filter records to those that match the parameter
		/// </summary>
		/// <param name="ids">Permission identifiers parameter</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService WithPermissionIds(List<int> ids);

		/// <summary>
		/// Filter records to those that match the parameter
		/// </summary>
		/// <param name="ids">Login identifiers parameter</param>
		/// <returns>Service instance</returns>
		IListSecurityGroupsService WithLoginIds(List<int> ids);

		/// <summary>
		/// Execute the retrieval of the records that match the filter(s)
		/// </summary>
		/// <param name="includeDates">Include DateTime properties in return</param>
		/// <returns>Model list populated with the base entity properties</returns>
		SecurityGroupList List(bool includeDates = true);

	}
}

using Hasslefree.Web.Models.Security.Permissions.List;
using System.Collections.Generic;

namespace Hasslefree.Services.Security.Permissions
{
	public interface IListPermissionsService
	{
		/// <summary>
		/// Search for records that match the parameter
		/// </summary>
		/// <param name="search">Search term parameter</param>
		/// <returns>Service instance</returns>
		IListPermissionsService WithSearch(string search);

		/// <summary>
		/// Filter records to those that match the parameter
		/// </summary>
		/// <param name="ids">Security Group identifiers parameter</param>
		/// <returns></returns>
		IListPermissionsService WithSecurityGroupIds(List<int> ids);

		/// <summary>
		/// Filter records to those that match the parameter
		/// </summary>
		/// <param name="ids">Login identifiers parameter</param>
		/// <returns></returns>
		IListPermissionsService WithLoginIds(List<int> ids);

		/// <summary>
		/// Sort the records by the parameter
		/// </summary>
		/// <param name="sortBy">Sort by parameter</param>
		/// <returns>Service instance</returns>
		IListPermissionsService SortBy(string sortBy);

		/// <summary>
		/// Set pagination sizes for the returned records
		/// </summary>
		/// <param name="page">Set which subset to return</param>
		/// <param name="pageSize">Set number of records returned</param>
		/// <returns>Service instance</returns>
		IListPermissionsService WithPaging(int page, int pageSize);

		/// <summary>
		/// Execute the retrieval of the records that match the filter(s)
		/// </summary>
		/// <returns>Model list populated with the base entity properties</returns>
		PermissionList List(bool includeDates = true, bool includeUniqueNames = true);
	}
}
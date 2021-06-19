using Hasslefree.Web.Models.People.List;
using System;
using System.Collections.Generic;

namespace Hasslefree.Services.People.Interfaces
{
	public interface IListPersonsService
	{
		/// <summary>
		/// Sets the service instance to include the DateTime properties 
		/// </summary>
		/// <returns>Service instance</returns>
		IListPersonsService IncludeDates();

		/// <summary>
		/// Filter Person records for records created after the parameter
		/// </summary>
		/// <param name="createdAfter">Created after parameter</param>
		/// <returns></returns>
		IListPersonsService CreatedAfter(DateTime? createdAfter);

		/// <summary>
		/// Filter Person records for records created before the parameter
		/// </summary>
		/// <param name="createdBefore">Created before parameter</param>
		/// <returns></returns>
		IListPersonsService CreatedBefore(DateTime? createdBefore);

		/// <summary>
		/// Search for Person records that match the parameter
		/// </summary>
		/// <param name="search">Search term parameter</param>
		/// <returns>Service instance</returns>
		IListPersonsService WithSearch(string search);

		/// <summary>
		/// Filter the Person records that match the parameter
		/// </summary>
		/// <param name="filterBy">Filter by parameter</param>
		/// <returns>Service instance</returns>
		IListPersonsService FilterBy(string filterBy);
		
		/// <summary>
		/// Sort the Person records by the parameter
		/// </summary>
		/// <param name="sortBy">Sort by parameter</param>
		/// <returns>Service instance</returns>
		IListPersonsService SortBy(string sortBy);

		/// <summary>
		/// Filter the Person records to those who have account memberships that match the parameter
		/// </summary>
		/// <param name="accountIds">Account identifiers parameter</param>
		/// <returns>Service instance</returns>
		IListPersonsService WithAccountIds(List<int> accountIds);

		/// <summary>
		/// Set pagination sizes for the returned Person records
		/// </summary>
		/// <param name="page">Set which subset to return</param>
		/// <param name="pageSize">Set number of records returned</param>
		/// <returns>Service instance</returns>
		IListPersonsService WithPaging(int page = 0, int pageSize = 50);

		/// <summary>
		/// Execute the retrieval of the Person records that match the filter(s)
		/// </summary>
		/// <returns>Model list populated with the Person base entity properties</returns>
		PersonList List();
	}
}

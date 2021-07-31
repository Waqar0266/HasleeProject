﻿using Hasslefree.Web.Models.Listings;
using System;

namespace Hasslefree.Services.Listings.Crud
{
	public interface IListListingService
	{
		/// <summary>
		/// Filter records for records created before the parameter
		/// </summary>
		/// <param name="createdBefore">Created before parameter</param>
		/// <returns></returns>
		IListListingService CreatedBefore(DateTime? createdBefore);

		/// <summary>
		/// Filter records for records created after the parameter
		/// </summary>
		/// <param name="createdAfter">Created after parameter</param>
		/// <returns></returns>
		IListListingService CreatedAfter(DateTime? createdAfter);

		/// <summary>
		/// Search for records that match the parameter
		/// </summary>
		/// <param name="search">Search term parameter</param>
		/// <returns>Service instance</returns>
		IListListingService WithSearch(string search);

		/// <summary>
		/// Set pagination sizes for the returned records
		/// </summary>
		/// <param name="page">Set which subset to return</param>
		/// <param name="pageSize">Set number of records returned</param>
		/// <returns>Service instance</returns>
		IListListingService WithPaging(int page = 0, int pageSize = 50);

		/// <summary>
		/// Execute the retrieval of the records that match the filter(s)
		/// </summary>
		/// <param name="includeDates">Include DateTime properties in return</param>
		/// <param name="includePicturePath">Include the picture URL in return</param>
		/// <returns>Model list populated with the base entity properties</returns>
		ListingList List();
	}
}

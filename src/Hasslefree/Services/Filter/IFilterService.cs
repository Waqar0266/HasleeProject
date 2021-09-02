using Hasslefree.Web.Models.Filter;

namespace Hasslefree.Services.Filter
{
	public interface IFilterService
	{
		IFilterService WithPath(string path);
		IFilterService WithFilters(string buildingIds = null, string externalFeaturesIds = null, string roomIds = null);
		IFilterService WithPropertyId(int propertyId);
		IFilterService WithSearch(string search);
		IFilterService SortBy(string sortBy);
		IFilterService WithPaging(int page = 0, int pageSize = 50);

		FilterList List();
		FilterListItem Single();
	}
}

using Hasslefree.Web.Models.Filter;
using System.Collections.Generic;

namespace Hasslefree.Services.Filter
{
	public interface IAvailableFiltersService
	{
		IAvailableFiltersService WithItems(List<FilterListItem> items);
		AvailableFilterModel Get();
	}
}

using Hasslefree.Services.Filter;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Filter;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Listings
{
	public class AvailableFiltersController : BaseController
	{
		private IAvailableFiltersService Filters { get; }

		public AvailableFiltersController(IAvailableFiltersService filters)
		{
			Filters = filters;
		}

		[ChildActionOnly]
		public ActionResult GetFilters(List<FilterListItem> items)
		{
			var model = Filters.WithItems(items).Get();

			return PartialView("../Listings/_AvailableFilters", model);
		}
	}
}
using Hasslefree.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Listings
{
	public class AvailableFiltersController : BaseController
	{
		[ChildActionOnly]
		public ActionResult GetFilters(List<PropertyListItem)
		{
			var model = FilterService.WithPath(path).List();

			ViewBag.Title = $"Browse Listings - {model.CategoryName}";

			//get category

			return View("../Listings/List", model);
		}
	}
}
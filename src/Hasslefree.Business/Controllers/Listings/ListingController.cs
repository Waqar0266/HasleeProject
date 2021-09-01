using Hasslefree.Services.Filter;
using Hasslefree.Web.Framework;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Listings
{
	public class ListingController : BaseController
	{
		#region Private Properties

		//Services
		private IFilterService FilterService { get; }

		#endregion

		#region Constructor

		public ListingController
		(
			//Servcies
			IFilterService filterService
		)
		{
			//Services
			FilterService = filterService;
		}

		#endregion

		[Route("listings/{*path}")]
		public ActionResult ListingBrowse(string path)
		{
			var model = FilterService.WithPath(path).List();

			ViewBag.Title = $"Browse Listings - {model.CategoryName}";

			//get category

			return View("../Listings/List", model);
		}

		[Route("listing-detail/{name}/{propertyId}")]
		public ActionResult ListingDetails(string name, int propertyId)
		{
			var model = FilterService.WithPropertyId(propertyId).Single();

			ViewBag.Title = $"Browse Property - {model.Title}";

			//get category

			return View("../Listings/Details", model);
		}
	}
}
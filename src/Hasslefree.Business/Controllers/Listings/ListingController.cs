using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Data;
using Hasslefree.Services.Filter;
using Hasslefree.Web.Framework;
using System.Data.Entity;
using System.Linq;
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
	}
}
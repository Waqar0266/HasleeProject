using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Data;
using Hasslefree.Web.Framework;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Listings
{
	public class ListingController : BaseController
	{
		#region Private Properties

		private IReadOnlyRepository<Category> CategoryRepo { get; }

		#endregion

		#region Constructor

		public ListingController(IReadOnlyRepository<Category> categoryRepo)
		{
			CategoryRepo = categoryRepo;
		}

		#endregion

		[Route("listings/{*path}")]
		public ActionResult ListingBrowse(string path)
		{
			var category = CategoryRepo.Table.Include(c => c.ParentCategory).FirstOrDefault(c => c.Path.ToLower() == "/" + path.ToLower().Replace("-", " ").Replace("_", "-"));

			ViewBag.Title = $"Browse Listings - {category.Name}";

			//get category

			return View("../Listings/List", category);
		}
	}
}
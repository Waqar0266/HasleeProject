using Hasslefree.Services.Cache;
using Hasslefree.Web.Framework;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Home
{
	public partial class HomeController : BaseController
	{
		#region Fields

		// Services
		private ICacheManager CacheManager { get; }

		#endregion

		#region Constructor

		public HomeController
		(
			ICacheManager cacheManager
		)
		{
			// Services
			CacheManager = cacheManager;
		}

		#endregion

		public ActionResult Index()
		{
			ViewBag.Title = "Home";
			return View("Index");
		}
	}
}
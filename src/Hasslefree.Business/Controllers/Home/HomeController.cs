using Hasslefree.Services.Cache;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
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

		[CacheControl(Scope = System.Web.HttpCacheability.Public, MaxAgeSeconds = 900)]
		public ActionResult Index()
		{
			ViewBag.Title = "Home";
			return View("Index");
		}
	}
}
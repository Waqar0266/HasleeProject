using Hasslefree.Core;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Framework;
using System.Net;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers
{
	public class LocationController : BaseController
	{
		//Other
		private ICacheManager Cache { get; }

		public LocationController(
		//Other
		ICacheManager cache)
		{
			//Other
			Cache = cache;
		}

		[Route("location")]
		public ActionResult Index(string lat, string lng)
		{
			var response = Cache.Get<string>($"/location/{lat}/{lng}", CacheKeys.Time.LongTime, () => { return new WebClient().DownloadString($"http://api.positionstack.com/v1/reverse?access_key=0fa2767bcb340e847b1e9a6e0a648c05&query={lat}%2C{lng}&output=json"); });
			return Json(response, JsonRequestBehavior.AllowGet);
		}
	}
}
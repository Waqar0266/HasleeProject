using Hasslefree.Web.Framework;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Errors
{
	public class ErrorController : BaseController
	{
		[HttpGet, Route("error/403")]
		public ActionResult _403()
		{
			ViewBag.Title = "Unauthorized";
			Response.TrySkipIisCustomErrors = true;
			Response.StatusCode = 403;
			return View("403");
		}

		[HttpGet, Route("error/404")]
		public ActionResult _404()
		{
			ViewBag.Title = "Not Found";
			Response.TrySkipIisCustomErrors = true;
			Response.StatusCode = 404;
			return View("404");
		}

		[HttpGet, Route("error/500")]
		public ActionResult _500()
		{
			ViewBag.Title = "Server Error";
			Response.TrySkipIisCustomErrors = true;
			Response.StatusCode = 500;
			return View("500");
		}
	}
}
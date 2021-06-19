using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hasslefree.Business
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("sitemap.xml");
			routes.IgnoreRoute("robots.txt");
			routes.IgnoreRoute("{*alljpeg}", new { allaspx = @".*\.jpeg(/.*)?" });
			routes.IgnoreRoute("{*alljpg}", new { allaspx = @".*\.jpg(/.*)?" });
			routes.IgnoreRoute("{*allpng}", new { allaspx = @".*\.png(/.*)?" });
			routes.IgnoreRoute("{*allico}", new { allaspx = @".*\.ico(/.*)?" });
			routes.IgnoreRoute("{*allgif}", new { allaspx = @".*\.gif(/.*)?" });
			routes.IgnoreRoute("{*alleot}", new { allaspx = @".*\.eot(/.*)?" });
			routes.IgnoreRoute("{*allpdf}", new { allaspx = @".*\.pdf(/.*)?" });

			routes.LowercaseUrls = true;

			routes.RouteExistingFiles = true;

			//Default
			routes.MapRoute("Default"
				, "{controller}/{action}/{id}"
				, new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
				new String[] { "Hasslefree.Business.Controllers.Home" });
		}
	}
}

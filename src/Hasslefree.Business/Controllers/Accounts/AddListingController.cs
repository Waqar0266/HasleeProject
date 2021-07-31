using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	[AccessControlFilter(Permission = "Agent,Director")]
	[AgentFilter]
	public class AddListingController : BaseController
	{
		[HttpGet, Route("account/add-listing")]
		public ActionResult Index()
		{
			// Normal HTML
			return View("../Accounts/Listings/Select");
		}
	}
}
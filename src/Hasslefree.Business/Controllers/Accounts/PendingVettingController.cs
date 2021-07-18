using Hasslefree.Web.Framework;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Accounts
{
	public class PendingVettingController : BaseController
	{
		[Route("account/pending-vetting")]
		public ActionResult Index()
		{
			ViewBag.Title = "Pending Vetting";
			return View("../Accounts/PendingVetting");
		}
	}
}
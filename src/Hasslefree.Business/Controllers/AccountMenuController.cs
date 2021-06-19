using Hasslefree.Web.Framework;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers
{
	public class AccountMenuController : BaseController
	{
		public ActionResult Index()
		{
			return PartialView("_AccountMenu");
		}
	}
}
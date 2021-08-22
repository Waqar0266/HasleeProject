using Hasslefree.Web.Framework.Filters;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework
{
	[RolesFilter(Order = 3)]
	public class BaseController : Controller
	{
	}
}

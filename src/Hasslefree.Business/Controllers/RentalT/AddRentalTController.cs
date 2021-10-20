using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hasslefree.Business.Controllers.RentalT
{
	[AccessControlFilter(Permission = "Agent,Director")]
	[AgentFilter]
	public class AddRentalTController : BaseController
	{

	}
}
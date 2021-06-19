using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Annotations
{
	public class AjaxChildActionOnly : ActionMethodSelectorAttribute
	{
		public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
		{
			return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest() || controllerContext.IsChildAction;
		}
	}
}

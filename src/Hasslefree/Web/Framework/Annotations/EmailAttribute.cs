using System;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Annotations
{
	/// <inheritdoc />
	/// <summary>
	/// Used to decorate MVC actions that generate emails
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class EmailAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			filterContext.Controller.ViewBag.IsEmail = true;
		}
	}
}

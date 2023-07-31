using Hasslefree.Core;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Cache;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework.Filters
{
    public class RolesFilter : ActionFilterAttribute, IActionFilter
    {
        public ISessionManager SessionManager { get; set; }
        public ICacheManager Cache { get; set; }

        /// <summary>
        /// Perform a access control check on the currently logged in user
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SessionManager.IsLoggedIn()) return;

            var groups = Cache.Get(CacheKeys.Server.Framework.Filter.Roles, CacheKeys.Time.LongTime, () => { return SessionManager.Login.SecurityGroupLogins.Select(s => s.SecurityGroup.SecurityGroupName).ToList(); });

            filterContext.Controller.ViewBag.IsDirector = groups.Contains("Director");
            filterContext.Controller.ViewBag.IsAdmin = groups.Contains("Admin");
            filterContext.Controller.ViewBag.IsAgent = groups.Contains("Agent");
            filterContext.Controller.ViewBag.IsLandlord = groups.Contains("Landlord");
            filterContext.Controller.ViewBag.IsTenant = groups.Contains("Tenant");
            filterContext.Controller.ViewBag.RoleNames = string.Join(", ", groups);
        }
    }
}

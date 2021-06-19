using Newtonsoft.Json;
using Hasslefree.Web.Models.Common;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Get
{
	public class SecurityGroupPermission
	{
		public int PermissionId { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public string Group { get; set; }

		[JsonIgnore]
		public CrudAction Action { get; set; }
	}
}

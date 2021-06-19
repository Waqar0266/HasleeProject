using Newtonsoft.Json;
using Hasslefree.Web.Models.Common;

namespace Hasslefree.Web.Models.Security.SecurityGroups
{
	public class SecurityGroupBaseMemberModel
	{
		public SecurityGroupBaseMemberModel()
		{
			Action = CrudAction.None;
		}

		public int MemberLoginId { get; set; }
		public string MemberFullName { get; set; }
		public string MemberEmail { get; set; }

		[JsonIgnore]
		public CrudAction Action { get; set; }
	}
}

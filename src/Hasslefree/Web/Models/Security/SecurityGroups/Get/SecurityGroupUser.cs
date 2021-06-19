using System;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Get
{
	public class SecurityGroupUser
	{
		public int LoginId { get; set; }

		public DateTime? CreatedOn { get; set; }

		public string FullName { get; set; }

		public string Email { get; set; }
	}
}

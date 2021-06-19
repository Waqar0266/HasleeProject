using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Login.Update.SecurityGroups
{
	/// <summary>
	/// Update security groups linked to a login
	/// </summary>
	public class SecurityGroupsUpdate
	{
		/// <summary>
		/// Link login to the security groups
		/// </summary>
		public List<int> Add { get; set; }

		/// <summary>
		/// Unlink login from the security groups
		/// </summary>
		public List<int> Remove { get; set; }
	}
}

using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Get
{
	/// <summary>
	/// Security group get model
	/// </summary>
	public class SecurityGroupGet
	{
		/// <summary>
		/// Unique row identifier
		/// </summary>
		public int SecurityGroupId { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Name of the group
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description of the group
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Indication of whether the group is a system group or not
		/// </summary>
		public bool IsSystemSecurityGroup { get; set; }

		/// <summary>
		/// List of permissions linked to the group
		/// </summary>
		public List<SecurityGroupPermission> Permissions { get; set; }

		/// <summary>
		/// List of users linked to the group
		/// </summary>
		public List<SecurityGroupUser> Users { get; set; }
	}
}

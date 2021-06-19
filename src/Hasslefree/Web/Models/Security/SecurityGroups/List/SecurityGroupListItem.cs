using System;

namespace Hasslefree.Web.Models.Security.SecurityGroups.List
{
	/// <summary>
	/// Security group listing model
	/// </summary>
	public class SecurityGroupListItem
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
		/// Total permission linked to the group
		/// </summary>
		public int TotalPermissions { get; set; }

		/// <summary>
		/// Total users linked to the group
		/// </summary>
		public int TotalUsers { get; set; }
	}
}

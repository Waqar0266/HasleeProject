using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups.List
{
	/// <summary>
	/// Security group list model
	/// </summary>
	public class SecurityGroupList
	{
		/// <summary>
		/// Page of the list
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// Size of the page
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Total records in the database
		/// </summary>
		public int TotalRecords { get; set; }

		/// <summary>
		/// List of security group items
		/// </summary>
		public List<SecurityGroupListItem> Items { get; set; }
	}
}

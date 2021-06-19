using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Permissions.List
{
	/// <summary>
	/// Permission list model
	/// </summary>
	public class PermissionList
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
		/// List of permission items
		/// </summary>
		public List<PermissionListItem> Items { get; set; }
	}
}

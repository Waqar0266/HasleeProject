using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Sessions.List
{
	/// <summary>
	/// Session list model
	/// </summary>
	public class SessionList
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
		/// List of session items
		/// </summary>
		public List<SessionListItem> Items { get; set; }
	}
}

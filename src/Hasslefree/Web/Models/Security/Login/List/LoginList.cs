using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Login.List
{
	/// <summary>
	/// Login list model
	/// </summary>
	public class LoginList
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
		/// List of login items
		/// </summary>
		public List<LoginListItem> Items { get; set; }
	}
}

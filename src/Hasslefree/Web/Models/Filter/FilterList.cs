using System.Collections.Generic;

namespace Hasslefree.Web.Models.Filter
{
	public class FilterList
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

		public string CategoryName { get; set; }
		public string CategoryPath { get; set; }

		/// <summary>
		/// List of category items
		/// </summary>
		public List<FilterListItem> Items { get; set; }
	}
}

using System.Collections.Generic;

namespace Hasslefree.Web.Models.Sales
{
    public class SaleList
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
		/// List of category items
		/// </summary>
		public List<SaleListItem> Items { get; set; }
	}
}

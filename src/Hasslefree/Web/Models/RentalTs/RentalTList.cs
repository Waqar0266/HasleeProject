using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
    public class RentalTList
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
		public List<RentalTListItem> Items { get; set; }
	}
}

using System.Collections.Generic;

namespace Hasslefree.Web.Models.People.List
{
	/// <summary>
	/// Person list model
	/// </summary>
	public class PersonList
	{
		/// <summary>
		/// Page number of the list
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
		/// List of person items
		/// </summary>
		public List<PersonListItem> Items { get; set; }
	}
}

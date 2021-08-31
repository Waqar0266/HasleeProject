using System;

namespace Hasslefree.Web.Models.Catalog.Categories.List
{
	/// <summary>
	/// Category listing item
	/// </summary>
	public class CategoryListItem
	{
		/// <summary>
		/// Unique row identifier
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Unique category path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Nest leveled
		/// </summary>
		public int NestedLevel { get; set; }

		/// <summary>
		/// Display order
		/// </summary>
		public int DisplayOrder { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the parent category
		/// </summary>
		public int? ParentCategoryId { get; set; }

		/// <summary>
		/// Indication of whether the category should be hidden or not
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Tags
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Total number of products linked to the category
		/// </summary>
		public int TotalProducts { get; set; }
	}
}

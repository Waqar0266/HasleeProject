namespace Hasslefree.Web.Models.Catalog.Categories.Get
{
	/// <summary>
	/// Product linked to a category
	/// </summary>
	public class CategoryProductGet
	{
		/// <summary>
		/// Unique row identifier
		/// </summary>
		public int ProductId { get; set; }

		/// <summary>
		/// Unique SKU code
		/// </summary>
		public string Sku { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }
	}
}

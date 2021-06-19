using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Web.Models.Catalog.Categories
{
	public class MiniCategoryModel
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string ParentPath { get; set; }
		public Picture Picture { get; set; }
	}
}

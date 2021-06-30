using System.Collections.Generic;

namespace Hasslefree.Web.Models.Menu
{
	public class MenuItem
	{
		public MenuItem()
		{
			this.Children = new List<MenuItem>();
		}
		public int CategoryId { get; set; }
		public int MenuItemId { get; set; }
		public int NestedLevel { get; set; }
		public int DisplayOrder { get; set; }
		public string Text { get; set; }
		public string Path { get; set; }
		public string LinkUrl { get; set; }
		public string LinkTarget { get; set; }
		public int? ParentId { get; set; }

		public MenuItem Parent { get; set; }
		public List<MenuItem> Children { get; set; }
	}
}

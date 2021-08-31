using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Catalog
{
	public class Category : BaseEntity
	{
		#region Constructor

		public Category()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			NestedLevel = 0;
			DisplayOrder = 0;
			Hidden = false;

			SubCategories = new HashSet<Category>();
		}

		#endregion

		#region Properties

		public int CategoryId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int NestedLevel { get; set; }
		public int DisplayOrder { get; set; }
		public string Path { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Hidden { get; set; }
		public string Tag { get; set; }
		public int? ParentCategoryId { get; set; }

		#endregion

		#region Navigation

		public ICollection<Category> SubCategories { get; set; }
		public Category ParentCategory { get; set; }

		#endregion
	}
}

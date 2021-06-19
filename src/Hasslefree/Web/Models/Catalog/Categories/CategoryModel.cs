using Hasslefree.Web.Models.Media.Pictures;
using System;

namespace Hasslefree.Web.Models.Catalog.Categories
{
	public class CategoryModel
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public bool HasShortDescription => !String.IsNullOrWhiteSpace(ShortDescription);
		public string FullDescription { get; set; }
		public bool HasFullDescription => !String.IsNullOrWhiteSpace(FullDescription);
		public string Path { get; set; }
		public string Url { get; set; }
		public bool HasParent => ParentCategory != null;
		public CategoryModel ParentCategory { get; set; }
		public bool HasPicture => Picture != null;
		public PictureModel Picture { get; set; }
	}
}
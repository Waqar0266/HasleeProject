using System;

namespace Hasslefree.Web.Models.Common
{
	public class PictureViewModel
	{
		public Int32 PictureId { get; set; }
		public String FileName { get; set; }
		public String SEOFriendlyName { get; set; }
		public String AlternateText { get; set; }
		public String MimeType { get; set; }
		public String ButtonPictureUrl { get; set; }
		public String ThumbnailPictureUrl { get; set; }
		public String SmallPictureUrl { get; set; }
		public String StandardPictureUrl { get; set; }
		public String LargePictureUrl { get; set; }
		public String ZoomPictureUrl { get; set; }
		public String OriginalPictureUrl { get; set; }
		public Int32? DisplayOrder { get; set; }
	}
}

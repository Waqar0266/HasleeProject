using System;

namespace Hasslefree.Web.Models.Media.Pictures
{
	public class MediaTransformModel
	{
		public Int32? PictureId { get; set; }
		public Int32? Height { get; set; }
		public Int32? Width { get; set; }
		public String FitMode { get; set; }
		public String Format { get; set; }
		public Int32? CropX1 { get; set; }
		public Int32? CropY1 { get; set; }
		public Int32? CropX2 { get; set; }
		public Int32? CropY2 { get; set; }
		public Boolean WhiteSpaceTrim { get; set; }
	}
}

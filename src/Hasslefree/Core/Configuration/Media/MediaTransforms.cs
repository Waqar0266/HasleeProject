using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Hasslefree.Core.Configuration.Media
{
	[TypeConverter(typeof(MediaTransformConverter))]
	[JsonConverter(typeof(NoTypeConverterJsonConverter<MediaTransforms>))]
	public class MediaTransforms
	{
		public Int32? Height { get; set; }
		public Int32? Width { get; set; }
		public String FitMode { get; set; }
		public String Format { get; set; }
		public Int32? CropX1 { get; set; }
		public Int32? CropY1 { get; set; }
		public Int32? CropX2 { get; set; }
		public Int32? CropY2 { get; set; }
		public Boolean WhiteSpaceTrim { get; set; }

		public override String ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}

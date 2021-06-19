using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Configuration.Media
{
	public class MediaTransformSettings : ISettings
	{
		public MediaTransformSettings()
		{
			Product = new MediaTransforms
			{
				WhiteSpaceTrim = false
			};
			Category = new MediaTransforms
			{
				WhiteSpaceTrim = false
			};
			Manufacturer = new MediaTransforms
			{
				WhiteSpaceTrim = false
			};
			Logo = new MediaTransforms
			{
				WhiteSpaceTrim = false
			};
			Favicon = new MediaTransforms
			{
				Width = 32,
				Height = 32,
				FitMode = "max",
				WhiteSpaceTrim = false
			};
			Content = new MediaTransforms
			{
				WhiteSpaceTrim = false
			};
		}

		public MediaTransforms Product { get; set; }
		public MediaTransforms Category { get; set; }
		public MediaTransforms Manufacturer { get; set; }
		public MediaTransforms Logo { get; set; }
		public MediaTransforms Favicon { get; set; }
		public MediaTransforms Content { get; set; }

	}
}

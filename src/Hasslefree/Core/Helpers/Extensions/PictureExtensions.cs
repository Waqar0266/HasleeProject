using System;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class PictureExtensions
	{
		private const String Protocol = "https";

		public static String GetUrl(this Picture picture)
		{
			// Ensure the picture is not null
			if (picture == null)
				return "";

			// Ensure it has a path
			if (String.IsNullOrWhiteSpace(picture.Path))
				return "";

			var image = picture.Path;

			// If the image starts with a URL then just return it as it is complete
			if(image.StartsWith("https"))
				return image;

			// If it is a relative path then add the URL Scheme
			if(image.StartsWith("//"))
				return $"{Protocol}:{image}";

			return image;
		}
	}
}

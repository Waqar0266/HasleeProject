using System;
using Hasslefree.Web.Models.Media.Pictures;

namespace Hasslefree.Services.Media.Pictures
{
	public interface IDefaultTransformService
	{
		IDefaultTransformService With(Int32? height = null, Int32? width = null, String fitMode = null, String format = null,
									Int32? cropX1 = null, Int32? cropY1 = null, Int32? cropX2 = null, Int32? cropY2 = null,
									Boolean whitespaceTrim = false);
		IDefaultTransformService For(PictureType type);
		String Get();
	}
}

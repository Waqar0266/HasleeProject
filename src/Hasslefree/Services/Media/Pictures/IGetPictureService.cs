using System;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Services.Media.Pictures
{
	public interface IGetPictureService
	{
		Picture this[Int32 pictureId] { get; }
	}
}

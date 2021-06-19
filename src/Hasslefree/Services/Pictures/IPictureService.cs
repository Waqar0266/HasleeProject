using System;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Services.Pictures
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public interface IPictureService
	{
		void InsertPicture(Picture picture);
		Picture GetPicture(int pictureId);
	}
}

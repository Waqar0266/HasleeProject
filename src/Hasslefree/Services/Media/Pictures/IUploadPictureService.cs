using System;
using System.Collections.Generic;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Web.Models.Media.Pictures.Crud;

namespace Hasslefree.Services.Media.Pictures
{
	public interface IUploadPictureService
	{
		IUploadPictureService WithPath(String path);
		IUploadPictureService Add(PictureModel picture);
		IUploadPictureService Add(IEnumerable<PictureModel> pictures);

		List<Picture> Return();
		List<Picture> Save();
	}
}
using System;
using System.Collections.Generic;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Web.Models.Media.Pictures.Crud;

namespace Hasslefree.Services.Media.Pictures
{
	public interface IRemovePictureService
	{
		IRemovePictureService Add(Int32 pictureId);
		IRemovePictureService Add(PictureModel picture);
		IRemovePictureService Add(IEnumerable<Int32> pictureIds);
		IRemovePictureService Add(IEnumerable<PictureModel> pictures);

		void Remove();
	}
}
using System;
using System.Linq;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;

namespace Hasslefree.Services.Media.Pictures
{
	public class GetPictureService : IGetPictureService, IInstancePerRequest
	{
		#region Private Properties

		private IReadOnlyRepository<Picture> PictureRepo { get; }

		#endregion

		#region Constructor

		public GetPictureService(
				IReadOnlyRepository<Picture> pictureRepo
			)
		{
			PictureRepo = pictureRepo;
		}

		#endregion

		#region IGetPictureService

		public Picture this[Int32 id]
		{
			get
			{
				return PictureRepo.Table.FirstOrDefault(a => a.PictureId == id);
			}
		}

		#endregion
	}
}

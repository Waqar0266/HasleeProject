using System;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using System.Linq;

namespace Hasslefree.Services.Pictures
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public class PictureService : IPictureService
	{
		/* Dependencies */
		private IDataRepository<Picture> PictureRepo { get; }
		
		/* CTOR */
		public PictureService(IDataRepository<Picture> pictureRepo)
		{
			PictureRepo = pictureRepo;
		}
		
		/// <summary>
		/// Inserts a new picture
		/// </summary>
		/// <param name="picture">A Picture object</param>
		public virtual void InsertPicture(Picture picture)
		{
			PictureRepo.Insert(picture);
		}

		/// <summary>
		/// Get an existing picture
		/// </summary>
		/// <param name="pictureId"></param>
		/// <returns></returns>
		public virtual Picture GetPicture(int pictureId)
		{
			return PictureRepo.Table.FirstOrDefault(p => p.PictureId == pictureId);
		}
	}
}



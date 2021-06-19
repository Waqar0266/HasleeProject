using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Web.Models.Media.Pictures
{
	public static class PictureModelExtensions
	{
		public const string NoImage = "/images/no-image.jpg";

		public static PictureModel GetCoverPicture(this List<PictureModel> list)
		{
			return !list.Any() ? new PictureModel() : list.FirstOrDefault();
		}

		public static PictureModel GetCoverPicture(this List<PictureModel> list, PictureUseType type)
		{
			var blankPicture = new PictureModel();

			if (!list.Any()) return blankPicture;

			switch (type)
			{
				case PictureUseType.List:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.List) ?? blankPicture;
				case PictureUseType.MiniCart:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.MiniCart) ?? blankPicture;
				case PictureUseType.DetailCarouselMain:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.DetailCarouselMain) ?? blankPicture;
				case PictureUseType.DetailCarouselThumbnail:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.DetailCarouselThumbnail) ?? blankPicture;
				case PictureUseType.Button:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.Button) ?? blankPicture;
				case PictureUseType.Thumbnail:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.Thumbnail) ?? blankPicture;
				case PictureUseType.Small:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.Small) ?? blankPicture;
				case PictureUseType.Standard:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.Standard) ?? blankPicture;
				case PictureUseType.Large:
					return list.FirstOrDefault(p => p.PictureUseType == PictureUseType.Large) ?? blankPicture;
				default:
					return blankPicture;
			}
		}

		public static List<PictureModel> GetPictures(this List<PictureModel> list, PictureUseType type)
		{
			if (!list.Any()) return new List<PictureModel>();

			switch (type)
			{
				case PictureUseType.List:
					return list.Where(p => p.PictureUseType == PictureUseType.List).ToList();
				case PictureUseType.MiniCart:
					return list.Where(p => p.PictureUseType == PictureUseType.MiniCart).ToList();
				case PictureUseType.DetailCarouselMain:
					return list.Where(p => p.PictureUseType == PictureUseType.DetailCarouselMain).ToList();
				case PictureUseType.DetailCarouselThumbnail:
					return list.Where(p => p.PictureUseType == PictureUseType.DetailCarouselThumbnail).ToList();
				case PictureUseType.Button:
					return list.Where(p => p.PictureUseType == PictureUseType.Button).ToList();
				case PictureUseType.Thumbnail:
					return list.Where(p => p.PictureUseType == PictureUseType.Thumbnail).ToList();
				case PictureUseType.Small:
					return list.Where(p => p.PictureUseType == PictureUseType.Small).ToList();
				case PictureUseType.Standard:
					return list.Where(p => p.PictureUseType == PictureUseType.Standard).ToList();
				case PictureUseType.Large:
					return list.Where(p => p.PictureUseType == PictureUseType.Large).ToList();
				default:
					return new List<PictureModel>();
			}
		}
	}
}

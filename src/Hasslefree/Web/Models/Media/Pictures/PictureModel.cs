using Newtonsoft.Json;
using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Web.Models.Media.Pictures
{
	public class PictureModel
	{
		public PictureModel()
		{
			PictureUseType = PictureUseType.Default;
			PictureFormat = PictureFormat.Jpeg;
			DisplayOrder = 0;
		}

		/// <summary>
		/// Unique identifier
		/// </summary>
		public int? PictureId { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// S3 Bucket path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Alternative text
		/// </summary>
		public string AltText { get; set; }

		/// <summary>
		/// Display order
		/// </summary>
		public int DisplayOrder { get; set; }

		/// <summary>String representation of the PictureUseType enum [case sensitive]
		/// - Default
		/// - List
		/// - MiniCart
		/// - DetailCarouselMain
		/// - DetailCarouselThumbnail
		/// - Button
		/// - Thumbnail
		/// - Small
		/// - Standard
		/// - Large
		/// </summary>
		public string PictureUseTypeEnum { get; set; }

		/// <summary> </summary>
		[JsonIgnore]
		public PictureUseType PictureUseType
		{
			get => (PictureUseType)Enum.Parse(typeof(PictureUseType), PictureUseTypeEnum);
			set => PictureUseTypeEnum = value.ToString();
		}

		/// <summary>String representation of the PictureFormat enum [case sensitive]
		/// - Jpeg
		/// - Png
		/// - Bitmap
		/// - Gif
		/// - Tif
		/// </summary>
		public string FormatEnum { get; set; }

		/// <summary></summary>
		[JsonIgnore]
		public PictureFormat PictureFormat
		{
			get => (PictureFormat)Enum.Parse(typeof(PictureFormat), FormatEnum);
			set => FormatEnum = value.ToString();
		}

		/// <summary>
		/// Transforms
		/// </summary>
		public string Transforms { get; set; }
	}
}
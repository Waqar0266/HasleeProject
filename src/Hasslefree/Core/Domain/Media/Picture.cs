using Hasslefree.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Media
{
	/// <summary>
	/// Represents an abstract of a picture of a product, category etc.
	/// </summary>
	/// <remarks>
	/// The Picture object represents an abstract of a picture and contains only the meta data associated with the picture, but not the actual picture data.
	/// The actual picture data is stored either on disk, in the cloud or in the database.
	/// Each picture can have multiple instances, where each instance is resized to approximately the correct dimensions.
	/// </remarks>
	[Serializable]
	public class Picture : BaseEntity
	{
		public Picture()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			Categories = new HashSet<Category>();
		}

		public int PictureId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string MimeType { get; set; }
		public string FormatEnum { get; set; }
		public string Name { get; set; }
		public string Folder { get; set; }
		public string Path { get; set; }
		public string AltText { get; set; }
		public string Transforms { get; set; }

		/// <summary>
		/// Gets or sets the format of the picture
		/// </summary>
		public PictureFormat Format
		{
			get => (PictureFormat)Enum.Parse(typeof(PictureFormat), FormatEnum);
			set => FormatEnum = value.ToString();
		}

		public ICollection<Category> Categories { get; set; }
	}
}

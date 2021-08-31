using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Properties
{
	public class PropertyPicture : BaseEntity
	{
		public PropertyPicture()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int PropertyPictureId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int PropertyId { get; set; }
		public Property Property { get; set; }
		public int PictureId { get; set; }
		public Picture Picture { get; set; }
	}
}

using System;

namespace Hasslefree.Core.Domain.Common
{
	public class Content : BaseEntity
	{
		public int ContentId { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Html { get; set; }
		public string ContentTypeEnum { get; set; }
		public ContentType ContentType
		{
			get => (ContentType)Enum.Parse(typeof(ContentType), ContentTypeEnum);
			set => ContentTypeEnum = value.ToString();
		}
	}
}

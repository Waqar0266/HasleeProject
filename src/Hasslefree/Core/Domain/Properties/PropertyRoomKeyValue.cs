using System;

namespace Hasslefree.Core.Domain.Properties
{
	public class PropertyRoomKeyValue : BaseEntity
	{
		public PropertyRoomKeyValue()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int PropertyRoomKeyValueId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int PropertyId { get; set; }
		public Property Property { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}

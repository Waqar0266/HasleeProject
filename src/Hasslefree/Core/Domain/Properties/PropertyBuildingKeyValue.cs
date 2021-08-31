using System;

namespace Hasslefree.Core.Domain.Properties
{
	public class PropertyBuildingKeyValue : BaseEntity
	{
		public PropertyBuildingKeyValue()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int PropertyBuildingKeyValueId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int PropertyId { get; set; }
		public Property Property { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}

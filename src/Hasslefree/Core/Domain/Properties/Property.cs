using Hasslefree.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Properties
{
	public class Property : BaseEntity
	{
		public Property()
		{
			this.CreatedOn = DateTime.Now;
			this.Images = new HashSet<PropertyPicture>();
			this.OverviewKeyValues = new HashSet<PropertyOverviewKeyValue>();
			this.RoomKeyValues = new HashSet<PropertyRoomKeyValue>();
			this.ExternalFeaturesKeyValues = new HashSet<PropertyExternalFeaturesKeyValue>();
			this.BuildingKeyValues = new HashSet<PropertyBuildingKeyValue>();
			this.OtherFeaturesKeyValues = new HashSet<PropertyOtherFeaturesKeyValue>();
		}

		public int PropertyId { get; set; }
		public string PrivatePropertyId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; }
		public string Title { get; set; }
		public string Address { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string PropertyTypeEnum { get; set; }
		public PropertyType PropertyType
		{
			get => (PropertyType)Enum.Parse(typeof(PropertyType), PropertyTypeEnum);
			set => PropertyTypeEnum = value.ToString();
		}
		public ICollection<PropertyPicture> Images { get; set; }
		public ICollection<PropertyOverviewKeyValue> OverviewKeyValues { get; set; }
		public ICollection<PropertyRoomKeyValue> RoomKeyValues { get; set; }
		public ICollection<PropertyExternalFeaturesKeyValue> ExternalFeaturesKeyValues { get; set; }
		public ICollection<PropertyBuildingKeyValue> BuildingKeyValues { get; set; }
		public ICollection<PropertyOtherFeaturesKeyValue> OtherFeaturesKeyValues { get; set; }
	}
}

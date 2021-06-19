using System;

namespace Hasslefree.Core.Domain.Common
{
	/// <summary>
	/// Represents an address or contact details of a person or order
	/// </summary>
	[Serializable]
	public class Address : BaseEntity
	{
		public Address()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			Deleted = false;
			Type = AddressType.Residential;
		}
		
		public int AddressId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string Town { get; set; }
		public string Code { get; set; }
		public string Country { get; set; }
		public string RegionName { get; set; }
		public bool Deleted { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }

		public string TypeEnum { get; set; }
		public AddressType Type
		{
			get => String.IsNullOrWhiteSpace(TypeEnum) ? AddressType.Residential : (AddressType)Enum.Parse(typeof(AddressType), TypeEnum);
			set => TypeEnum = value.ToString();
		}

	}
}

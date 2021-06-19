using System;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Web.Models.Addresses
{
	public class AddressModel
	{
		#region Address

		public int AddressId { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string City { get; set; }
		public string RegionName { get; set; }
		public string Code { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

		public string TypeEnum { get; set; }
		public AddressType Type
		{
			get => String.IsNullOrWhiteSpace(TypeEnum) ? AddressType.Residential : (AddressType)Enum.Parse(typeof(AddressType), TypeEnum);
			set => TypeEnum = value.ToString();
		}

		#endregion

		#region Country

		public int CountryId { get; set; }
		public string CountryName { get; set; }
		public string CountryTwoLetterIsoCode { get; set; }
		public string CountryThreeLetterIsoCode { get; set; }

		#endregion
    }
}
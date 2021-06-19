using System;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Services.Common.Addresses
{
	public interface IUpdateAddressService
	{
		IUpdateAddressService this[Int32 id] { get; }
		
		IUpdateAddressService WithType(AddressType type);
		IUpdateAddressService WithAddress1(String address1);
		IUpdateAddressService WithAddress2(String address2);
		IUpdateAddressService WithAddress3(String address3);
		IUpdateAddressService WithTown(String town);
		IUpdateAddressService WithCode(String code);
		IUpdateAddressService WithCountry(String country);
		IUpdateAddressService WithRegion(String region);
		IUpdateAddressService WithLocation(String latitude, String longitude);

		Boolean Update();
	}
}

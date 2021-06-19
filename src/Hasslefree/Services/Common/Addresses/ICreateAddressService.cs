using System;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Services.Common.Addresses
{
	public interface ICreateAddressService
	{
		ICreateAddressService New(AddressType type);
		
		ICreateAddressService WithAddress1(String address1);
		ICreateAddressService WithAddress2(String address2);
		ICreateAddressService WithAddress3(String address3);
		ICreateAddressService WithTown(String town);
		ICreateAddressService WithCode(String code);
		ICreateAddressService WithCountry(String country);
		ICreateAddressService WithRegion(String region);
		ICreateAddressService WithLocation(String latitude, String longitude);

		Boolean Create(Boolean save = true);

		Core.Domain.Common.Address Result { get; }
	}
}

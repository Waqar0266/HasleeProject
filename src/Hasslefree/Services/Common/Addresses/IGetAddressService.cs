using System;
using Hasslefree.Web.Models.Addresses;

namespace Hasslefree.Services.Common.Addresses
{
	public interface IGetAddressService
	{
		/// <summary>
		/// Get an address based on the specified address id
		/// Cached
		/// </summary>
		/// <param name="addressId"></param>
		/// <returns></returns>
		AddressModel this[Int32 addressId] { get; }
	}
}

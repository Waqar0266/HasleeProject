using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Addresses;

namespace Hasslefree.Services.Common.Addresses
{
	public class GetAddressService : IGetAddressService, IInstancePerRequest
	{
		#region Private Properties

		// Data Repositories
		private IReadOnlyRepository<Address> AddressRepo { get; }

		// Managers
		private ICacheManager Cache { get; }

		#endregion

		#region Constrcutor

		public GetAddressService(
				IReadOnlyRepository<Address> addressRepo, 
				ICacheManager cache
			)
		{
			AddressRepo = addressRepo;
			Cache = cache;
		}

		#endregion

		#region IGetAddressService

		public AddressModel this[Int32 addressId] => GetAddress(addressId);

		#endregion

		#region Private Methods

		private AddressModel GetAddress(Int32 addressId)
		{
			var cached = Cache.Get<Dictionary<Int32, AddressModel>>($"/addresses/{addressId}") ?? new Dictionary<Int32, AddressModel>();

			if (cached.ContainsKey(addressId))
				return cached[addressId];

			var address = AddressRepo.Table.FirstOrDefault(a => a.AddressId == addressId);

			var model = address != null ? new AddressModel()
			{
				AddressId = address.AddressId,
				Address1 = address.Address1,
				Address2 = address.Address2,
				Address3 = address.Address3,
				Latitude = address.Latitude,
				Longitude = address.Longitude,
				RegionName = address.RegionName,
				City = address.Town,
				Code = address.Code,
				CountryName = address.Country,
				Type = address.Type
			} : null;
			
			if(!cached.ContainsKey(addressId))
				cached.Add(addressId, model);
			Cache.Set($"/addresses/{addressId}", cached, CacheKeys.Time.DefaultTime);

			return model;
		}

		#endregion
	}
}

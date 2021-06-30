using System;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;

namespace Hasslefree.Services.Common.Addresses
{
	public class UpdateAddressService : IUpdateAddressService, IInstancePerRequest
	{
		#region Private Properties

		private IDataRepository<Address> AddressRepo { get; }

		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private Address _address;

		#endregion

		#region Constructor

		public UpdateAddressService(
				IDataRepository<Address> addressRepo,
				ICacheManager cache
			)
		{
			AddressRepo = addressRepo;

			Cache = cache;
		}

		#endregion

		#region IUpdateAddressService

		public IUpdateAddressService this[Int32 id]
		{
			get
			{
				_address = AddressRepo.Table.FirstOrDefault(a => a.AddressId == id);
				return this;
			}
		}

		public IUpdateAddressService WithType(AddressType type)
		{
			_address.Type = type;

			return this;
		}

		public IUpdateAddressService WithAddress1(String address1)
		{
			_address.Address1 = address1;

			return this;
		}

		public IUpdateAddressService WithAddress2(String address2)
		{
			_address.Address2 = address2;

			return this;
		}

		public IUpdateAddressService WithAddress3(String address3)
		{
			_address.Address3 = address3;

			return this;
		}

		public IUpdateAddressService WithTown(String town)
		{
			_address.Town = town;

			return this;
		}

		public IUpdateAddressService WithCode(String code)
		{
			_address.Code = code;
			return this;
		}

		public IUpdateAddressService WithCountry(String country)
		{
			_address.Country = country;

			return this;
		}

		public IUpdateAddressService WithRegion(String region)
		{
			_address.RegionName = region;

			return this;
		}

		public IUpdateAddressService WithLocation(String latitude, String longitude)
		{
			_address.Latitude = latitude;
			_address.Longitude = longitude;
			
			return this;
		}

		public Boolean Update()
		{
			if (_address == null)
				return Clean(false);

			try
			{
				_address.ModifiedOn = DateTime.Now;

				AddressRepo.Update(_address);

				return Clean(true);
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
			return Clean(false);
		}

		#endregion

		#region Private Methods

		private Boolean Clean(Boolean success)
		{
			_address = null;

			return success;
		}

		#endregion
	}
}

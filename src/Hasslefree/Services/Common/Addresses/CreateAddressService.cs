using System;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;

namespace Hasslefree.Services.Common.Addresses
{
	public class CreateAddressService : ICreateAddressService
	{
		#region Private Properties

		private IDataRepository<Address> AddressRepo { get; }
		private IDataContext Database { get; }
		#endregion

		#region Fields

		private Address _address;
		private Boolean _success;

		#endregion

		#region Constructor

		public CreateAddressService(IDataRepository<Address> addressRepo, IDataContext database)
		{
			AddressRepo = addressRepo;
			Database = database;
		}

		#endregion

		#region ICreateAddressService

		public ICreateAddressService New(AddressType type)
		{
			_address = new Address()
			{
				CreatedOn = DateTime.Now,
				ModifiedOn = DateTime.Now,
				Type = type
			};

			return this;
		}

		public ICreateAddressService WithAddress1(String address1)
		{
			_address.Address1 = address1;

			return this;
		}

		public ICreateAddressService WithAddress2(String address2)
		{
			_address.Address2 = address2;

			return this;
		}

		public ICreateAddressService WithAddress3(String address3)
		{
			_address.Address3 = address3;

			return this;
		}

		public ICreateAddressService WithTown(String town)
		{
			_address.Town = town;

			return this;
		}

		public ICreateAddressService WithCode(String code)
		{
			_address.Code = code;
			return this;
		}

		public ICreateAddressService WithCountry(String country)
		{
			_address.Country = country;

			return this;
		}

		public ICreateAddressService WithRegion(String region)
		{
			_address.RegionName = region;

			return this;
		}

		public ICreateAddressService WithLocation(String latitude, String longitude)
		{
			_address.Latitude = latitude;
			_address.Longitude = longitude;

			return this;
		}

		public Boolean Create(Boolean save = true)
		{
			if (_address == null)
				return _success;

			try
			{
				AddressRepo.Add(_address);

				if (save)
					Database.SaveChanges();

				_success = true;
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
			return _success;
		}

		public Address Result => _success ? _address : null;

		#endregion
	}
}

using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Linq;

namespace Hasslefree.Services.Common
{
	public class UpdateFirmService : IUpdateFirmService, IInstancePerRequest
	{
		#region Private Properties

		private IDataRepository<Firm> FirmRepo { get; }
		private IDataRepository<Address> AddressRepo { get; }
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Address _postalAddress;
		private Address _physicalAddress;
		private string _businessName;
		private string _tradeName;
		private string _phone;
		private string _fax;
		private string _email;
		private string _referenceNumber;
		private string _aiNumber;

		#endregion

		#region Constructor

		public UpdateFirmService(
								//Repos
								IDataRepository<Firm> firmRepo,
								IDataRepository<Address> addressRepo,

								//Other
								IDataContext database
								)
		{
			//Repos
			FirmRepo = firmRepo;
			AddressRepo = addressRepo;

			//Other
			Database = database;
		}

		#endregion

		#region ICreateAddressService

		public IUpdateFirmService WithSettings(string businessName, string tradeName, string phone, string fax, string email, string referenceNumber, string aiNumber)
		{
			_businessName = businessName;
			_tradeName = tradeName;
			_phone = phone;
			_fax = fax;
			_email = email;
			_referenceNumber = referenceNumber;
			_aiNumber = aiNumber;

			return this;
		}

		public IUpdateFirmService WithPhysicalAddress(string address1, string address2, string address3, string town, string code, string country, string region)
		{
			_physicalAddress = new AddressBuilder(address1, address2, address3, town, country, region, code).AutoCoordinates().WithType(AddressType.Residential).Build();
			return this;
		}

		public IUpdateFirmService WithPostalAddress(string address1, string address2, string address3, string town, string code, string country, string region)
		{
			_postalAddress = new AddressBuilder(address1, address2, address3, town, country, region, code).AutoCoordinates().WithType(AddressType.Postal).Build();
			return this;
		}

		public void Update()
		{
			try
			{
				var firm = FirmRepo.Table.FirstOrDefault();
				if (firm == null)
				{
					FirmRepo.Insert(new Firm()
					{
						AiNumber = _aiNumber,
						BusinessName = _businessName,
						Email = _email,
						Fax = _fax,
						Phone = _phone,
						PhysicalAddress = _physicalAddress,
						PostalAddress = _postalAddress,
						ReferenceNumber = _referenceNumber,
						TradeName = _tradeName
					});
				}
				else
				{
					var physicalAddress = AddressRepo.Table.FirstOrDefault(a => a.AddressId == firm.PhysicalAddressId);
					var postalAddress = AddressRepo.Table.FirstOrDefault(a => a.AddressId == firm.PostalAddressId);

					physicalAddress.Address1 = _physicalAddress.Address1;
					physicalAddress.Address2 = _physicalAddress.Address2;
					physicalAddress.Address3 = _physicalAddress.Address3;
					physicalAddress.Code = _physicalAddress.Code;
					physicalAddress.Country = _physicalAddress.Country;
					physicalAddress.Latitude = _physicalAddress.Latitude;
					physicalAddress.Longitude = _physicalAddress.Longitude;
					physicalAddress.RegionName = _physicalAddress.RegionName;
					physicalAddress.Town = _physicalAddress.Town;
					physicalAddress.Type = _physicalAddress.Type;

					postalAddress.Address1 = _postalAddress.Address1;
					postalAddress.Address2 = _postalAddress.Address2;
					postalAddress.Address3 = _postalAddress.Address3;
					postalAddress.Code = _postalAddress.Code;
					postalAddress.Country = _postalAddress.Country;
					postalAddress.Latitude = _postalAddress.Latitude;
					postalAddress.Longitude = _postalAddress.Longitude;
					postalAddress.RegionName = _postalAddress.RegionName;
					postalAddress.Town = _postalAddress.Town;
					postalAddress.Type = _postalAddress.Type;

					AddressRepo.Update(physicalAddress);
					AddressRepo.Update(postalAddress);

					firm.AiNumber = _aiNumber;
					firm.BusinessName = _businessName;
					firm.Email = _email;
					firm.Fax = _fax;
					firm.Phone = _phone;
					firm.ReferenceNumber = _referenceNumber;
					firm.TradeName = _tradeName;

					FirmRepo.Update(firm);
				}
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
		}

		#endregion
	}
}

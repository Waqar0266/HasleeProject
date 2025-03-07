﻿using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Common;
using System.Linq;

namespace Hasslefree.Services.Common
{
	public class GetFirmService : IGetFirmService, IInstancePerRequest
	{
		#region Private Properties

		private IReadOnlyRepository<Firm> FirmRepo { get; }
		private IReadOnlyRepository<Address> AddressRepo { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Constructor

		public GetFirmService(
				IReadOnlyRepository<Firm> firmRepo,
				IReadOnlyRepository<Address> addressRepo,
				ICacheManager cache
			)
		{
			FirmRepo = firmRepo;
			AddressRepo = addressRepo;
			Cache = cache;
		}

		#endregion

		#region IGetFirmService

		/// <inheritdoc />
		public FirmModel Get()
		{
			var firm = Cache.Get(CacheKeys.Server.Firms.Path, CacheKeys.Time.LongTime, () => FirmRepo.Table.FirstOrDefault());

			if (firm == null) return new FirmModel();

			var physicalAddress = Cache.Get(CacheKeys.Server.Addresses.AddressById(firm.PhysicalAddressId), CacheKeys.Time.LongTime, () => AddressRepo.Table.FirstOrDefault(a => a.AddressId == firm.PhysicalAddressId));
			var postalAddress = Cache.Get(CacheKeys.Server.Addresses.AddressById(firm.PostalAddressId), CacheKeys.Time.LongTime, () => AddressRepo.Table.FirstOrDefault(a => a.AddressId == firm.PostalAddressId));

			return new FirmModel()
			{
				AiNumber = firm.AiNumber,
				BusinessName = firm.BusinessName,
				Email = firm.Email,
				Fax = firm.Fax,
				Phone = firm.Phone,
				ReferenceNumber = firm.ReferenceNumber,
				TradeName = firm.TradeName,
				PhysicalAddress1 = physicalAddress?.Address1 ?? "",
				PhysicalAddress2 = physicalAddress?.Address2 ?? "",
				PhysicalAddress3 = physicalAddress?.Address3 ?? "",
				PhysicalAddressCode = physicalAddress?.Code ?? "",
				PhysicalAddressCountry = physicalAddress?.Country ?? "",
				PhysicalAddressRegion = physicalAddress?.RegionName ?? "",
				PhysicalAddressTown = physicalAddress?.Town ?? "",
				PostalAddress1 = postalAddress?.Address1 ?? "",
				PostalAddress2 = postalAddress?.Address2 ?? "",
				PostalAddress3 = postalAddress?.Address3 ?? "",
				PostalAddressCode = postalAddress?.Code ?? "",
				PostalAddressCountry = postalAddress?.Country ?? "",
				PostalAddressRegion = postalAddress?.RegionName ?? "",
				PostalAddressTown = postalAddress?.Town ?? ""
			};
		}

		#endregion
	}
}

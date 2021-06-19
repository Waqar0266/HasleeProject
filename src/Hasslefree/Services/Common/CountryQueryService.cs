using System;
using System.Collections.Generic;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System.Linq;
using Hasslefree.Services.Common.Countries;

namespace Hasslefree.Services.Common
{
	public class CountryQueryService : ICountryQueryService
	{
		#region Private Properties
	
		private ICacheManager CacheManager { get; }
		private IDataRepository<Country> CountryRepo { get; }

		#endregion
		
		#region Constructor

		public CountryQueryService(
			ICacheManager cacheManager,
			IDataRepository<Country> countryRepo
		)
		{
			CacheManager = cacheManager;
			CountryRepo = countryRepo;
		}

		#endregion

		#region ICountryQueryService

		/// <summary>
		/// Sets the store id
		/// </summary>
		/// <param name="storeId"></param>
		/// <returns></returns>
		public ICountryQueryService WithStoreId(int storeId)
		{
			return this;
		}

		/// <summary>
		/// Get the countries that the store supports
		/// </summary>
		/// <returns></returns>
		public List<CountryModel> Get()
		{
			return CacheManager.Get(CacheKeys.Store.Country.List(0), CacheKeys.Time.LongTime, () =>
			{
				var countryIds = GetStoreCountryIds();
				return CountryRepo.Table
					.Where(c => countryIds.Contains(c.CountryId))
					.Select(c => new CountryModel
					{
						CountryId = c.CountryId,
						Name = c.Name,
						TwoLetterIsoCode = c.TwoLetterIsoCode,
						ThreeLetterIsoCode = c.ThreeLetterIsoCode,
						NumericIsoCode = c.NumericIsoCode
					}).ToList();
			});
		}

		#endregion

		#region Private Methods

		private  List<Int32> GetStoreCountryIds()
		{
			var countryIds = new List<int>(0);

			return countryIds.Distinct().ToList();
		}

		#endregion
	}
}

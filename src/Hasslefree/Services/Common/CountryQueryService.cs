using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Common.Countries;
using System.Collections.Generic;
using System.Linq;

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
		/// Get the countries that the store supports
		/// </summary>
		/// <returns></returns>
		public List<CountryModel> Get()
		{
			return CountryRepo.Table
				.Select(c => new CountryModel
				{
					CountryId = c.CountryId,
					Name = c.Name,
					TwoLetterIsoCode = c.TwoLetterIsoCode,
					ThreeLetterIsoCode = c.ThreeLetterIsoCode,
					NumericIsoCode = c.NumericIsoCode
				}).ToList();
		}

		#endregion
	}
}

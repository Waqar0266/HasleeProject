using System;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.Cache;

namespace Hasslefree.Services.Common.Countries
{
	internal class GetCountryService : IGetCountryService
	{
		#region Private Properties

		private IReadOnlyRepository<Country> CountryRepo { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Constructor

		public GetCountryService(
				IReadOnlyRepository<Country> countryRepo,
				ICacheManager cache
			)
		{
			CountryRepo = countryRepo;
			Cache = cache;
		}

		#endregion

		#region IGetCountryService

		/// <inheritdoc />
		public CountryModel this[Int32 id]
		{
			get
			{
				return Cache.Get(CacheKeys.Server.Countries.Country(id), CacheKeys.Time.LongTime, () =>
				{
					return CountryRepo.Table
						.Where(a => a.CountryId == id)
						.Select(a => new CountryModel
						{
							CountryId = a.CountryId,
							Name = a.Name,
							TwoLetterIsoCode = a.TwoLetterIsoCode,
							ThreeLetterIsoCode = a.ThreeLetterIsoCode,
							NumericIsoCode = a.NumericIsoCode
						}).FirstOrDefault();
				});
			}
		}

		#endregion
	}

	public class CountryModel
	{
		public int CountryId { get; set; }
		public string Name { get; set; }
		public string TwoLetterIsoCode { get; set; }
		public string ThreeLetterIsoCode { get; set; }
		public int NumericIsoCode { get; set; }
	}
}

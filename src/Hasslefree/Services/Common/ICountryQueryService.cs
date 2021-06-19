using System.Collections.Generic;
using Hasslefree.Services.Common.Countries;

namespace Hasslefree.Services.Common
{
	public interface ICountryQueryService
	{
		ICountryQueryService WithStoreId(int storeId);
		List<CountryModel> Get();
	}
}

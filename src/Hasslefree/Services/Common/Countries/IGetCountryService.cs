using System;

namespace Hasslefree.Services.Common.Countries
{
	public interface IGetCountryService
	{
		/// <summary>
		/// Get the details of a single country
		/// </summary>
		/// <param name="id">The country identifier</param>
		/// <returns></returns>
		CountryModel this[Int32 id] { get; }
	}
}

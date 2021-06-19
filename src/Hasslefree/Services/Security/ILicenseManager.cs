using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Security
{
	public interface ILicenseManager
	{
		Boolean IsTrial { get; }
		Boolean IsExpired { get; }
		List<String> LicenseFeatures { get; }
		DateTime ExpiryDate { get; }

		Boolean this[String key] { get; }
		Boolean this[String[] keys, Boolean all = false] { get; }
	}
}

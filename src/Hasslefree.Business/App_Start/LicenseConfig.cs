using System;
using System.Text;
using Z.BulkOperations;

namespace Hasslefree.Business
{
	public class LicenseConfig
	{
		#region Constants

		private const String ZzzLicenseName = "NTE5OzEwMC1XYXJwIERldmVsb3BtZW50";
		private const String ZzzLicenseKey = "c206b260-ea10-ec17-fa51-e55cbaa828fc";
		private const String ZzzLicenseName2 = "NTE5OzMwMC1XYXJwIERldmVsb3BtZW50";
		private const String ZzzLicenseKey2 = "22a0611a-7616-707f-d875-e4d4992603c5";

		#endregion

		private static readonly Object LicenseRegisterLocker = new Object();

		public static void Register()
		{
			lock (LicenseRegisterLocker)
			{
				#region ZZZ Projects - Entity Framework Extensions

				Z.EntityFramework.Extensions.LicenseManager.AddLicense(Encoding.UTF8.GetString(Convert.FromBase64String(ZzzLicenseName)), ZzzLicenseKey);
				LicenseManager.AddLicense(Encoding.UTF8.GetString(Convert.FromBase64String(ZzzLicenseName2)), ZzzLicenseKey2);
				if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense(ProviderType.MySql))
					throw new Exception("Invalid License!");

				#endregion
			}
		}
	}
}
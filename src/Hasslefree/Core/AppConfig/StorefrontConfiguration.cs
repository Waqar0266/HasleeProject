using System;
using System.Configuration;

namespace Hasslefree.Core.AppConfig
{
	public class HasslefreeConfiguration : ConfigurationSection
	{
		private static string SectionName = "Hasslefree";

		private static object _getConfigLocker = new object();
		/// <summary>
		/// Returns an instance of thje HasslefreeConfiguration class
		/// </summary>
		public static HasslefreeConfiguration GetConfig()
		{
			lock (_getConfigLocker)
			{
				return (HasslefreeConfiguration)ConfigurationManager.GetSection(SectionName) ?? new HasslefreeConfiguration();
			}
		}
		
		[ConfigurationProperty("country", IsRequired = false)]
		public string Country
		{
			get
			{
				return (string)this["country"];
			}
			set
			{
				this["country"] = value;
			}
		}

		[ConfigurationProperty("currency", IsRequired = false)]
		public string Currency
		{
			get
			{
				return (string)this["currency"];
			}
			set
			{
				this["currency"] = value;
			}
		}

		[ConfigurationProperty("server-tenancy", IsRequired = false)]
		public ServerTenancy ServerTenancy
		{
			get
			{
				return (ServerTenancy)Enum.Parse(typeof(ServerTenancy), this["server-tenancy"].ToString());
			}
			set
			{
				this["server-tenancy"] = value;
			}
		}
	}
}

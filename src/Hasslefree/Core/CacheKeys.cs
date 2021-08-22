using System;
using System.Diagnostics.CodeAnalysis;

namespace Hasslefree.Core
{
	[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
	public static class CacheKeys
	{
		public static class Time
		{
			public static int VeryShortTime => 3;

			public static int ShortTime => 10;
			public static int HalfHour => 30;

			public static int Hour => 60;

			public static int DefaultTime => 120;

			public static int LongTime => 720;
		}

		public static class FrontEnd
		{
			public static class Menu
			{
				public static class MenuItems
				{
					public static string Category(bool authenticated, int categoryId, int personId) => $"/menu/menu-items/category?authenticated={authenticated}&categoryId={categoryId}&personId={personId}";
				}
			}
		}

		public static class Server
		{
			public static class Framework
			{
				public static class Filter
				{
					public static string Session(string reference) => $"/server/framework/filter/session?reference={reference}";
					public static string Roles = $"/server/framework/filter/roles";
				}
			}

			public static class Agents
			{
				public static string Path = "/server/agents/";
				public static string AgentByGuid(Guid guid) => $"{Path}agent-by-guid?guid={guid}";
				public static string AgentByPersonId(int personId) => $"{Path}agent-by-personId?personId={personId}";
			}

			public static class Addresses
			{
				public static string Path = "/server/addresses/";
				public static string AddressById(int id) => $"{Path}address-by-id?id={id}";
			}

			public static class Downloads
			{
				public static string Path = "/server/downloads/";
				public static string DownloadById(int id) => $"{Path}download-by-id?id={id}";
			}

			public static class Firms
			{
				public static string Path = "/server/firms/";
			}

			public static class Countries
			{
				public static string Country(int id) => $"/server/countries/country?id={id}";
			}
		}
	}
}

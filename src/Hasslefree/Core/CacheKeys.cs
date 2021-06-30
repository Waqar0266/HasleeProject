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
				}
			}

			public static class Countries
			{
				public static string Country(int id) => $"/server/countries/country?id={id}";
			}
		}
	}
}

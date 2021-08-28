using System;
using System.Collections.Generic;
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

			public static class Rentals
			{
				public static string Path = "/server/rentals/";
				public static string RentalById(int id) => $"{Path}rental-by-id?id={id}";
				public static string RentalByGuid(string guid) => $"{Path}rental-by-guid?guid={guid}";
				public static string GetLandlords(int rentalId) => $"{Path}rental-landlords?id={rentalId}";
				public static string GetMandate(int rentalId) => $"{Path}rental-mandate?id={rentalId}";
				public static string GetFica(int rentalId) => $"{Path}rental-fica?id={rentalId}";
				public static string GetAgent(int rentalId) => $"{Path}rental-agent?id={rentalId}";
				public static string GetWitness(int rentalId) => $"{Path}rental-witness?id={rentalId}";
				public static string GetAgentPerson(int personId) => $"{Path}agent-person?id={personId}";
				public static string GetLandlordBankAccounts(int rentalId) => $"{Path}landlord-bank-accounts?id={rentalId}";
				public static string GetLandlordAddresses(int rentalLandlordId) => $"{Path}landlord-addresses?id={rentalLandlordId}";
				public static string GetLandlordAddress(int addressId) => $"{Path}landlord-address?id={addressId}";
				public static string GetAgentAddresses(int agent) => $"{Path}agent-addresses?id={agent}";
				public static string GetAgentAddress(int addressId) => $"{Path}agent-address?id={addressId}";
			}

			public static class Countries
			{
				public static string Country(int id) => $"/server/countries/country?id={id}";
			}
		}
	}
}

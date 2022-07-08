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
					public static string BasePath = "/menu/menu-items/category";
					public static string Category(bool authenticated, int categoryId, int personId) => $"{BasePath}?authenticated={authenticated}&categoryId={categoryId}&personId={personId}";
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

			public static class Filter
			{
				public static string Path = "/server/filter/";

				public static string FilterWithPath(string path) => $"{Path}filter-with-path?path={path}";
				public static string Properties(List<int> categoryIds) => $"{Path}properties?ids={String.Join(",", categoryIds)}";
				public static string Properties(int page, int pageSize) => $"{Path}properties?page={page}&pageSize={pageSize}";
				public static string Property(int propertyId) => $"{Path}property?ids={propertyId}";
				public static string BuildingKeyValues(List<int> propertyIds) => $"{Path}building-key-values?ids={String.Join(",", propertyIds)}";
				public static string ExternalFeaturesKeyValues(List<int> propertyIds) => $"{Path}external-features-key-values?ids={String.Join(",", propertyIds)}";
				public static string OtherFeaturesKeyValues(List<int> propertyIds) => $"{Path}other-features-key-values?ids={String.Join(",", propertyIds)}";
				public static string OverviewKeyValues(List<int> propertyIds) => $"{Path}overview-key-values?ids={String.Join(",", propertyIds)}";
				public static string RoomsKeyValues(List<int> propertyIds) => $"{Path}rooms-key-values?ids={String.Join(",", propertyIds)}";
				public static string Images(List<int> propertyIds) => $"{Path}images?id=?ids={String.Join(",", propertyIds)}";
			}

			public static class Rentals
			{
				public static string Path = "/server/rentals/";
				public static string RentalById(int id) => $"{Path}rental-by-id?id={id}";
				public static string RentalByGuid(string guid) => $"{Path}rental-by-guid?guid={guid}";
				public static string GetLandlords(int rentalId) => $"{Path}rental-landlords?id={rentalId}";
				public static string GetMandate(int rentalId) => $"{Path}rental-mandate?id={rentalId}";
				public static string GetResolution(int rentalId) => $"{Path}rental-resolution?id={rentalId}";
				public static string GetFica(int rentalId) => $"{Path}rental-fica?id={rentalId}";
				public static string GetAgent(int rentalId) => $"{Path}rental-agent?id={rentalId}";
				public static string GetWitness(int rentalId) => $"{Path}rental-witness?id={rentalId}";
				public static string GetAgentPerson(int personId) => $"{Path}agent-person?id={personId}";
				public static string GetLandlordBankAccounts(int rentalId) => $"{Path}landlord-bank-accounts?id={rentalId}";
				public static string GetLandlordCommonAddresses(int rentalLandlordId) => $"{Path}landlord-common-addresses?id={rentalLandlordId}";
				public static string GetLandlordAddresses(int rentalLandlordId) => $"{Path}landlord-addresses?id={rentalLandlordId}";
				public static string GetLandlordAddress(int addressId) => $"{Path}landlord-address?id={addressId}";
				public static string GetAgentAddresses(int agent) => $"{Path}agent-addresses?id={agent}";
				public static string GetAgentAddress(int addressId) => $"{Path}agent-address?id={addressId}";
				public static string GetLandlordDocumentation(int rentalLandlordId) => $"{Path}landlord-documentation?id={rentalLandlordId}";
				public static string GetForms(int rentalId) => $"{Path}forms?id={rentalId}";
			}

			public static class RentalTs
			{
				public static string Path = "/server/rentalts/";
				public static string RentalTById(int id) => $"{Path}rentalt-by-id?id={id}";
				public static string RentalTByGuid(string guid) => $"{Path}rentalt-by-guid?guid={guid}";
			}

			public static class ExistingRentals
			{
				public static string Path = "/server/existing-rentals/";
				public static string ExistingRentalById(int id) => $"{Path}rental-by-id?id={id}";
				public static string ExistingRentalByGuid(string guid) => $"{Path}rental-by-guid?guid={guid}";
				public static string GetForms(int existingRentalId) => $"{Path}forms?id={existingRentalId}";
			}

			public static class Sales
			{
				public static string Path = "/server/sales/";
				public static string SaleById(int id) => $"{Path}sale-by-id?id={id}";
				public static string SaleByGuid(string guid) => $"{Path}sale-by-guid?guid={guid}";
				public static string GetSellers(int saleId) => $"{Path}sellers?id={saleId}";
				public static string GetAgent(int saleId) => $"{Path}sale-agent?id={saleId}";
				public static string GetWitness(int saleId) => $"{Path}sale-witness?id={saleId}";
				public static string GetAgentPerson(int personId) => $"{Path}agent-person?id={personId}";
				public static string GetAgentAddresses(int agentId) => $"{Path}agent-addresses?agentId={agentId}";
				public static string GetAgentAddress(int addressId) => $"{Path}agent-address?addressId={addressId}";
			}

			public static class Countries
			{
				public static string Country(int id) => $"/server/countries/country?id={id}";
			}
		}
	}
}

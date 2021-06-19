using System;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
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

			public static int Hour => 60;

			public static int DefaultTime => 120;

			public static int LongTime => 720;
		}

		public static class CustomResources
		{
			public static class Scripts
			{
				public static string Pattern => "/custom-resources/scripts/";

				public static string All => "/custom-resources/scripts/all";

				public static string Filtered(string url) => $"/custom-resources/scripts/filtered?url={url}";
			}

			public static class Stylesheets
			{
				public static string Pattern => "/custom-resources/stylesheets/";

				public static string All => "/custom-resources/stylesheets/all";

				public static string Filtered(string url) => $"/custom-resources/stylesheets/filtered?url={url}";
			}
		}

		public static class Store
		{
			public static string Logo => "/store/logo";

			public static string WithHostname(string host) => $"/store?host={host}";

			public static string DisplayPricing(bool isLoggedIn) => $"/store/display-pricing?loggedIn={isLoggedIn}";

			public static class Banks
			{
				public static string General => "/store/banks";
			}

			public static class Emails
			{
				public static String General => "/store/emails";
				public static string StoreModel => $"{General}/store-model";

				public static string SenderEmail(string type) => $"{General}/sender-email?type={type}";

				public static string OrderModel(String type, Int32 documentId) => $"{General}/order-model?type={type}&documentId={documentId}";
			}

			public static class GoogleMaps
			{
				public static string ApiKey() => $"/store/googlemaps/apikey";
			}

			public static class Account
			{
				public static class Login
				{
					public static string IsSystemAdmin(int storeId, int loginId) => $"/store/{storeId}/account/login/{loginId}/issystemadmin";

					public static string Permissions(int storeId, int loginId) => $"/store/{storeId}/account/login/{loginId}/permissions";

					public static string SecurityGroups(int storeId, int loginId) => $"/store/{storeId}/account/login/{loginId}/securitygroups";
				}

				public static class Member
				{
					public static string IsOwner(int accountId, int personId) => $"/store/account/{accountId}/member/{personId}/isowner";

					public static string IsOwnerOrManager(int accountId, int personId) => $"/store/account/{accountId}/member/{personId}/isownerormanager";
				}

				public static class Order
				{
					public static string OrderModel => "Store.Models.Shared.OrderModel({0})";

					public static string IsOwner(int documentId, int accountId) => $"/store/account/order/is-owner?documentId={documentId}&accountId={accountId}";

					public static string ReOrder(int documentId) => $"/store/account/order/re-order?documentId={documentId}";
				}

				public static class Person
				{
					public static string All(int storeId, int page, int pageSize) => $"/store/{storeId}/account/people/{pageSize}/{page}";

					public static string ByEmail(int storeId, string email) => $"/store/{storeId}/account/person/{email}/";

					public static string ById(int storeId, int personId) => $"/store/{storeId}/account/person/{personId}/";

					public static string ByPricelistId(int storeId, int pricelistId) => $"/store/{storeId}/account/person/pricelist/{pricelistId}";

					public static string ByTerm(int storeId, string term) => $"/store/{storeId}/account/person/{term}/";

					public static string Pattern(int storeId) => $"/store/{storeId}/account/person";
				}

				public static class Registries
				{
					public static string GiftRegistries => "/store/account/gift-registries";

					public static string Wishlists(int storeId, int accountId) => $"/store/account/wishlists/{storeId}/{accountId}";
					public static string WishlistsDictionary(int storeId, int accountId) => $"/store/account/wishlistsdictionary/{storeId}/{accountId}";
					public static String Wishlist(Int32 documentId) => $"/store/account/wishlist?documentId={documentId}";
				}
			}

			public static class Catalog
			{
				public static string Products => "/store/catalog/products";

				public static string RelatedProducts => "/store/catalog/products/{0}/related?page={1}&pageSize={2}";

				public static string ProductSegments => "/store/catalog/product-segments";

				public static string Segments => "/store/catalog/segments";

				public static string SegmentValues => "/store/catalog/segments-values";

			}

			public static class Checkout
			{
				public static class Details
				{
					public static string CollectionPoints(int storeId, int addressId) => $"/store/{storeId}/checkout/details/collection-point/{addressId}";

					public static class Model
					{
						public static string Pattern => "/store/checkout/details/model";

						public static string BasketViewModel(int documentId) =>
							$"{Pattern}/basket-view-model?docmentId={documentId}";
						public static string BasketViewModel(string documentRef) => $"/store/checkout/detais/model/{documentRef}/basketviewmodel";

						public static string Errors(int documentId) => $"/store/checkout/details/model/{documentId}/errors";
					}
				}

				public static class Confirmation
				{
					public static string ProductCategory(List<int> products) => $"/store/checkout/confirmation/product-category?productIds={string.Join(",", products)}";
					public static string ProductManufacture(List<int> products) => $"/store/checkout/confirmation/product-manufacturer?productIds={string.Join(",", products)}";
					public static string ProductsVariant(List<int> products) => $"/store/checkout/confirmation/product-variants?productIds={string.Join(",", products)}";
					public static string ProductsVoucherUse(List<int> documentLineIds) => $"/store/checkout/confirmation/product-voucher-uses?productIds={string.Join(",", documentLineIds)}";
				}

				public static string OrderPrefix => "/store/checkout/create-order/settings/order-prefix";

				public static string OrderLength => "/store/checkout/create-order/settings/order-length";

				public static class Click2Order
				{
					public static string ProductConfirmC2o(int productId) => $"/cto/product-confirm-c2o/{productId}";
					public static string ProductConfirmC2o(string sku) => $"/cto/product-confirm-c2o/{sku}";
					public static string Description(int productId) => $"/cto/product-c20-description/{productId}";
					public static string Pictures(int productId) => $"/cto/product-c20-pictures/{productId}";
				}
			}

			public static class Content
			{
				public static string Page(string key, bool isLoggedIn) => $"/store/content/page?key={key}&loggedIn={isLoggedIn}";
				public static string ZoneItemContent() => $"/store/content/zoneitem-content";
				public static string ZoneItem(bool isLoggedIn, string pattern, string zone) => $"/store/content/zoneitem({isLoggedIn},{pattern},{zone})";
			}

			public static class Coupon
            {
				public static string Generate(string vref) => $"coupons/generate?vref={vref}";
			}

			public static class Criteria
			{
				public static string ProductsWithMasterProducts => "/store/criteria/products-with-master-products";
			}

			public static class Discounts
			{
				public static string Promotions => "/discounts";

				public static string PromotionCriteria(int discountId) => $"{Promotions}/criteria?discountId={discountId}";
			}

			public static class Factories
			{
				public static string Products = "/store/factories/products";

				public static class Product
				{
					public static string Skus = "/store/factories/products/skus";

					public static string Categories = "/store/factories/products/categories";

					public static string Manufacturers = "/store/factories/products/manufacturers";

					public static string Currencies = "/store/factories/products/currencies";

					public static string Descriptions = "/store/factories/products/descriptions";

					public static class MasterProduct
					{
						public static string DefaultPictures = "/store/factories/products/master-product/default-pictures";
					}
				}

				public static class MiniProduct
				{
					public static string ProductSkus => "/store/factories/mini-product/product-skus";
					public static string Products => "/store/factories/mini-product/products";
				}

				public static class MiniCategory
				{
					public static string CategoryNames => "/store/factories/mini-category/category-names";
					public static string Categories => "/store/factories/mini-category/categories";
				}

				public static class MiniLabel
				{
					public static string LabelNames => "/store/factories/mini-label/label-names";
					public static string Labels => "/store/factories/mini-label/labels";
				}

				public static class MiniManufacturer
				{
					public static string ManufacturerNames => "/store/factories/mini-manufacturer/manufacturer-names";
					public static string Manufacturers => "/store/factories/mini-manufacturer/manufacturers";
				}

				public static class PredictiveSearch
				{
					public static string Sort => "/store/factories/predictive-search/sort";
					public static string Search(string term) => $"/store/factories/predictive-search/search?term={term}";
				}

				public static class HamperProduct
				{
					public static string Currencies = "/store/factories/hamper-product/products/currencies";
					public static string HamperProductsIds => "/store/factories/hamper-product/hamper-products/ids";
					public static string HamperProductsSkus => "/store/factories/hamper-product/hamper-products/skus";
					public static string HamperItems => "/store/factories/hamper-product/hamper-items";
				}

				public static class ConfigurationProduct
				{
					public static string Currencies = "/store/factories/configuration-product/products/currencies";
					public static string ProductIds => "/store/factories/configuration-product/products/ids";
					public static string ProductSkus => "/store/factories/configuration-product/products/skus";
					public static string Positions => "/store/factories/configuration-product/positions";
					public static string PositionItems => $"/store/factories/configuration-product/position/items";
				}

				public static class ProductSet
				{
					public static string Currencies = "/store/factories/product-set/products/currencies";
					public static string ProductIds => "/store/factories/product-set/products/ids";
					public static string ProductSkus => "/store/factories/product-set/products/skus";
					public static string Positions => "/store/factories/product-set/positions";
					public static string PositionItems => "/store/factories/product-set/positions/items";
				}

				public static class Filter
				{
					public static string Products => "/store/factories/filter/products";
				}
			}

			public static class Filter
			{
				public static string Published => "/store/filter/published";

				public static string Products => "/store/filter/products";

				public static string CategoryPathLookup => $"/store/filter/product/category-lookup";
				public static string FilterExtension(string name, string qs) => $"/store/filter/{name}/products?{qs}";
				public static string FilterAvailable(string name, string key) => $"/store/filter/{name}/products?key={key}";

				public static class Splash
				{
					public static string TopLevelCategories => "/store/filter/splash/top-level-categories";
					public static string Categories(int personId) => $"/store/filter/splash/categories?personId={personId}";
				}

				public static class Categories
				{
					public static string Products(int categoryId) => $"/store/filter/categories/{categoryId}/products";
					public static string Selected => "/store/filter/categories/selected";
					public static string Available => "/store/filter/categories/available";
					public static string ProductCounts => "/store/filter/categories/productcounts";
				}

				public static class Manufacturers
				{
					public static string Products(int brandId) => $"/store/filter/manufacturers/{brandId}/products";
					public static string Selected => "/store/filter/manufacturers/selected";
					public static string Available => "/store/filter/manufacturers/available";
				}

				public static class Attributes
				{
					public static string Products(int attributeId) => $"/store/filter/attributes/{attributeId}/products";
					public static string Selected => "/store/filter/attributes/selected";
					public static string Available => "/store/filter/attributes/available";
				}

				public static class Labels
				{
					public static string Products(int labelId) => $"/store/filter/labels/{labelId}/products";
					public static string Selected => "/store/filter/labels/selected";
					public static string Available => "/store/filter/labels/available";
				}

				public static class Segments
				{
					public static string MasterProductSegments => "/store/filter/segments/master-product-segments";

					public static string Products(string segment, string segmentValue) => $"/store/filter/segments/{segment}_{segmentValue}/products";
					public static string Selected => "/store/filter/segments/selected";
					public static string Available => "/store/filter/segments/available";
				}

				public static class Pricing
				{
					public const string Prices = "/store/filter/pricing/prices";
					public const string Settings = "/store/filter/pricing/settings";
				}

				public static string Search(string term) => $"/store/filter/search?term={term}";
				public static string Sort => "/store/filter/sort";
				public static string SortProduct(int id) => $"/store/filter/sort?id={id}";
			}

			public static class Menu
			{
				public static class MenuItems
				{
					public static string Category(bool authenticated, int categoryId, int personId) => $"/store/menu/menu-items/category?authenticated={authenticated}&categoryId={categoryId}&personId={personId}";

					public static string Manufacturer(bool authenticated, int manufacturerId, int personId) => $"/store/menu/menu-items/manufacturer?authenticated={authenticated}&categoryId={manufacturerId}&personId={personId}";

					public static string Picture(int pictureId, EntityType type, PictureSize size) => $"/store/menu/menu-items/picture?id={pictureId}&type={type}&size={size}";

					public static class RenderMenu
					{
						public static string NavigationItems(string type, bool isLoggedIn) => $"/store/menu/menu-items/render-menu/navigation-items?type={type}&loggedIn={isLoggedIn}";
						public static string Content => $"/store/menu/menu-items/render-menu/content";
						public static string SocialMedia => $"/store/menu/menu-items/render-menu/social-media";
					}
				}
			}

			public static class MultiBuy
			{
				public static string Promotions(int storeId) => $"/store/{storeId}/multi-buy/promotions";

				public static string PromotionCriteria(int storeId, int multiBuyId) => $"/store/{storeId}/multi-buy/{multiBuyId}/criteria";
			}

			public static class Product
			{
				public static class Actions
				{
					public static string GetProductActions => "/store/product/actions/get-product-actions";
				}

				public static string Details(int id) => $"/store/product/details?id={id}";
			}

			public static class Seo
			{
				public static string Category => "/store/dictionaries/seo/category";
				public static string Product => "/store/dictionaries/seo/product";
				public static string Brand => "/store/dictionaries/seo/brand";
				public static string GetSeoProducts => "/store/dictionaries/seo/seo-products";
			}

			public static class Session
			{
				public static string PatternSession(string sessionRef) => $"/sessions/{sessionRef}";

				public static string Login(string sessionRef) => $"/sessions/{sessionRef}/login";

				public static string Account(string sessionRef) => $"/sessions/{sessionRef}/account";

				public static string Cart(string sessionRef) => $"/sessions/{sessionRef}/cart";

				public static string Currency(string sessionRef) => $"/sessions/{sessionRef}/currency";
			}

			public static class Branding
			{
				public static class Pantone
				{
					public static string GetColours => "/store/branding/pantone/get-colours";
					public static string GetSortedColours => "/store/branding/pantone/get-sorted-colours";
					public static string GetColoursByName(int pantoneId) => $"/store/branding/pantone/get-colours/by-name?pantoneId={pantoneId}";
					public static string GetColoursByThread(int pantoneId) => $"/store/branding/pantone/get-colours/by-thread?pantoneId={pantoneId}";
					public static string GetColoursById(int pantoneId) => $"/store/branding/pantone/get-colours?pantoneId={pantoneId}";
				}

				public static string Enabled => "/store/branding/enabled";
				public static string ProductHasBranding(int productId) => $"/store/branding/product-has-branding?productId={productId}";
				public static string ProductHasBranding(string sku) => $"/store/branding/product-has-branding?sku={sku}";

				public static string AllProcesses => "/store/branding/all-processes";

				public static string DocumentLineOptions(int documentLineId) => $"/store/branding/document-line-options?documentLineId={documentLineId}";
				public static string DocumentLineArtworks(int documentLineId) => $"/store/branding/document-line-artworks?documentLineId={documentLineId}";

				public static string GetCosts(int processId, string type, string identifier = null, bool lookForGreater = false) => $"/store/branding/get-costs?processId={processId}&type={type}&identifier={identifier}&lookForGreater={lookForGreater}";
				public static string ProductOptions(int id) => $"/store/branding/product-options?id={id}";

				public static string IsEnquiry(int processId) => $"/store/branding/is-enquiry?processId={processId}";

				public static class Order
				{
					public static string Layouts(int productId) => $"/store/branding/order/layouts?productId={productId}";
				}
			}

			public static class Themes
			{
				public static string Pattern = "/store/themes/";
				public static string Scripts(int themeId) => $"/store/themes/scripts?{themeId}";
			}

			public static class ProductReview
			{
				public static string Pattern(int productId) => $"/store/product-reviews/{productId}";
				public static string Product(int productId) => $"{Pattern(productId)}/product";
				public static string MayReview(int productId, int accountId) => $"{Pattern(productId)}/may-review?accountId={accountId}";
				public static string HasReviewed(int productId, int accountId) => $"{Pattern(productId)}/has-reviewed?accountId={accountId}";
				public static string ProductReviews(int productId, decimal minRating) => $"{Pattern(productId)}/reviews?min-rating={minRating}";
				public static string ProductReviewEmail(int reviewId) => $"/store/product-reviews/{reviewId}/email";
			}

			public static class Announcements
			{
				public static string ActiveAnnouncements = "/store/announcements";
			}

			public static class Messages
			{
				public static string Pattern => "/store/messages";
				public static string PersonPattern(int personId) => $"{Pattern}/{personId}";
				public static string UserMessages(int personId, string status) => $"{PersonPattern(personId)}?status={status}";
			}

			public static class Country
			{
				private static string Pattern => "/store/countries";
				public static string List(int storeId) => $"{Pattern}/list?storeId={storeId}";
			}

			public static class Address
			{
				public static string Pattern => "/store/address";
				public static string ShippingList(bool usingSession, int documentId, bool isLoggedIn, int accountId, int personId) => $"{Pattern}/shipping-list?usingSession={usingSession}&documentId={documentId}&isLoggedIn={isLoggedIn}&accountId={accountId}&personId={personId}";
				public static string Get(int addressId) => $"{Pattern}/get?addressId={addressId}";

				public static string Product(int productId) => $"{Pattern}/product?id={productId}";
			}

			public static class Newsletter
			{
				private static string Pattern => "/store/newsletter/contact";
				public static string Contact(int id) => $"{Pattern}?id={id}";
				public static string Contact(string email) => $"{Pattern}?email={email}";
			}
		}

		public static class BackOffice
		{
			public static class Api
			{
				public static string Admin => "/backoffice/api/admin";

				public static string Central => "/backoffice/api/central";
			}

			public static class Dashboard
			{
				public static string Widgets => "/backoffice/dashboard/widgets";
			}

			public static class Catalog
			{
				public static class Suppliers
				{
					public static string SubscriberNumber(int supplierId) => $"/backoffice/catalog/suppliers/manage-subscriptions?supplierId={supplierId}";
					public static string Categories() => "/backoffice/catalog/suppliers/manage-subscriptions/categories";
				}

				public static class Products
				{
					public static string Exists(int productId) => $"/backoffice/catalog/product/exists?productId={productId}";

					public static class Variants
					{
						public static string CachePattern => "/backoffice/catalog/variants";
						public static string VariantBySku(string sku) => $"{CachePattern}?sku={sku}";
					}
				}

				public static class Subscribers
				{
					public static string SubscriberExists(string subscriberNumber) => $"/backoffice/catalog/subscribers/subscriber-exists?subscriberNumber={subscriberNumber}";
				}
			}

			public static class Reviews
			{
				public static class ProductReviews
				{
					public static string Pattern => "/backoffice/reviews/product-reviews";
					public static string Reviews(string term) => $"{Pattern}?term={term}";
				}
			}

			public static class MultiStore
			{
				public static string ChannelDomain(string channel) => $"/backoffice/multi-store/get-channel-domain?channel={channel}";
				public static string HostnameAvailable(string hostname) => $"/backoffice/multi-store/hostname-available?hostname={hostname}";
			}

			public static class Themes
			{
				public static string CentralThemes => "/backoffice/themes/central-themes";
				public static string StoreThemesPattern => "/backoffice/themes/store-theme";
				public static string StoreThemes(int page, int pageSize) => $"/backoffice/themes/store-themes?page={page}&pageSize={pageSize}";
				public static string StoreTheme(int themeId) => $"/backoffice/themes/store-theme?themeId={themeId}";
			}

			public static class MediaLibrary
			{
				public static string Pattern => "/backoffice/media-library";
				public static string FullMediaFolderList => $"{Pattern}/folders";
				public static string FullMediaFileList => $"{Pattern}/files";
				public static string MediaFolderPath(int mediaFolderId) => $"{Pattern}/folders/path?mediaFolderId={mediaFolderId}";
				public static string FolderLibrary(int? mediaFolderId, string mediaFolderPath, string search) => $"{Pattern}?mediaFolderId={mediaFolderId}&mediaFolderPath={mediaFolderPath}&search={search}";
			}

			public static class Sales
			{
				public static class Enquiries
				{
					public static string Accounts => "/backoffice/sales/enquiries/accounts";
					public static string AccountPeople(int accountId) => $"/backoffice/sales/enquiries/people?accountId={accountId}";

					public static string Country(String name) => $"/backoffice/sales/enquiries/recalculate/country?name={name}";
				}
			}

			public static class DataTransfer
			{
				private const string General = "/backoffice/data-transfer";

				public static class Pictures
				{
					private const string General = DataTransfer.General + "/pictures";

					public const string ProductIds = General + "/product-ids";
				}
			}
		}

		public static class Server
		{
			public const string Pattern = "/server";
			public static class Store
			{
				public static string OwnerEmail(int storeId) => $"/server/store/owner-email?storeId={storeId}";
			}

			public static class Database
			{
				public static string Schemas => "/server/database/schemas";
			}

			public static class Framework
			{
				public static string Extensions => "/server/framework/extensions";
				public static string ExtensionTypes => "/server/framework/extension-types";

				public static class Lookups
				{
					public static class People
					{
						public static string AllCustomerSegments => "/server/framework/lookups/people/all-customer-segments";
						public static string PersonCustomerSegments => "/server/framework/lookups/people/person-customer-segments";
						public static string Pricelists => "/server/framework/lookups/people/pricelists";
						public static string Products => "/server/framework/lookups/people/products";
						public static string Brands => "/server/framework/lookups/people/brands";
						public static string Categories => "/server/framework/lookups/people/categories";

						public static string GlobalPricelists => "/server/framework/lookups/people/global-pricelists";
						public static string GlobalPricelistCategories => "/server/framework/lookups/people/global-pricelist-categories";
					}

					public static class CustomerSegments
					{
						public static string People => "/server/framework/lookups/customer-segments/people";
						public static string Pricelists => "/server/framework/lookups/customer-segments/pricelists";
						public static string Products => "/server/framework/lookups/customer-segments/products";
						public static string Brands => "/server/framework/lookups/customer-segments/brands";
						public static string Categories => "/server/framework/lookups/customer-segments/categories";
					}

					public static class Badge
					{
						public static string ProductsWithBadges => "/server/framework/lookups/badges/products-with-badges";
						public static string ProductBadges => "/server/framework/lookups/badge/product-badges";
						public static string Badges => "/server/framework/lookups/badge/badges";
					}
				}

				public static class Filter
				{
					public static string OnActionExecuting => "/server/framework/filter/on-action-executing";
					public static string OnActionExecuted => "/server/framework/filter/on-action-executed";

					public static string HostNames => "/server/framework/filter/host-name-filter/hosts";

					public static string Session(string reference) => $"/server/framework/filter/session?reference={reference}";
				}

				public static class Cors
				{
					public static string HostNames => "/server/framework/cors/host-names";
				}
			}

			public static class Domains
			{
				public static string IsBuiltIn() => $"/server/domains/is-built-in";
			}

			public static class Filter
			{
				public static class MultiStore
				{
					public static string Stores(int personId) => "/server/filter/multi-store/stores?personId=" + personId;
					public static string PersonStores() => "/server/filter/multi-store/person-stores";
					public static string PersonStores(int personId) => $"/server/filter/multi-store/person-stores?personId={personId}";
				}
			}

			public static class Catalog
			{
				public static class Product
				{
					public const string Pattern = "/server/catalog/product";

					public static string Enquiries => $"{Pattern}/enquiries";

					public static string ProductSku(string sku) => $"{Pattern}/product-sku?sku={sku}";
				}

				public static class Inventory
				{
					public static string ProductSkus => "/server/catalog/inventory/product-skus";
					public static string StockCount(int productId) => $"/server/catalog/inventory/stock-count?productId={productId}";

					public static class Dapper
					{
						public const string Pattern = "/server/catalog/inventory/dapper";

						public static string ProductSku(string sku) => $"{Pattern}/product-sku?sku={sku}";
						public static string Stock(int id) => $"{Pattern}/stock?productId={id}";
					}
				}
			}

			public static class Template
			{
				public static string This => "/server/template";
				public static string Theme(int storeId) => $"/server/template/theme/{storeId}";
				public static string ParentTheme(int storeId) => $"/server/template/parent-theme/{storeId}";
				//public static string Css(int storeId) => $"/server/template/css/{storeId}";
				//public static string Js(int storeId) => $"/server/template/js/{storeId}";
				public static string TemplateManifest(int storeId) => $"/server/template/template-manifest/{storeId}";
				public static string Settings(int storeId) => $"/server/template/settings/{storeId}";
			}

			public static class Seo
			{
				public static class Canonical
				{
					public static string RequestHost => "/server/seo/canonical/request-host";
					public static string SitemapUrls => "/server/seo/canonical/sitemap-urls";
				}
			}

			public static class SitemapWithActions
			{
				public static string SitemapUrlsWithAction => "/server/sitemap/sitemap-urls-with-actions";
			}

			public static class Handler
			{
				public static string Store(string request) => $"/server/handler/store?request={request}";

				public static string StoreHostName => $"/server/handler/store-host-name";

				public static string SitemapXml => $"/server/handler/sitemap-xml";
			}

			public static class Currency
			{
				public static string Get(string code) => $"/server/currency/get?code={code}";
				public static string Get(int id) => $"/server/currency/get?id={id}";
				public static string ExchangeRates(string baseC) => $"/server/currency/exchange-rates?base={baseC}";
			}

			public static class Branding
			{
				public static class Manager
				{
					public static string Processes => "/server/branding/manager/processes";
				}
			}

			public static class Pricing
			{
				public const string Pattern = Server.Pattern + "/pricing";

				public static class Pricelists
				{
					public static string All(int storeId) => $"/server/pricing/pricelists/all/{storeId}";

					public static string Account(int storeId, int accountId) => $"/server/pricing/pricelists/account?storeId={storeId}&accountId={accountId}";
				}

				public static class Dapper
				{
					public const string Pattern = Pricing.Pattern + "/dapper";

					public static string QuoteProduct(Int32 id) => $"{Pattern}/quote-product?id={id}";
					public static string Variants(Int32 id) => $"{Pattern}/variants?id={id}";

					public static string Settings => $"{Pattern}/settings";
					public static string DefaultPricelist => $"{Pattern}/default-pricelist";

					public static string QuotePrice(Int32 pricelistId, Int32 productId) => $"{Pattern}/quote-price?pricelist-id={pricelistId}&product-id={productId}";

					public static class Filter
					{
						public const string Pattern = Dapper.Pattern + "/filter";

						public static string QuotePrice(Int32 pricelistId, Int32 productId) => $"{Pattern}/quote-price?pricelist-id={pricelistId}&product-id={productId}";
						public static string Settings => $"{Pattern}/settings";
					}
				}
			}

			public static class Cart
			{
				public static string ConfigurationProductPositions => "/server/cart/configuration-product-positions";
				public static string HamperProductItems => "/server/cart/hamper-product-items";
				public static string ProductSetPositions => "/server/cart/product-set-positions";
			}

			public static class Managers
			{
				public static string Sitemap => "/server/managers/sitemap";
				public static string DirtySitemap => "/server/managers/dirty-sitemap";
			}

			public static class Webhooks
			{
				public static string Pattern => "/server/webhooks/";
				public static string QueueUrl => $"{Pattern}queue-url";
				public static string Subscribers(string eve) => $"{Pattern}subscribers?event={eve}";

				public static string Subscriber(int id) => $"{Pattern}subscriber?id={id}";
			}

			public static class Documents
			{
				public static string Person(int documentId) => $"/server/documents/person?documentId={documentId}";
			}

			public static class Countries
			{
				public static string Country(int id) => $"/server/countries/country?id={id}";
			}

			public static class Events
			{
				public static class SetupProgress
				{
					public const string Pattern = "/server/setup-progress";
				}
			}
		}

		public static class Mappings
		{
			public static string ClearPattern = "/mappings/";
			public static class Product
			{
				public static string ClearPattern = "/mappings/product/";
				public static class FromProductSku
				{
					public static string ClearPattern = "/mappings/product/from_product_sku/";
					public static class ToProductId
					{
						public static string Key(string sku)
						{
							return $"mappings/product/from_product_sku/to_product_id({sku})";
						}

						public static string ClearPattern()
						{
							return $"mappings/product/from_product_sku/to_product_id";
						}
						public static string ClearPattern(string sku)
						{
							return $"mappings/product/from_product_sku/to_product_id({sku})";
						}
					}
				}
			}
		}
		
		public static class Models
		{
			internal static string BaseKey = "/models";
			public static string ClearPattern = BaseKey;

			public static class Catalog
			{
				internal static string BaseKey = $"{Models.BaseKey}/catalog";
				public static string ClearPattern = BaseKey;

				#region ProductModel
				public static class ProductModel
				{
					internal static string BaseKey = $"{Models.Catalog.BaseKey}/product_model";
					public static string Key(int productId)
					{
						return $"{BaseKey}({productId})";
					}

					public static string ClearPattern()
					{
						return BaseKey;
					}
					public static string ClearPattern(int productId)
					{
						return $"{BaseKey}({productId}";
					}
				}
				#endregion

				#region FilterProductModel

				public static class FilterProductModel
				{
					internal static string BaseKey = $"{Models.Catalog.BaseKey}/filter_product_model";
					public static string Key(int productId)
					{
						return $"{BaseKey}({productId})";
					}

					public static string ClearPattern()
					{
						return BaseKey;
					}
					public static string ClearPattern(int productId)
					{
						return $"{BaseKey}({productId}";
					}
				}
				#endregion

				#region ProductManufacturerModel

				public static class ProductManufacturerModel
				{
					internal static string BaseKey = $"{Models.Catalog.BaseKey}/product_manufacturer_model";
					public static string Key(int manufacturerId)
					{
						return $"{BaseKey}({manufacturerId})";
					}

					public static string ClearPattern()
					{
						return BaseKey;
					}

					public static string ClearPattern(int manufacturerId)
					{
						return $"{BaseKey}({manufacturerId}";
					}
				}
				#endregion

				#region ProductSegmentModel

				public static class ProductSegmentModel
				{
					internal static string BaseKey = $"{Models.Catalog.BaseKey}/product_segment_model";
					public static string Key(int segmentationId)
					{
						return $"{BaseKey}({segmentationId})";
					}

					public static string ClearPattern()
					{
						return BaseKey;
					}

					public static string ClearPattern(int segmentationId)
					{
						return $"{BaseKey}({segmentationId}";
					}
				}
				#endregion

				#region CategoryModel
				public static class CategoryModel
				{
					internal static string BaseKey = $"{Models.Catalog.BaseKey}/category_model";
					public static string Key(int categoryId)
					{
						return $"{BaseKey}({categoryId})";
					}

					public static string ClearPattern()
					{
						return BaseKey;
					}

					public static string ClearPattern(int categoryId)
					{
						return $"{BaseKey}({categoryId}";
					}
				}
				#endregion
			}
		}

		public static class Api
		{
			public const string Pattern = "/api";

			public static class Ajax
			{
				public const string Pattern = Api.Pattern + "/ajax";

				public static class Pricing
				{
					public const String Pattern = Ajax.Pattern + "/pricing";

					public static String GetProductIds(String sku) => $"{Pattern}/get-product-ids?sku={sku}";
				}

				public static class Stock
				{
					public const String Pattern = Ajax.Pattern + "/stock";

					public static String GetVariantIds(String sku) => $"{Pattern}/get-variant-ids?sku={sku}";
				}

				public static class Wishlist
				{
					public const String Pattern = Ajax.Pattern + "/wishlist";

					public static String GetDocument(String @ref) => $"{Pattern}/get-document?documentRef={@ref}";
					public static String GetDocuments(Int32 accountId) => $"{Pattern}/get-documents?accountId={accountId}";
				}
			}
		}
	}
}

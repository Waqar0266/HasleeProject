using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Listings
{
	public class PropertyInfoPage
	{
		private readonly HtmlDocument _document;

		public PropertyInfoPage(HtmlWeb web, string propertyId)
		{
			_document = web.Load($"https://www.property24.com/for-sale/search?SearchText={propertyId}");
		}

		public List<string> GetImages()
		{
			var imageNodes = _document.DocumentNode.SelectNodes("//a[@class='js_lightboxImageSrc']");
			return imageNodes.Select(x => x.Attributes["data-lightbox-src"].Value).ToList();
		}

		public string GetProvince()
		{
			return InnerText("div[@id='breadCrumbContainer']/ul/li", 5).Replace("\r\n", "");
		}

		public string GetCity()
		{
			return InnerText("div[@id='breadCrumbContainer']/ul/li", 7).Replace("\r\n", "");
		}

		public string GetSuburb()
		{
			return InnerText("div[@id='breadCrumbContainer']/ul/li", 9).Replace("\r\n", "");
		}

		public decimal GetPrice()
		{
			var price = InnerText("div[@class='p24_price']", 1).Replace("\r\n", "").ToLower().Replace("r", "").Replace(" ", "");
			return decimal.Parse(price);
		}

		public string GetName()
		{
			return InnerText("div[@class='p24_mBM']", 1);
		}

		public string GetDescription()
		{
			return InnerHtml("div[@class='js_readMoreText p24_readMoreText']");
		}

		public string GetAddress()
		{
			var nodes = _document.DocumentNode.SelectNodes("//div[contains(@class, \"p24_mB\")]");
			return nodes[4].InnerText.Replace("\r\n","").Trim();
		}

		public Dictionary<string, string> GetOverviewKeyValues()
		{
			var result = new Dictionary<string, string>();
			var container = _document.DocumentNode.SelectSingleNode("//div[@id='js_accordion_propertyoverview']");
			if (container == null) return new Dictionary<string, string>();
			var items = container.SelectNodes(".//div[@class='row p24_propertyOverviewRow']");
			foreach (var item in items)
			{
				var key = item.SelectSingleNode(".//div[contains(@class, \"p24_propertyOverviewKey\")]").InnerText.Trim();

				string value = "";
				if (key == "Street Address") value = item.SelectSingleNode(".//a[contains(@class, \"js_displayMap\")]").InnerText.Trim();
				else value = item.SelectSingleNode(".//div[contains(@class, \"p24_info\")]").InnerText.Trim();
				if (!result.ContainsKey(key)) result.Add(key, value);
			}
			return result;
		}

		public Dictionary<string, string> GetRoomsKeyValues()
		{
			var result = new Dictionary<string, string>();
			var container = _document.DocumentNode.SelectSingleNode("//div[@id='js_accordion_rooms']");
			if (container == null) return new Dictionary<string, string>();
			var items = container.SelectNodes(".//div[@class='row p24_propertyOverviewRow']");
			foreach (var item in items)
			{
				var key = item.SelectSingleNode(".//div[contains(@class, \"p24_propertyOverviewKey\")]").InnerText.Trim();
				var value = item.SelectSingleNode(".//div[contains(@class, \"p24_info\")]").InnerText.Trim();
				if (!result.ContainsKey(key)) result.Add(key, value);
			}
			return result;
		}

		public Dictionary<string, string> GetExternalFeaturesKeyValues()
		{
			var result = new Dictionary<string, string>();
			var container = _document.DocumentNode.SelectSingleNode("//div[@id='js_accordion_externalfeatures']");
			if (container == null) return new Dictionary<string, string>();
			var items = container.SelectNodes(".//div[@class='row p24_propertyOverviewRow']");
			foreach (var item in items)
			{
				var key = item.SelectSingleNode(".//div[contains(@class, \"p24_propertyOverviewKey\")]").InnerText.Trim();
				var value = item.SelectSingleNode(".//div[contains(@class, \"p24_info\")]").InnerText.Trim();
				if (!result.ContainsKey(key)) result.Add(key, value);
			}
			return result;
		}

		public Dictionary<string, string> GetBuildingKeyValues()
		{
			var result = new Dictionary<string, string>();
			var container = _document.DocumentNode.SelectSingleNode("//div[@id='js_accordion_building']");
			if (container == null) return new Dictionary<string, string>();
			var items = container.SelectNodes(".//div[@class='row p24_propertyOverviewRow']");
			foreach (var item in items)
			{
				var key = item.SelectSingleNode(".//div[contains(@class, \"p24_propertyOverviewKey\")]").InnerText.Trim();
				var value = item.SelectSingleNode(".//div[contains(@class, \"p24_info\")]").InnerText.Trim();
				if (!result.ContainsKey(key)) result.Add(key, value);
			}
			return result;
		}

		public Dictionary<string, string> GetOtherFeaturesKeyValues()
		{
			var result = new Dictionary<string, string>();
			var container = _document.DocumentNode.SelectSingleNode("//div[@id='js_accordion_otherfeatures']");
			if (container == null) return new Dictionary<string, string>();
			var items = container.SelectNodes(".//div[@class='row p24_propertyOverviewRow']");
			foreach (var item in items)
			{
				var key = item.SelectSingleNode(".//div[contains(@class, \"p24_propertyOverviewKey\")]").InnerText.Trim();
				var value = item.SelectSingleNode(".//div[contains(@class, \"p24_info\")]").InnerText.Trim();
				if (!result.ContainsKey(key)) result.Add(key, value);
			}
			return result;
		}

		private string InnerHtml(string selector)
		{
			var name = _document.DocumentNode.SelectSingleNode($"//{selector}");
			if (name == null) return "";
			return name.InnerHtml;
		}

		private string InnerText(string selector, int index = 0)
		{
			var names = _document.DocumentNode.SelectNodes($"//{selector}");
			if (names == null) return "";
			return names[index].InnerText.Replace("\r\n", "").Trim();
		}
	}
}

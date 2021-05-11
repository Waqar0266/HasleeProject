using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Nop.Plugin.Widgets.HassleFree.Services.Property24
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
            var breadcrumbs = _document.DocumentNode.SelectNodes("//div[@id='breadCrumbContainer']/ul/li");
            return breadcrumbs[5].InnerText.Replace("\r\n", "").Trim();
        }

        public string GetCity()
        {
            var breadcrumbs = _document.DocumentNode.SelectNodes("//div[@id='breadCrumbContainer']/ul/li");
            return breadcrumbs[7].InnerText.Replace("\r\n", "").Trim();
        }

        public string GetSuburb()
        {
            var breadcrumbs = _document.DocumentNode.SelectNodes("//div[@id='breadCrumbContainer']/ul/li");
            return breadcrumbs[9].InnerText.Replace("\r\n", "").Trim();
        }

        public decimal GetPrice()
        {
            var priceElements = _document.DocumentNode.SelectNodes("//div[@class='p24_price']");
            var price = priceElements[1].InnerText.Replace("\r\n", "").Trim().ToLower().Replace("r", "").Replace(" ", "");
            return decimal.Parse(price);
        }

        public string GetName()
        {
            var names = _document.DocumentNode.SelectNodes("//div[@class='p24_mBM']");
            return names[1].InnerText.Replace("\r\n", "").Trim();
        }

        public string GetDescription()
        {
            var name = _document.DocumentNode.SelectSingleNode("//span[@class='p24_dPL js_readMoreText']");
            return name.Attributes["data-visible"].Value.Trim() + name.Attributes["data-hidden"].Value.Trim();
        }
    }
}

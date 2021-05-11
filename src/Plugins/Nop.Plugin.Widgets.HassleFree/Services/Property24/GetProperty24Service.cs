using System.Threading.Tasks;
using HtmlAgilityPack;
using Nop.Plugin.Widgets.HassleFree.Models.Property24;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.HassleFree.Services.Property24
{
    public class GetProperty24Service : IGetProperty24Service
    {
        private string _propertyId;
        private readonly HtmlWeb _web;
        private IPriceFormatter _priceFormatter { get; }

        public GetProperty24Service(IPriceFormatter priceFormatter)
        {
            _web = new HtmlWeb();
            _priceFormatter = priceFormatter;
        }

        public IGetProperty24Service WithPropertyId(string propertyId)
        {
            _propertyId = propertyId;
            return this;
        }

        public async Task<Property24Model> Get()
        {
            var propertyInfo = new PropertyInfoPage(_web, _propertyId);
            return new Property24Model()
            {
                City = propertyInfo.GetCity(),
                Description = propertyInfo.GetDescription(),
                Name = propertyInfo.GetName(),
                PriceNumeric = propertyInfo.GetPrice(),
                Price = await _priceFormatter.FormatPriceAsync(propertyInfo.GetPrice()),
                Province = propertyInfo.GetProvince(),
                Suburb = propertyInfo.GetSuburb(),
                Images = propertyInfo.GetImages(),
                PropertyId = _propertyId
            };
        }
    }
}

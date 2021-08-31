using Hasslefree.Core.Infrastructure;
using Hasslefree.Web.Models.Listings;
using HtmlAgilityPack;

namespace Hasslefree.Services.Listings
{
	public class GetProperty24Service : IGetProperty24Service, IInstancePerRequest
	{
		private string _propertyId;
		private readonly HtmlWeb _web;

		public GetProperty24Service()
		{
			_web = new HtmlWeb();
		}

		public IGetProperty24Service WithPropertyId(string propertyId)
		{
			_propertyId = propertyId;
			return this;
		}

		public Property24Model Get()
		{
			var propertyInfo = new PropertyInfoPage(_web, _propertyId);
			return new Property24Model()
			{
				City = propertyInfo.GetCity(),
				Description = propertyInfo.GetDescription(),
				Name = propertyInfo.GetName(),
				PriceNumeric = propertyInfo.GetPrice(),
				Price = propertyInfo.GetPrice().ToString("# ##0"),
				Province = propertyInfo.GetProvince(),
				Suburb = propertyInfo.GetSuburb(),
				Images = propertyInfo.GetImages(),
				PropertyId = _propertyId,
				Address = propertyInfo.GetAddress(),
				OverviewKeyValues = propertyInfo.GetOverviewKeyValues(),
				RoomsKeyValues = propertyInfo.GetRoomsKeyValues(),
				ExternalFeaturesKeyValues = propertyInfo.GetExternalFeaturesKeyValues(),
				BuildingKeyValues = propertyInfo.GetBuildingKeyValues(),
				OtherFeaturesKeyValues = propertyInfo.GetOtherFeaturesKeyValues()
			};
		}
	}
}

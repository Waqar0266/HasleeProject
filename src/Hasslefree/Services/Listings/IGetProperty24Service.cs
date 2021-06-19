using Hasslefree.Web.Models.Listings;

namespace Hasslefree.Services.Listings
{
	public interface IGetProperty24Service
    {
        IGetProperty24Service WithPropertyId(string propertyId);
        Property24Model Get();
    }
}

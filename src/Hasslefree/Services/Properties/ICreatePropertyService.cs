using Hasslefree.Core.Domain.Properties;
using System.Collections.Generic;

namespace Hasslefree.Services.Properties
{
	public interface ICreatePropertyService
	{
		int PropertyId { get; }
		ICreatePropertyService New(PropertyType type, string title, string address, string description, decimal price, string privatePropertyId);
		ICreatePropertyService WithOverviewKeyValues(string key, string value);
		ICreatePropertyService WithRoomsKeyValues(string key, string value);
		ICreatePropertyService WithExternalFeaturesKeyValues(string key, string value);
		ICreatePropertyService WithBuildingKeyValues(string key, string value);
		ICreatePropertyService WithOtherFeaturesKeyValues(string key, string value);
		ICreatePropertyService WithLocation(string suburb, string city, string province);
		ICreatePropertyService WithImages(List<string> images);
		bool Create();
	}
}

using Hasslefree.Core.Domain.Properties;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Filter;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Filter
{
	public class AvailableFiltersService : IAvailableFiltersService, IInstancePerRequest
	{
		//Repos
		private IReadOnlyRepository<PropertyBuildingKeyValue> BuildingRepo { get; }
		private IReadOnlyRepository<PropertyExternalFeaturesKeyValue> ExternalFeaturesRepo { get; }
		private IReadOnlyRepository<PropertyOtherFeaturesKeyValue> OtherFeaturesRepo { get; }
		private IReadOnlyRepository<PropertyOverviewKeyValue> OverviewRepo { get; }
		private IReadOnlyRepository<PropertyRoomKeyValue> RoomRepo { get; }

		//Fields
		private List<FilterListItem> _items;

		public AvailableFiltersService
		(
		//Repos
		IReadOnlyRepository<PropertyBuildingKeyValue> buildingRepo,
		IReadOnlyRepository<PropertyExternalFeaturesKeyValue> externalFeaturesRepo,
		IReadOnlyRepository<PropertyOtherFeaturesKeyValue> otherFeaturesRepo,
		IReadOnlyRepository<PropertyOverviewKeyValue> overviewRepo,
		IReadOnlyRepository<PropertyRoomKeyValue> roomRepo
		)
		{
			//Repos
			BuildingRepo = buildingRepo;
			ExternalFeaturesRepo = externalFeaturesRepo;
			OtherFeaturesRepo = otherFeaturesRepo;
			OverviewRepo = overviewRepo;
			RoomRepo = roomRepo;
		}

		public IAvailableFiltersService WithItems(List<FilterListItem> items)
		{
			_items = items;
			return this;
		}

		public AvailableFilterModel Get()
		{
			var buildingList = new List<string>();
			var externalFeaturesList = new List<string>();
			var otherFeaturesList = new List<string>();
			var overviewList = new List<string>();
			var roomList = new List<string>();

			foreach (var item in _items)
			{
				buildingList.AddRange(item.BuildingKeyValues.Select(a => a.Key));
				externalFeaturesList.AddRange(item.ExternalFeaturesKeyValues.Select(a => a.Key));
				otherFeaturesList.AddRange(item.OtherFeaturesKeyValues.Select(a => a.Key));
				overviewList.AddRange(item.OverviewKeyValues.Select(a => a.Key));
				roomList.AddRange(item.RoomsKeyValues.Select(a => a.Key));
			}

			//distinct all the lists
			buildingList = buildingList.Distinct().ToList();
			externalFeaturesList = externalFeaturesList.Distinct().ToList();
			otherFeaturesList = otherFeaturesList.Distinct().ToList();
			overviewList = overviewList.Distinct().ToList();
			roomList = roomList.Distinct().ToList();

			var buildingDictionary = BuildingRepo.Table.Where(a => buildingList.Contains(a.Key)).ToDictionary(x => x.PropertyBuildingKeyValueId, x => x.Key + ": " + x.Value);
			var externalFeaturesDictionary = ExternalFeaturesRepo.Table.Where(a => externalFeaturesList.Contains(a.Key)).ToDictionary(x => x.PropertyExternalFeaturesKeyValueId, x => x.Key + ": " + x.Value);
			var otherFeaturesDictionary = OtherFeaturesRepo.Table.Where(a => otherFeaturesList.Contains(a.Key)).ToDictionary(x => x.PropertyOtherFeaturesKeyValueId, x => x.Key + ": " + x.Value);
			var overviewDictionary = OverviewRepo.Table.Where(a => overviewList.Contains(a.Key)).ToDictionary(x => x.PropertyOverviewKeyValueId, x => x.Key + ": " + x.Value);
			var roomDictionary = RoomRepo.Table.Where(a => roomList.Contains(a.Key)).ToDictionary(x => x.PropertyRoomKeyValueId, x => x.Key + ": " + x.Value);

			return new AvailableFilterModel()
			{
				BuildingKeyValues = buildingDictionary.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(aa => aa.Key).ToList()),
				ExternalFeaturesKeyValues = externalFeaturesDictionary.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(aa => aa.Key).ToList()),
				OtherFeaturesKeyValues = otherFeaturesDictionary.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(aa => aa.Key).ToList()),
				RoomsKeyValues = roomDictionary.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(aa => aa.Key).ToList()),
				OverviewKeyValues = overviewDictionary.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(aa => aa.Key).ToList()),
			};
		}
	}
}

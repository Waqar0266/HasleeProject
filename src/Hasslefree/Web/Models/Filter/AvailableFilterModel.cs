using System.Collections.Generic;

namespace Hasslefree.Web.Models.Filter
{
	public class AvailableFilterModel
	{
	public AvailableFilterModel()
	{
			this.RoomsKeyValues = new Dictionary<string, List<int>>();
			this.ExternalFeaturesKeyValues = new Dictionary<string, List<int>>();
			this.BuildingKeyValues = new Dictionary<string, List<int>>();
			this.OtherFeaturesKeyValues = new Dictionary<string, List<int>>();
			this.OverviewKeyValues = new Dictionary<string, List<int>>();
		}

		public Dictionary<string, List<int>> RoomsKeyValues { get; set; }
		public Dictionary<string, List<int>> ExternalFeaturesKeyValues { get; set; }
		public Dictionary<string, List<int>> BuildingKeyValues { get; set; }
		public Dictionary<string, List<int>> OverviewKeyValues { get; set; }
		public Dictionary<string, List<int>> OtherFeaturesKeyValues { get; set; }
	}
}

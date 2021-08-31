using System.Collections.Generic;

namespace Hasslefree.Web.Models.Listings
{
	public class Property24Model
    {
        public string PropertyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public string Address { get; set; }
		public string Province { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public decimal PriceNumeric { get; set; }
        public string Price { get; set; }
        public List<string> Images { get; set; }
		public Dictionary<string,string> OverviewKeyValues { get; set; }
		public Dictionary<string,string> RoomsKeyValues { get; set; }
		public Dictionary<string,string> ExternalFeaturesKeyValues { get; set; }
		public Dictionary<string,string> BuildingKeyValues { get; set; }
		public Dictionary<string,string> OtherFeaturesKeyValues { get; set; }
    }
}

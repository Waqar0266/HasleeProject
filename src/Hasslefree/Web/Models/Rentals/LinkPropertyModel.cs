using System.Collections.Generic;

namespace Hasslefree.Web.Models.Rentals
{
	public class LinkPropertyModel
	{
		public int RentalId { get; set; }
		public string Property24Id { get; set; }
		public string Title { get; set; }
		public string Address { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string PrettyPrice { get; set; }
		public string Province { get; set; }
		public string City { get; set; }
		public string Suburb { get; set; }
		public List<string> Images { get; set; }
		public string ImageList { get; set; }
		public string KeyValues { get; set; }
		public Dictionary<string, string> OverviewKeyValues { get; set; }
		public Dictionary<string, string> RoomsKeyValues { get; set; }
		public Dictionary<string, string> ExternalFeaturesKeyValues { get; set; }
		public Dictionary<string, string> BuildingKeyValues { get; set; }
		public Dictionary<string, string> OtherFeaturesKeyValues { get; set; }
	}
}

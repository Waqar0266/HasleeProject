using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Rentals
{
	public class RentalCreate
	{
		public RentalCreate()
		{
			this.Landlords = new List<RentalCreateLandlord>();
		}

		public LeaseType LeaseType { get; set; }
		public List<RentalCreateLandlord> Landlords { get; set; }
		public string Premises { get; set; }
		public string StandErf { get; set; }
		public string Address { get; set; }
		public string Township { get; set; }
		public int? RentalId { get; set; }
		public ExistingRentalType Option { get; set; }
	}

	public class RentalCreateLandlord
	{
		public string IdNumber { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
	}
}

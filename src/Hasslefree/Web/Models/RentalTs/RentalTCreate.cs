using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
	public class RentalTCreate
	{
		public int RentalId { get; set; }
		public List<RentalTCreateTenant> Tenants { get; set; }

	}
}

using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.RentalTs
{
	public class RentalTGet
	{
		public RentalTGet()
		{
			this.Tenants = new List<Tenant>();
		}

		public int RentalTId { get; set; }
		public Guid RentalTGuid { get; set; }
		public RentalGet Rental { get; set; }
		public List<Tenant> Tenants { get; set; }
	}
}

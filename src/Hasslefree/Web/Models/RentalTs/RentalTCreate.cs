using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Web.Models.RentalTs
{
	public class RentalTCreate
	{
		public LeaseType LeaseType { get; set; }
		public List<RentalTCreateTenant> Tenants { get; set; }

	}
}

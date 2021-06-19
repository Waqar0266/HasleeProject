using System;
using System.Collections.Generic;
using Hasslefree.Web.Models.Addresses;

namespace Hasslefree.Web.Models.Common.Address
{
	public class AddressListModel
	{
		public AddressSortBy SortBy { get; set; }
		public String Search { get; set; }
		public Int32 Page { get; set; }
		public Int32 PageSize { get; set; }

		public Int32 TotalItems { get; set; }
		public List<AddressViewModel> List { get; set; }
	}
}

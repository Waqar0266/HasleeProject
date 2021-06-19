using System;

namespace Hasslefree.Core.Domain.Common
{
	public class Country : BaseEntity
	{
		public Country()
		{
			//Set default values
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			Published = true;
		}

		public int CountryId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Name { get; set; }
		public bool AllowsBilling { get; set; }
		public bool AllowsShipping { get; set; }
		public string TwoLetterIsoCode { get; set; }
		public string ThreeLetterIsoCode { get; set; }
		public int NumericIsoCode { get; set; }
		public bool SubjectToVat { get; set; }
		public bool Published { get; set; }
		public int DisplayOrder { get; set; }

	}
}

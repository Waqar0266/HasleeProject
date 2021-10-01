using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class ExistingRentalForm : BaseEntity
	{
		public ExistingRentalForm()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int ExistingRentalFormId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int ExistingRentalId { get; set; }
		public ExistingRental ExistingRental { get; set; }
		public int DownloadId { get; set; }
		public Download Download { get; set; }
		public string ExistingRentalFormNameEnum { get; set; }
		public ExistingRentalFormName ExistingRentalFormName
		{
			get => (ExistingRentalFormName)Enum.Parse(typeof(ExistingRentalFormName), ExistingRentalFormNameEnum);
			set => ExistingRentalFormNameEnum = value.ToString();
		}
	}
}

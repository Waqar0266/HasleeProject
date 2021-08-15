using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalForm : BaseEntity
	{
		public int RentalFormId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalId { get; set; }
		public Rental Rental { get; set; }
		public int DownloadId { get; set; }
		public Download Download { get; set; }
		public string RentalFormNameEnum { get; set; }
		public RentalFormName RentalFormName
		{
			get => (RentalFormName)Enum.Parse(typeof(RentalFormName), RentalFormNameEnum);
			set => RentalFormNameEnum = value.ToString();
		}
	}
}

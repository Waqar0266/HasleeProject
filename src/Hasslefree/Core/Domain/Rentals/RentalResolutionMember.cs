using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalResolutionMember : BaseEntity
	{
		public RentalResolutionMember()
		{
			this.CreatedOn = DateTime.Now;
		}

		public int RentalResolutionMemberId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int RentalResolutionId { get; set; }
		public RentalResolution RentalResolution { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string IdNumber { get; set; }
		public string Email { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public string SignedAt { get; set; }
		public DateTime? SignedOn { get; set; }
	}
}

using Hasslefree.Core.Domain.Media;
using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTJuristicApplicant : BaseEntity
	{
		public RentalTJuristicApplicant()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int RentalTJuristicApplicantId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid UniqueId { get; set; }
		public int RentalTJuristicId { get; set; }
		public RentalTJuristic RentalTJuristic { get; set; }
		public string Position { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string IdNumber { get; set; }
		public string Nationality { get; set; }
		public string TelHome { get; set; }
		public string TelWork { get; set; }
		public string Mobile { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public int? InitialsId { get; set; }
		public Picture Initials { get; set; }
		public int? SignatureId { get; set; }
		public Picture Signature { get; set; }
		public string SignedAt { get; set; }
		public DateTime? SignedOn { get; set; }
	}
}

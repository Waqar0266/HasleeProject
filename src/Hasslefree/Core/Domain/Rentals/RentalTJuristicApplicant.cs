using Hasslefree.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTJuristicApplicant : BaseEntity
	{
		public int RentalTJuristicApplicantId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime Modified { get; set; }
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

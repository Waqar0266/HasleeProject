namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalResolutionMemberSignature
	{
		public int RentalId { get; set; }
		public int RentalResolutionMemberId { get; set; }
        public string Signature { get; set; }
		public string SignedAt { get; set; }
	}
}

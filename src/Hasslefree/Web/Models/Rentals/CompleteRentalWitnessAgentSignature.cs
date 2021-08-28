namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalWitnessAgentSignature
	{
		public string UniqueId { get; set; }
		public int WitnessNumber { get; set; }
		public int RentalId { get; set; }
		public string Signature { get; set; }
		public string Initials { get; set; }
		public string SignedAtSignature { get; set; }
	}
}

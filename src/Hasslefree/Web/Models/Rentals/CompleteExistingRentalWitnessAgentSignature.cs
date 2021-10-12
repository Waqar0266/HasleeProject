namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteExistingRentalWitnessAgentSignature
	{
		public int ExistingRentalId { get; set; }
		public int WitnessNumber { get; set; }
		public string Signature { get; set; }
		public string Initials { get; set; }
		public string SignedAtSignature { get; set; }
	}
}

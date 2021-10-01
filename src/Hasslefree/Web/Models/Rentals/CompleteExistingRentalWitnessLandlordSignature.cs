namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteExistingRentalWitnessLandlordSignature
	{
		public string UniqueId { get; set; }
		public int WitnessNumber { get; set; }
		public int ExistingRentalId { get; set; }
		public string Signature { get; set; }
		public string SignedAtSignature { get; set; }
	}
}

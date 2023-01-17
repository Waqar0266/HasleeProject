namespace Hasslefree.Web.Models.Rentals
{
	public class CompleteRentalAgentSignature
	{
		public int RentalId { get; set; }
		public string AgentGuid { get; set; }
		public string Signature { get; set; }
		public string Initials { get; set; }
		public string SignedAtSignature { get; set; }
		public string Witness1Name { get; set; }
		public string Witness1Surname { get; set; }
		public string Witness1Email { get; set; }
		public string Witness1Mobile { get; set; }
		public string Witness2Name { get; set; }
		public string Witness2Surname { get; set; }
		public string Witness2Email { get; set; }
		public string Witness2Mobile { get; set; }
	}
}

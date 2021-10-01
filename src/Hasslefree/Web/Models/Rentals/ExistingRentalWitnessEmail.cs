namespace Hasslefree.Web.Models.Rentals
{
	public class ExistingRentalWitnessEmail
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Link { get; set; }
		public RentalGet Rental { get; set; }
	}
}

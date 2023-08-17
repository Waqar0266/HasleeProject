namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalTLandlordApproval
    {
        public int RentalTId { get; set; }
        public RentalTGet Rental { get; set; }
        public string Hash { get; set; }
    }
}

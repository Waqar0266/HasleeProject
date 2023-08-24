namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalTAgentApproval
    {
        public int RentalTId { get; set; }
        public RentalTGet Rental { get; set; }
        public string Hash { get; set; }
        public string Action { get; set; }
        public string DeclineReason { get; set; }
    }
}

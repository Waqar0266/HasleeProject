namespace Hasslefree.Web.Models.RentalTs
{
    public class RentalTTenantApprovalEmail
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Link { get; set; }
        public bool Approved { get; set; }
        public string DeclineReason { get; set; }
    }
}

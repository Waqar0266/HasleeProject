namespace Hasslefree.Web.Models.RentalTs
{
    public class CompleteRentalTSignature
    {
        public int RentalTId { get; set; }
        public int TenantId { get; set; }
        public string Signature { get; set; }
        public string Initials { get; set; }
        public string SignedAtSignature { get; set; }
    }
}

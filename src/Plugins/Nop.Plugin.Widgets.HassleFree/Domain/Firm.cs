using Nop.Core;

namespace Nop.Plugin.Widgets.HassleFree.Domain
{
    public class Firm : BaseEntity
    {
        public string Name { get; set; }
        public string TradeName { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string PhysicalAddress3 { get; set; }
        public string PhysicalAddressPostalCode { get; set; }
        public string PostalAddress1 { get; set; }
        public string PostalAddress2 { get; set; }
        public string PostalAddress3 { get; set; }
        public string PostalAddressPostalCode { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Reference { get; set; }

    }
}
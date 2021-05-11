using System.Collections.Generic;

namespace Nop.Plugin.Widgets.HassleFree.Models.Property24
{
    public class Property24Model
    {
        public string PropertyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public decimal PriceNumeric { get; set; }
        public string Price { get; set; }
        public List<string> Images { get; set; }
    }
}

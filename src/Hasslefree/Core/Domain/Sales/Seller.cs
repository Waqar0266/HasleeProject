using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Sales
{
    public class Seller : BaseEntity
    {
        public Seller()
        {
            this.CreatedOn = DateTime.Now;
            this.ModifiedOn = DateTime.Now;
            this.UniqueId = Guid.NewGuid();
        }

        public int SellerId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Tempdata { get; set; }
        public string IdNumber { get; set; }
        public int? PersonId { get; set; }
        public Person Person { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
        public string VatNumber { get; set; }
        public int? PhysicalAddressId { get; set; }
        public Address PhysicalAddress { get; set; }
        public int? PostalAddressId { get; set; }
        public Address PostalAddress { get; set; }
        public int? SignatureId { get; set; }
        public Picture Signature { get; set; }
        public int? InitialsId { get; set; }
        public Picture Initials { get; set; }
        public string SignedAt { get; set; }
        public DateTime? SignedOn { get; set; }
    }
}

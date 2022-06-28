using Hasslefree.Core.Domain.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Domain.Sales
{
    public class Sale : BaseEntity
    {
        public Sale()
        {
            this.CreatedOn = DateTime.Now;
            this.ModifiedOn = DateTime.Now;
        }

        public int SaleId { get; set; }
        public Guid UniqueId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string SaleTypeEnum { get; set; }
        public SaleType SaleType
        {
            get => (SaleType)Enum.Parse(typeof(SaleType), SaleTypeEnum);
            set => SaleTypeEnum = value.ToString();
        }
        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        public decimal AgentCommissionPercentage { get; set; }
        public decimal AgentCommissionAmount { get; set; }
        public bool ExistingLightFittings { get; set; }
        public bool PoolCleaningEquipment { get; set; }
        public bool PelmentsAndCurtainFittings { get; set; }
        public bool FittedCarpets { get; set; }
        public bool Fences { get; set; }
        public bool Generators { get; set; }
        public bool TelevisionAerials { get; set; }
        public bool Stoves { get; set; }
        public bool Blinds { get; set; }
        public bool Pumps { get; set; }
        public bool PoolEquipment { get; set; }
        public bool BoreholePumps { get; set; }

    }
}

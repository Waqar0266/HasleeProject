using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Domain.Rentals;
using System;

namespace Hasslefree.Core.Domain.Sales
{
    public class SaleWitness : BaseEntity
    {
        public SaleWitness()
        {
            this.CreatedOn = DateTime.Now;
            this.ModifiedOn = DateTime.Now;
            this.UniqueId = Guid.NewGuid();
        }

        public int SaleWitnessId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid UniqueId { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
        public string SellerWitness1Name { get; set; }
        public string SellerWitness1Surname { get; set; }
        public string SellerWitness1Email { get; set; }
        public string SellerWitness1Mobile { get; set; }
        public string SellerWitness2Name { get; set; }
        public string SellerWitness2Surname { get; set; }
        public string SellerWitness2Email { get; set; }
        public string SellerWitness2Mobile { get; set; }
        public string AgentWitness1Name { get; set; }
        public string AgentWitness1Surname { get; set; }
        public string AgentWitness1Email { get; set; }
        public string AgentWitness1Mobile { get; set; }
        public string AgentWitness2Name { get; set; }
        public string AgentWitness2Surname { get; set; }
        public string AgentWitness2Email { get; set; }
        public string AgentWitness2Mobile { get; set; }
        public int? AgentWitness1SignatureId { get; set; }
        public Picture AgentWitness1Signature { get; set; }
        public int? AgentWitness1InitialsId { get; set; }
        public Picture AgentWitness1Initials { get; set; }
        public int? AgentWitness2SignatureId { get; set; }
        public Picture AgentWitness2Signature { get; set; }
        public int? AgentWitness2InitialsId { get; set; }
        public Picture AgentWitness2Initials { get; set; }
        public int? SellerWitness1SignatureId { get; set; }
        public Picture SellerWitness1Signature { get; set; }
        public int? SellerWitness1InitialsId { get; set; }
        public Picture SellerWitness1Initials { get; set; }
        public int? SellerWitness2SignatureId { get; set; }
        public Picture SellerWitness2Signature { get; set; }
        public int? SellerWitness2InitialsId { get; set; }
        public Picture SellerWitness2Initials { get; set; }
        public string WitnessStatusEnum { get; set; }
        public WitnessStatus WitnessStatus
        {
            get => (WitnessStatus)Enum.Parse(typeof(WitnessStatus), WitnessStatusEnum);
            set => WitnessStatusEnum = value.ToString();
        }
        public string SellerWitness1SignedAt { get; set; }
        public string SellerWitness2SignedAt { get; set; }
        public string AgentWitness1SignedAt { get; set; }
        public string AgentWitness2SignedAt { get; set; }
        public DateTime? SellerWitness1SignedOn { get; set; }
        public DateTime? SellerWitness2SignedOn { get; set; }
        public DateTime? AgentWitness1SignedOn { get; set; }
        public DateTime? AgentWitness2SignedOn { get; set; }
    }
}

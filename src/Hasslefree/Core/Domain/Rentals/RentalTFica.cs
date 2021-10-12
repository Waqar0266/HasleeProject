using System;

namespace Hasslefree.Core.Domain.Rentals
{
	public class RentalTFica : BaseEntity
	{
		public RentalTFica()
		{
			this.CreatedOn = DateTime.Now;
			this.ModifiedOn = DateTime.Now;
			this.UniqueId = Guid.NewGuid();
		}

		public int RentalTFicaId { get; set; }
		public Guid UniqueId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public int RentalTId { get; set; }
		public RentalT RentalT { get; set; }
		public bool ForeignMemberOfRoyalFamily { get; set; }
		public bool ForeignMemberOfCabinet { get; set; }
		public bool ForeignSeniorPolitical { get; set; }
		public bool ForeignSeniorJudicialOfficer { get; set; }
		public bool ForeignSeniorExecutives { get; set; }
		public bool ForeignHighRankingMilitary { get; set; }
		public bool DomesticDeputyPresident { get; set; }
		public bool DomesticCabinetMinister { get; set; }
		public bool DomesticPremier { get; set; }
		public bool DomesticMemberOfCouncil { get; set; }
		public bool DomesticMayor { get; set; }
		public bool DomesticLeaderPolitical { get; set; }
		public bool DomesticRoyalFamily { get; set; }
		public bool DomesticCfoProvincial { get; set; }
		public bool DomesticCfoMunicipality { get; set; }
		public bool DomesticCeo { get; set; }
		public bool DomesticJudge { get; set; }
		public bool DomesticAmbassador { get; set; }
		public bool DomesticOther { get; set; }
	}
}

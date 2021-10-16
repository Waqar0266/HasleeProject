using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalTFicaConfiguration : EntityTypeConfiguration<RentalTFica>
	{
		public RentalTFicaConfiguration()
		{
			// Table
			ToTable("RentalTFica");

			// Primary Key
			HasKey(a => a.RentalTFicaId);

			HasRequired(a => a.RentalT)
			.WithMany()
			.HasForeignKey(a => a.RentalTId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.UniqueId).IsRequired();
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.DomesticAmbassador).IsRequired();
			Property(a => a.DomesticCabinetMinister).IsRequired();
			Property(a => a.DomesticCeo).IsRequired();
			Property(a => a.DomesticCfoMunicipality).IsRequired();
			Property(a => a.DomesticCfoProvincial).IsRequired();
			Property(a => a.DomesticDeputyPresident).IsRequired();
			Property(a => a.DomesticJudge).IsRequired();
			Property(a => a.DomesticLeaderPolitical).IsRequired();
			Property(a => a.DomesticMayor).IsRequired();
			Property(a => a.DomesticMemberOfCouncil).IsRequired();
			Property(a => a.DomesticOther).IsRequired();
			Property(a => a.DomesticPremier).IsRequired();
			Property(a => a.DomesticRoyalFamily).IsRequired();
			Property(a => a.ForeignHighRankingMilitary).IsRequired();
			Property(a => a.ForeignMemberOfCabinet).IsRequired();
			Property(a => a.ForeignMemberOfRoyalFamily).IsRequired();
			Property(a => a.ForeignSeniorExecutives).IsRequired();
			Property(a => a.ForeignSeniorJudicialOfficer).IsRequired();
			Property(a => a.ForeignSeniorPolitical).IsRequired();
		}
	}
}

using Hasslefree.Core.Domain.Agents;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Agents
{
	public class AgentConfiguration : EntityTypeConfiguration<Agent>
	{
		public AgentConfiguration()
		{
			// Table
			ToTable("Agent");

			// Primary Key
			HasKey(a => a.AgentId);

			HasOptional(a => a.Person)
			.WithMany()
			.HasForeignKey(a => a.PersonId)
			.WillCascadeOnDelete(true);

			HasOptional(a => a.EaabProofOfPayment)
			.WithMany()
			.HasForeignKey(a => a.EaabProofOfPaymentId)
			.WillCascadeOnDelete(true);

			HasOptional(a => a.Initials)
			.WithMany()
			.HasForeignKey(a => a.InitialsId)
			.WillCascadeOnDelete(true);

			HasOptional(a => a.Signature)
			.WithMany()
			.HasForeignKey(a => a.SignatureId)
			.WillCascadeOnDelete(true);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.AgentGuid).IsRequired();
			Property(a => a.AgentStatusEnum).IsRequired().HasMaxLength(32);
			Property(a => a.AgentTypeEnum).IsRequired().HasMaxLength(32);
			Property(a => a.Convicted).IsRequired();
			Property(a => a.Dismissed).IsRequired();
			Property(a => a.EaabReference).IsOptional().HasMaxLength(55);
			Property(a => a.Ffc).IsRequired();
			Property(a => a.FfcIssueDate).IsOptional();
			Property(a => a.FfcNumber).IsOptional().HasMaxLength(55);
			Property(a => a.IdNumber).IsOptional().HasMaxLength(32);
			Property(a => a.Insolvent).IsRequired();
			Property(a => a.Nationality).IsOptional().HasMaxLength(55);
			Property(a => a.Race).IsOptional().HasMaxLength(55);
			Property(a => a.PreviousEmployer).IsOptional().HasMaxLength(128);
			Property(a => a.Withdrawn).IsRequired();
			Property(a => a.TempData).IsOptional().HasMaxLength(255);

			// Ignore
			Ignore(a => a.AgentStatus);
			Ignore(a => a.AgentType);
		}
	}
}

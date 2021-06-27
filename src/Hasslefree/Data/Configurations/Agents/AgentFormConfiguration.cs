using Hasslefree.Core.Domain.Agents;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Agents
{
	public class AgentFormConfiguration : EntityTypeConfiguration<AgentForm>
	{
		public AgentFormConfiguration()
		{
			// Table
			ToTable("AgentForm");

			// Primary Key
			HasKey(a => a.AgentFormId);

			HasRequired(a => a.Agent)
			.WithMany()
			.HasForeignKey(a => a.AgentId)
			.WillCascadeOnDelete(true);

			HasRequired(a => a.Download)
			.WithMany()
			.HasForeignKey(a => a.DownloadId)
			.WillCascadeOnDelete(true);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.FormNameEnum).IsRequired().HasMaxLength(255);

			// Ignore
			Ignore(a => a.FormName);
		}
	}
}

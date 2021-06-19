using Hasslefree.Core.Domain.Agents;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Agents
{
	public class AgentDocumentationConfiguration : EntityTypeConfiguration<AgentDocumentation>
	{
		public AgentDocumentationConfiguration()
		{
			// Table
			ToTable("AgentDocumentation");

			// Primary Key
			HasKey(a => a.AgentDocumentationId);

			HasRequired(a => a.Download)
			.WithMany()
			.HasForeignKey(a => a.DownloadId)
			.WillCascadeOnDelete(true);

			HasRequired(a => a.Agent)
			.WithMany()
			.HasForeignKey(a => a.AgentId)
			.WillCascadeOnDelete(true);
		}
	}
}

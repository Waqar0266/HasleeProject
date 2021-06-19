using Hasslefree.Core.Domain.Agents;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Agents
{
	public class AgentAddressConfiguration : EntityTypeConfiguration<AgentAddress>
	{
		public AgentAddressConfiguration()
		{
			// Table
			ToTable("AgentAddress");

			// Primary Key
			HasKey(a => a.AgentAddressId);

			HasRequired(a => a.Address)
			.WithMany()
			.HasForeignKey(a => a.AddressId)
			.WillCascadeOnDelete(true);

			HasRequired(a => a.Agent)
			.WithMany()
			.HasForeignKey(a => a.AgentId)
			.WillCascadeOnDelete(true);
		}
	}
}

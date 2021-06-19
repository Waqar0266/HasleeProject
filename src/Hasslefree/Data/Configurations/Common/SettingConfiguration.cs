using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Hasslefree.Data.Configurations.Common
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class SettingConfiguration : EntityTypeConfiguration<Core.Domain.Common.Setting>
	{
		public SettingConfiguration()
		{
			// Table
			ToTable("Setting");

			// Primary Key
			HasKey(a => a.SettingId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(255);
			Property(a => a.Value).IsRequired().HasMaxLength(2048);

			// Constraints & Indexes
			Property(a => a.Key)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(
						new IndexAttribute("UIX_Setting_Key", 2)));
		}
	}
}



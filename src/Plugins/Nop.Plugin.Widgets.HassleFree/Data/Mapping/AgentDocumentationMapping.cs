using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.HassleFree.Domain;

namespace Nop.Plugin.Widgets.HassleFree.Data.Mapping
{
    public class AgentDocumentationMapping : NopEntityBuilder<AgentDocumentation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn("AgentId").AsInt32().ForeignKey<Agent>().NotNullable()
                .WithColumn("CreatedOn").AsDateTime()
                .WithColumn("DownloadId").AsInt32().ForeignKey<Download>().NotNullable();
        }

        #endregion
    }
}

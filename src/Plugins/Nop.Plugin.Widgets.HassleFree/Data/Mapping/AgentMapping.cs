using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.HassleFree.Domain;

namespace Nop.Plugin.Widgets.HassleFree.Data.Mapping
{
    public class AgentMapping : NopEntityBuilder<Agent>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn("DirectorId").AsInt32().Nullable().ForeignKey<Customer>()
                .WithColumn("CustomerId").AsInt32().ForeignKey<Customer>().Nullable()
                .WithColumn("Province").AsString(100)
                .WithColumn("UniqueId").AsGuid().NotNullable();
        }

        #endregion
    }
}

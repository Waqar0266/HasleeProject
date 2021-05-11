using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.HassleFree.Domain;

namespace Nop.Plugin.Widgets.HassleFree.Data.Mapping
{
    public class FirmMapping : NopEntityBuilder<Firm>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("TradeName").AsString(255).NotNullable()
                .WithColumn("PhysicalAddress1").AsString(255).NotNullable()
                .WithColumn("PhysicalAddress2").AsString(255).Nullable()
                .WithColumn("PhysicalAddress3").AsString(255).Nullable()
                .WithColumn("PhysicalAddressPostalCode").AsString(10).NotNullable()
                .WithColumn("PostalAddress1").AsString(255).NotNullable()
                .WithColumn("PostalAddress2").AsString(255).Nullable()
                .WithColumn("PostalAddress3").AsString(255).Nullable()
                .WithColumn("PostalAddressPostalCode").AsString(10).NotNullable()
                .WithColumn("Tel").AsString(15).NotNullable()
                .WithColumn("Fax").AsString(15).Nullable()
                .WithColumn("Email").AsString(100).NotNullable()
                .WithColumn("Reference").AsString(100).NotNullable();
        }

        #endregion
    }
}

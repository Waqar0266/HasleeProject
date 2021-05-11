using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.HassleFree.Domain;

namespace Nop.Plugin.Widgets.HassleFree.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/04/07 09:09:17:6455442", "Widgets.HassleFree base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Fields

        protected IMigrationManager _migrationManager;

        #endregion

        #region Ctor

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<Agent>(Create);
            _migrationManager.BuildTable<Firm>(Create);
        }

        #endregion
    }
}

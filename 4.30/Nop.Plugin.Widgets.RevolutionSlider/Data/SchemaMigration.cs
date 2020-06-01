using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/05/26 09:30:17:6455422", "Widgets.RevolutionSlider base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<RevSlider>(Create);
            _migrationManager.BuildTable<RevSliderWidgetZone>(Create);
            _migrationManager.BuildTable<RevSliderUrlRecord>(Create);
            _migrationManager.BuildTable<RevSliderProduct>(Create);


        }
    }

}

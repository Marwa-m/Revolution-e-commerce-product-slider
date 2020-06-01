using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Data
{
        public partial class RevSliderWidgetZoneBuilder : NopEntityBuilder<RevSliderWidgetZone>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
               table.WithColumn(nameof(RevSliderWidgetZone.RevSliderId)).AsInt32().ForeignKey().NotNullable();

        }

        #endregion
    }

}

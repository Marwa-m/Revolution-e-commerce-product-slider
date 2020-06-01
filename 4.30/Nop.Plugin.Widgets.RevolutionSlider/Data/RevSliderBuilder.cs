using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Data
{
    public partial class RevSliderBuilder : NopEntityBuilder<RevSlider>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RevSlider.Name)).AsString(500).NotNullable()
                .WithColumn(nameof(RevSlider.FromTimeUtc)).AsTime().Nullable()
                .WithColumn(nameof(RevSlider.ToTimeUtc)).AsTime().Nullable();




        }

        #endregion
    }

}

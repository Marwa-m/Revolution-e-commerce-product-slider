using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Data
{
    /// <summary>
    /// Represents a product entity builder
    /// </summary>
    public partial class RevSliderProductBuilder : NopEntityBuilder<RevSliderProduct>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RevSliderProduct.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(RevSliderProduct.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(RevSliderProduct.RevSliderId)).AsInt32().ForeignKey().NotNullable();
                

        }

        #endregion
    }
}

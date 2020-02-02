using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    public partial class ProductSliderMap :NopEntityTypeConfiguration<ProductSlider>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductSlider> builder)
        {
            builder.ToTable("ProductSlider");
            builder.HasKey(p => p.Id);
            builder.Property(c => c.Name).HasMaxLength(400);
            builder.Property(p => p.Price).HasColumnType("decimal(18, 4)");
            builder.Property(p => p.OldPrice).HasColumnType("decimal(18, 4)");
            builder.Property(p => p.ProductId).IsRequired();
            base.Configure(builder);
        }

        #endregion
        
    }
}

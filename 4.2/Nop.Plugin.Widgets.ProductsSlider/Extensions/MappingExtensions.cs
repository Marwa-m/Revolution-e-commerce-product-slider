using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Extensions
{
   public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        public static ProductSliderModel ToModel(this ProductSlider entity)
        {
            return entity.MapTo<ProductSlider, ProductSliderModel>();
        }
        public static ProductSlider ToEntity(this ProductSliderModel model)
        {
            return model.MapTo<ProductSliderModel, ProductSlider>();
        }
        public static ProductProductSliderModel ToModel(this ProductModel model)
        {
            return model.MapTo<ProductModel, ProductProductSliderModel>();
        }
        public static ProductSlider ToEntity(this ProductSliderModel model, ProductSlider destination)
        {
            return model.MapTo(destination);
        }
        public static ProductOverviewModel ToModel(this Product model, ProductOverviewModel destination)
        {
            return model.MapTo(destination);
        }
    }
}

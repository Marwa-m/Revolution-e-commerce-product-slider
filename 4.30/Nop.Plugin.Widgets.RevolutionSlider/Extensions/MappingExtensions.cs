using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.RevolutionSlider.Extensions
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

        public static RevSliderModel ToModel(this RevSlider entity)
        {
            return entity.MapTo<RevSlider, RevSliderModel>();
        }

        public static RevSlider ToEntity(this RevSliderModel model)
        {
            return model.MapTo<RevSliderModel, RevSlider>();
        }

        public static RevSlider ToEntity(this RevSliderModel model, RevSlider destination)
        {
            return model.MapTo(destination);
        }
        public static RevSliderModel.AddRevSliderProductModel ToModel(this RevSliderProduct entity)
        {
            return entity.MapTo<RevSliderProduct, RevSliderModel.AddRevSliderProductModel>();
        }
        public static RevSliderProduct ToEntity(this RevSliderModel.AddRevSliderProductModel model)
        {
            return model.MapTo<RevSliderModel.AddRevSliderProductModel, RevSliderProduct>();
        }
       
        public static RevSliderProduct ToEntity(this RevSliderModel.AddRevSliderProductModel model, RevSliderProduct destination)
        {
            return model.MapTo(destination);
        }
        public static ProductOverviewModel ToModel(this Product model, ProductOverviewModel destination)
        {
            return model.MapTo(destination);
        }
    }
}

using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Domain.Infrastructure.Mapper
{
    class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public AdminMapperConfiguration()
        {
            //productSliders
            CreateMap<ProductSlider, ProductSliderModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

            CreateMap<ProductSliderModel, ProductSlider>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
            CreateMap<Product, ProductOverviewModel>()
            .ForMember(dest => dest.ProductPrice, mo => mo.Ignore())
            .ForMember(dest => dest.ProductType, mo => mo.Ignore());

        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

    }
}

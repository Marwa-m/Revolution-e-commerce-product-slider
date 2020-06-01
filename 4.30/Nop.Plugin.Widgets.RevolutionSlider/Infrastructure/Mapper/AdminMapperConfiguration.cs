using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain.Infrastructure.Mapper
{
    class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public AdminMapperConfiguration()
        {
            //productSliders
            CreateMap<RevSliderProduct, RevSliderModel.AddRevSliderProductModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

            CreateMap<RevSliderModel.AddRevSliderProductModel, RevSliderProduct>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            CreateMap<Product, ProductOverviewModel>()
            .ForMember(dest => dest.ProductPrice, mo => mo.Ignore())
            .ForMember(dest => dest.ProductType, mo => mo.Ignore());

            CreateMap<RevSlider, RevSliderModel>()
               .ForMember(dest => dest.Locales, mo => mo.Ignore())
               .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
               .ForMember(dest => dest.SelectedCustomerRoleIds, mo => mo.Ignore())
               .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
               .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
               .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            CreateMap<RevSliderModel, RevSlider>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());

            CreateMap<RevSliderWidgetZone, RevSliderWidgetZoneModel>();
            CreateMap<RevSliderWidgetZoneModel, RevSliderWidgetZone>()
                .ForMember(entity => entity.RevSliderId, options => options.Ignore())
                .ForMember(entity => entity.RevSlider, options => options.Ignore());

            CreateMap<RevSliderUrlRecord, RevSliderUrlRecordModel>();
            CreateMap<RevSliderUrlRecordModel, RevSliderUrlRecord>()
                .ForMember(entity => entity.RevSliderId, options => options.Ignore())
                .ForMember(entity => entity.RevSlider, options => options.Ignore());
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

    }
}

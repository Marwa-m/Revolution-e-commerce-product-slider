using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Plugin.Widgets.ProductsSlider.Extensions;
using Nop.Plugin.Widgets.ProductsSlider.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.ProductsSlider.Factories
{
    public partial class ProductSliderModelFactory:IProductSliderModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IProductSliderService _productSliderService;
        private readonly IPictureService _pictureService;

        #endregion
        #region Ctor
        public ProductSliderModelFactory(ILocalizationService localizationService, ILocalizedModelFactory localizedModelFactory, IBaseAdminModelFactory baseAdminModelFactory, IProductSliderService productSliderService,IPictureService pictureService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _productSliderService = productSliderService;
            _pictureService = pictureService;
        }

        #endregion
        public virtual ProductSliderSearchModel PrepareProductSliderSearchModel(ProductSliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //   searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();
            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.All"), Value = "0" });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.PublishedOnly"), Value = "1" });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual ProductSliderListModel PrepareProductSliderListModel(ProductSliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
            var productSliders = _productSliderService.SearchProductSliders(
                Name: searchModel.SearchName,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );

            //prepare list model
            var model = new ProductSliderListModel().PrepareToGrid(searchModel, productSliders, () =>
            {
                return productSliders.Select(productslider =>
                {
                    //fill in model values from the entity
                    var productSliderModel = productslider.ToModel<ProductSliderModel>();
                    productSliderModel.ShortDescription = "";
                    //picture
                    var picture = _pictureService.GetPictureById(productslider.PictureId);
                    productSliderModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(picture, 75, true);

                    return productSliderModel;
                });
            });

            return model;
        }

    }
}

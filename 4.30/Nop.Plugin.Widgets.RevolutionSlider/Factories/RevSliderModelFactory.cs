using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Seo;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Plugin.Widgets.RevolutionSlider.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.RevolutionSlider.Factories
{
    public partial class RevSliderModelFactory:IRevSliderModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IRevSliderService _revSliderService;
        private readonly IPictureService _pictureService;


        #endregion
        #region Ctor
        public RevSliderModelFactory(ILocalizationService localizationService, ILocalizedModelFactory localizedModelFactory, IBaseAdminModelFactory baseAdminModelFactory, IRevSliderService revSliderService, IPictureService pictureService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _revSliderService = revSliderService;
            _pictureService = pictureService;
        }


        #endregion
        public RevSliderModel PrepareRevSliderModel(RevSliderModel model, RevSlider revSlider, bool excludeProperties = false)
        {
            Action<RevSliderLocalizedModel, int> localizedModelConfiguration = null;

            if (revSlider != null)
            {
                //fill in model values from the entity
                model = model ?? revSlider.ToModel<RevSliderModel>();

                //prepare nested search model
                PrepareRevSliderProductSearchModel(model.RevSliderProductSearchModel, revSlider);


                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(revSlider, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (revSlider == null)
            {
                model.Published = true;

            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);


            return model;
        }

        public virtual RevSliderSearchModel PrepareRevSliderSearchModel(RevSliderSearchModel searchModel)
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
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.All"), Value = "0" });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.PublishedOnly"), Value = "1" });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual RevSliderListModel PrepareRevSliderListModel(RevSliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var revSliders = _revSliderService.GetAllRevSliders(revSliderName: searchModel.SearchName, storeId: searchModel.SearchStoreId, showHidden: true,
  pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));



            //prepare list model
             var model = new RevSliderListModel().PrepareToGrid(searchModel, revSliders, () =>
                {
                    return revSliders.Select(revSlider =>
                    {
                        //fill in model values from the entity
                        var revSliderModel = revSlider.ToModel<RevSliderModel>();


                        return revSliderModel;
                    });
                });
            return model;
        }


        protected virtual RevSliderProductSearchModel PrepareRevSliderProductSearchModel(RevSliderProductSearchModel searchModel, RevSlider revSlider)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (revSlider == null)
                throw new ArgumentNullException(nameof(revSlider));

            searchModel.RevSliderId = revSlider.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual RevSliderProductListModel PrepareRevSliderProductListModel(RevSliderProductSearchModel searchModel,RevSlider revSlider)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

           
            var revSliderProducts = _revSliderService.GetRevSliderProductsByRevSliderId(revSlider.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            //prepare list model
            var model = new RevSliderProductListModel().PrepareToGrid(searchModel, revSliderProducts, () =>
            {
                return revSliderProducts.Select(revSliderProduct =>
                {
                    //fill in model values from the entity
                    var productSliderModel = revSliderProduct.ToModel<RevSliderModel.AddRevSliderProductModel>();
                    productSliderModel.ShortDescription = "";
                    //picture
                    productSliderModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(revSliderProduct.PictureId, 75, true);

                    return productSliderModel;
                });
            });

            return model;
        }

        #region widgetZone
        public virtual RevSliderWidgetZoneListModel PrepareRevSliderWidgetZoneListModel(RevSliderWidgetZoneSearchModel searchModel, RevSlider revSlider)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (revSlider == null)
                throw new ArgumentNullException(nameof(revSlider));

            //get product categories
            var widgetZoneRevSliders = _revSliderService.GetRevSliderWidgetZonesByRevSliderId(revSlider.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new RevSliderWidgetZoneListModel().PrepareToGrid(searchModel, widgetZoneRevSliders, () =>
            {
                return widgetZoneRevSliders.Select(widgetzone =>
                {
                    //fill in model values from the entity
                    var revSliderWidgetZoneModel = widgetzone.ToModel<RevSliderWidgetZoneModel>();

                    //fill in additional values (not existing in the entity)
                    // revSliderProductModel.WidgetZone = _productService.GetProductById(productCategory.ProductId)?.Name;

                    return revSliderWidgetZoneModel;
                });
            });

            return model;
        }
        #endregion

        #region UrlRecord
        public virtual RevSliderUrlRecordListModel PrepareRevSliderUrlRecordListModel(RevSliderUrlRecordSearchModel searchModel, RevSlider revSlider)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (revSlider == null)
                throw new ArgumentNullException(nameof(revSlider));

            var urlRecordRevSliders = _revSliderService.GetRevSliderUrlRecordsByRevSliderId(revSlider.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

           // prepare grid model
            var model = new RevSliderUrlRecordListModel().PrepareToGrid(searchModel, urlRecordRevSliders, () =>
            {
                return urlRecordRevSliders.Select(item =>
                {
                    //fill in model values from the entity
                    var revSliderUrlRecordModel = item;

                    //fill in additional values (not existing in the entity)
                    // revSliderProductModel.WidgetZone = _productService.GetProductById(productCategory.ProductId)?.Name;

                    return revSliderUrlRecordModel;
                });
            });

            return model;
        }

        #endregion
    }
}

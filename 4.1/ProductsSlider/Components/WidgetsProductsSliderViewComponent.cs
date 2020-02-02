using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.ProductsSlider.Extensions;
using Nop.Plugin.Widgets.ProductsSlider.Services;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.ProductsSlider.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Configuration;

namespace Nop.Plugin.Widgets.ProductsSlider.Components
{
    [ViewComponent(Name = "WidgetsProductsSlider")]
    public class WidgetsProductsSliderViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IProductSliderService _productSliderService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        #endregion
        public WidgetsProductsSliderViewComponent(IProductSliderService productSliderService,
                                            IPictureService pictureService,
                                            IWorkContext workContext,
                                            ICurrencyService currencyService,
                                            IProductService productService,
                                            IStoreContext storeContext,
                                            IProductModelFactory productModelFactory,
                                            IPriceFormatter priceFormatter,
                                            ILocalizationService localizationService,
                                            IStaticCacheManager staticCacheManager,
                                            ISettingService settingService)
        {
            this._productSliderService = productSliderService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._productService = productService;
            this._storeContext = storeContext;
            this._productModelFactory = productModelFactory;
            this._priceFormatter = priceFormatter;
            this._workContext = workContext;
            _localizationService = localizationService;
            _cacheManager = staticCacheManager;
            _settingService = settingService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
          DateTime dtCreate=  _settingService.GetSettingByKey<DateTime>("ProductSliderdt");
            if (dtCreate.AddDays(7) < DateTime.Now)
                return View("~/Plugins/Widgets.ProductsSlider/Views/PublicInfo.cshtml", new List<ProductSliderPresentationModel>());
            var CacheKey = string.Format(ModelCacheEventConsumer.PRODUCTSLIDERS_BY_WIDGET_ZONE_KEY,
                _workContext.WorkingLanguage.Id,
                _workContext.CurrentCustomer.Id,
                _storeContext.CurrentStore.Id);
            var productSlidersModels = _cacheManager.Get(CacheKey, () =>
                _productSliderService.GetProductSliderForShowing(_storeContext.CurrentStore.Id)
                .Select(x =>
                {

                    ProductSliderPresentationModel model = new ProductSliderPresentationModel
                    {
                        IsExternal = x.IsExternal,
                        Link = x.Link,
                        Name = _localizationService.GetLocalized(x, l => l.Name),
                        Preview = x.Preview,
                        ProductId = x.ProductId,
                        ProductSliderId = x.ProductId,
                        ShortDescription = _localizationService.GetLocalized(x, l => l.ShortDescription),
                        OldPrice = (x.OldPrice > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.OldPrice, _workContext.WorkingCurrency)) : ""),
                        Price = (x.Price > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.Price, _workContext.WorkingCurrency)) : ""),

                    };
                    //picture
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    model.PictureThumbnailUrl = _pictureService.GetPictureUrl(picture, 0, true);
                    if (model.ProductId != 0)
                    {
                        var product = _productService.GetProductById(model.ProductId);
                        if (product != null)
                        {
                            var productOverviewmodel = _productModelFactory.PrepareProductOverviewModels(new[] { product }.AsEnumerable<Product>()).FirstOrDefault();

                            model.productOverviewModel = productOverviewmodel;
                        }

                    }
                    return model;
                })
                .ToList()
            );
            
            return View("~/Plugins/Widgets.ProductsSlider/Views/PublicInfo.cshtml", productSlidersModels);

        }
    }
}

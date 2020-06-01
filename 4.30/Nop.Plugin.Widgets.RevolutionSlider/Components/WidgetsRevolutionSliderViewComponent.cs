using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.RevolutionSlider.Extensions;
using Nop.Plugin.Widgets.RevolutionSlider.Services;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
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
using Nop.Plugin.Widgets.RevolutionSlider.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Configuration;
using Nop.Services.Caching;
using Nop.Plugin.Widgets.RevolutionSlider.Models;

namespace Nop.Plugin.Widgets.Revolution.Components
{
    [ViewComponent(Name = "WidgetsRevolutionSlider")]
    public class WidgetsRevolutionSliderViewComponent : NopViewComponent
    {
        #region Fields
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRevSliderService _RevSliderService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        #endregion
        public WidgetsRevolutionSliderViewComponent(IRevSliderService RevSliderService,
                                            IPictureService pictureService,
                                            IWorkContext workContext,
                                            ICurrencyService currencyService,
                                            IProductService productService,
                                            IStoreContext storeContext,
                                            IProductModelFactory productModelFactory,
                                            IPriceFormatter priceFormatter,
                                            ILocalizationService localizationService,
                                            IStaticCacheManager staticCacheManager,
                                            ISettingService settingService,
                                            ICacheKeyService cacheKeyService,
                                            IWebHelper webHelper
                                            )
        {
            this._RevSliderService = RevSliderService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._productService = productService;
            this._storeContext = storeContext;
            this._productModelFactory = productModelFactory;
            this._priceFormatter = priceFormatter;
            this._workContext = workContext;
            _localizationService = localizationService;
            _staticCacheManager = staticCacheManager;
            _settingService = settingService;
            _cacheKeyService = cacheKeyService;
            _webHelper = webHelper;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {

            ////Trial Version
            //DateTime dtCreate = _settingService.GetSettingByKey<DateTime>("RevSliderdt4.3");
            //if (dtCreate.AddDays(7) < DateTime.Now)
            //    return View("~/Plugins/Widgets.RevolutionSlider/Views/PublicInfo.cshtml", new List<RevSliderPresentationModel>());

            var revSliders = _RevSliderService.GetRevSlidersByWidgetZoneName(widgetZone);
            List<RevSliderPresentationModel> listModel = new List<RevSliderPresentationModel>();
            if(revSliders !=null && revSliders.Count>0)
            {
                RevSliderPresentationModel revPresentationModel = null;
                foreach (var x in revSliders)
                {
                    revPresentationModel = new RevSliderPresentationModel();
                    revPresentationModel.Name = _localizationService.GetLocalized(x, l => l.Name);
                    revPresentationModel.CustomStyle = x.CustomStyle;
                    revPresentationModel.HideOnMobile = x.HideOnMobile;
                    revPresentationModel.ShowName = x.ShowName;
                    revPresentationModel.revSliderProductPresentaionModels = _RevSliderService.PrepareRevSliderProductPresentaionModel(x.Id);
                    listModel.Add(revPresentationModel);
                }
                
            }
            //var revSlidersModels = revSliders.Select(x =>
            //{
            //    RevSliderPresentationModel model = new RevSliderPresentationModel
            //    {
            //        Name = _localizationService.GetLocalized(x, l => l.Name),
            //        CustomStyle = x.CustomStyle,
            //        ShowName = x.ShowName,
            //        revSliderProductPresentaionModels= _RevSliderService.PrepareRevSliderProductPresentaionModel(x.Id)
            //    };
                
            //    return model;
            //});


             


            return View("~/Plugins/Widgets.RevolutionSlider/Views/PublicInfo.cshtml", listModel);

        }
    }
}

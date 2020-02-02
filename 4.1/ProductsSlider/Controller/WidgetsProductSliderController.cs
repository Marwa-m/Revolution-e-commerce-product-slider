using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Plugin.Widgets.ProductsSlider.Extensions;
using Nop.Plugin.Widgets.ProductsSlider.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Factories;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Widgets.ProductsSlider.Helpers;

namespace Nop.Plugin.Widgets.ProductsSlider.Controller
{
    [Area(AreaNames.Admin)]
    public class WidgetsProductSliderController : BasePluginController
    {
        #region Fields

        private readonly IProductSliderService _productSliderService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISettingService _settings;

        #endregion
        private IRepository<ProductSlider> _productSliderRepository;
        public WidgetsProductSliderController(IRepository<ProductSlider> productSliderRepository, ILocalizationService localizationService, IPermissionService permissionService, IStoreService storeService, IStoreMappingService storeMappingService, IProductSliderService productSliderService, ILanguageService languageService, ICustomerActivityService customerActivityService, ILocalizedEntityService localizedEntityService
            , IPictureService pictureService, IDateTimeHelper dateTimeHelper, ICurrencyService currencyService, CurrencySettings currencySettings, ICacheManager cacheManager,
            IProductService productService, IStoreContext storeContext,
            IProductModelFactory productModelFactory, IPriceFormatter priceFormatter, IWorkContext workContext, ISettingService settingService)
        {
            this._productSliderRepository = productSliderRepository;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._productSliderService = productSliderService;
            this._languageService = languageService;
            this._customerActivityService = customerActivityService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._dateTimeHelper = dateTimeHelper;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._cacheManager = cacheManager;
            this._productService = productService;
            this._storeContext = storeContext;
            this._productModelFactory = productModelFactory;
            this._priceFormatter = priceFormatter;
            this._workContext = workContext;
            this._settings = settingService;

        }
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(ProductSlider productSlider, ProductSliderModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productSlider,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(productSlider,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
            }

        }
        [NonAction]
        protected virtual void UpdatePictureSeoNames(ProductSlider productSlider)
        {
            var picture = _pictureService.GetPictureById(productSlider.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(productSlider.Name));
        }
        [NonAction]
        protected virtual void PrepareStoresMappingModel(ProductSliderModel model, ProductSlider productSlider, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && productSlider != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(productSlider).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }
        [NonAction]
        protected virtual void PrepareProductSliderModel(ProductSliderModel model, ProductSlider product, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product != null)
            {

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(product.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(product.UpdatedOnUtc, DateTimeKind.Utc);
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;



            //products
            model.AvailableProducts.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.Products.Fields.None"),
                Value = "0"
            });
            var products = ProductsSlider.Helpers.SelectedListHelper.GetProductList(_productSliderService, _cacheManager, true);
            foreach (var v in products)
                model.AvailableProducts.Add(v);

            //default values
            if (setPredefinedValues)
            {
                model.Published = true;
            }

        }

        #endregion
        #region List
        //[AdminAuthorize]
        //[ChildActionOnly]
        public virtual IActionResult List()
        {
            DateTime dtCreate = _settings.GetSettingByKey<DateTime>("ProductSliderdt");
            if (dtCreate.AddDays(7) < DateTime.Now)
            {
                WarningNotification(_localizationService.GetResource("Plugins.Widgets.ProductsSlider.trialEnd"));
            }

            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();
            var model = new ProductSliderListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.UnpublishedOnly"), Value = "2" });
            // return Redirect("/Admin/Widget/ConfigureWidget?systemName=Widgets.RevolutionProductsSlider");
            // return RedirectToRoute(new RouteValueDictionary() { { "Controller", "Widget" },{"Action","ConfigureWidget" },{ "Namespaces", "Nop.Plugin.Widgets.ProductsSlider.Controllers" }, { "systemName", "Widgets.RevolutionProductsSlider" }, { "area", "admin" } });

            //return View(model);
            return View("~/Plugins/Widgets.ProductsSlider/Views/List.cshtml", model);


        }

        [HttpPost]
        public virtual ActionResult ProductSliderList(DataSourceRequest command, ProductSliderListModel model)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedKendoGridJson();


            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var productSliders = _productSliderService.SearchProductSliders(
                Name: model.SearchName,
                storeId: model.SearchStoreId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult();
            gridModel.Data = productSliders.Select(x =>
            {
                var productSliderModel = x.ToModel();
                productSliderModel.ShortDescription = "";

                //picture
                var picture = _pictureService.GetPictureById(x.PictureId);
                productSliderModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(picture, 75, true);
                return productSliderModel;
            });
            gridModel.Total = productSliders.TotalCount;
            return Json(gridModel);
        }
        [NonAction]
        protected virtual void SaveStoreMappings(ProductSlider productSlider, ProductSliderModel model)
        {
            productSlider.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(productSlider);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(productSlider, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        #region Create / Edit / Delete /Settings

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            var model = new ProductSliderModel();
            PrepareProductSliderModel(model, null, true, true);
            //locales
            AddLocales(_languageService, model.Locales);

            //Stores
            PrepareStoresMappingModel(model, null, false);
            return View("~/Plugins/Widgets.ProductsSlider/Views/Create.cshtml", model);

        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductSliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //productSlider
                var productSlider = model.ToEntity();
                productSlider.CreatedOnUtc = DateTime.UtcNow;
                productSlider.UpdatedOnUtc = DateTime.UtcNow;
                _productSliderService.InsertProductSlider(productSlider);
                //locales
                UpdateLocales(productSlider, model);
                //update picture seo file name
                UpdatePictureSeoNames(productSlider);
                //stores
                SaveStoreMappings(productSlider, model);

                _productSliderService.UpdateProductSlider(productSlider);
                //activity log
                _customerActivityService.InsertActivity("AddNewProductSlider", _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ActivityLog.AddNewProductSlider"), productSlider);

                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productSlider.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareStoresMappingModel(model, null, true);
            PrepareProductSliderModel(model, null, false, true);
            return View("~/Plugins/Widgets.ProductsSlider/Views/Create.cshtml",model);
        }
        //edit productSlider
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            var productSlider = _productSliderService.GetProductSliderById(id);
            if (productSlider == null)
                //No productSlider found with the specified id
                return RedirectToAction("List");

            var model = productSlider.ToModel();
            PrepareProductSliderModel(model, productSlider, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = _localizationService.GetLocalized(productSlider,x => x.Name, languageId, false, false);
                locale.ShortDescription = _localizationService.GetLocalized(productSlider,x => x.ShortDescription, languageId, false, false);
            });

            PrepareStoresMappingModel(model, productSlider, false);
            return View("~/Plugins/Widgets.ProductsSlider/Views/Edit.cshtml", model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductSliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            var productSlider = _productSliderService.GetProductSliderById(model.Id);

            if (productSlider == null)
                //No productSlider found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = productSlider.PictureId;

                //productSlider
                productSlider = model.ToEntity(productSlider);

                productSlider.UpdatedOnUtc = DateTime.UtcNow;
                _productSliderService.UpdateProductSlider(productSlider);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != productSlider.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //locales
                UpdateLocales(productSlider, model);
                //stores
                SaveStoreMappings(productSlider, model);
                //picture seo names
                UpdatePictureSeoNames(productSlider);
                _productSliderService.UpdateProductSlider(productSlider);
                //activity log
                _customerActivityService.InsertActivity("EditProductSlider", _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSlider"), productSlider);

                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productSlider.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductSliderModel(model, productSlider, false, true);
            PrepareStoresMappingModel(model, productSlider, true);

            return View("~/Plugins/Widgets.ProductsSlider/Views/Edit.cshtml", model);
        }

        //delete productSlider
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            var productSlider = _productSliderService.GetProductSliderById(id);
            if (productSlider == null)
                //No productSlider found with the specified id
                return RedirectToAction("List");

            _productSliderService.DeleteProductSlider(productSlider);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductSlider", _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ActivityLog.DeleteProductSlider"), productSlider);

            SuccessNotification(_localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _productSliderService.DeleteProductSliders(_productSliderService.GetProductSlidersByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }

        public virtual IActionResult Settings()
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();

            var model = new ProductSliderSettingsModel();
            model.WidgetZoneName = _settings.GetSettingByKey<string>("ProductSliderWidgetZone");
            model.AvailableWidgetZones = SelectedListHelper.GetPublicWidgetZones(model.WidgetZoneName);
            return View("~/Plugins/Widgets.ProductsSlider/Views/Settings.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Settings(ProductSliderSettingsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductsSliderPlugin.ManageProductSliderPermission))
                return AccessDeniedView();



            if (ModelState.IsValid)
            {
                _settings.SetSetting<string>("ProductSliderWidgetZone", model.WidgetZoneName);

                //activity log
                _customerActivityService.InsertActivity("EditProductSliderSettings", _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSliderSettings"));

                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.settingsupdated"));

                if (continueEditing)
                {

                    return RedirectToAction("Settings");
                }
                return RedirectToAction("List");
            }

            return View("~/Plugins/Widgets.ProductsSlider/Views/Settings.cshtml", model);

        }

        #endregion
        #region BaseAdminController
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        //protected virtual ActionResult AccessDeniedView()
        //{
        //    //return new HttpUnauthorizedResult();
        //    return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        //}

        ///// <summary>
        ///// Access denied json data for kendo grid
        ///// </summary>
        ///// <returns>Access denied json data</returns>
        //protected JsonResult AccessDeniedKendoGridJson()
        //{
        //    var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        //    return ErrorForKendoGridJson(localizationService.GetResource("Admin.AccessDenied.Description"));
        //}

        /// <summary>
        /// Save selected TAB name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \Nop.Web.Framework\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = this.Request.Form["selected-tab-name"];
            }

            if (!string.IsNullOrEmpty(tabName))
            {
                const string dataKey = "nop.selected-tab-name";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = tabName;
                }
                else
                {
                    ViewData[dataKey] = tabName;
                }
            }
        }

        #endregion
        #region Ajax
        //ajax
       // [AcceptVerbs(HttpVerbs.Get)]
        public virtual IActionResult GetProductById(string Id)
        {
            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException("productId");

            var product = _productService.GetProductById(Convert.ToInt32(Id));
            var productModel = product.MapTo<Product, ProductModel>();
            int approvedRatingSum = 0;
            int approvedTotalReviews = 0;
            var reviews = product.ProductReviews;
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews++;
                }

            }

            product.ApprovedRatingSum = approvedRatingSum;
            product.ApprovedTotalReviews = approvedTotalReviews;
            ProductProductSliderModel ppS = new ProductProductSliderModel
            {
                Id = product.Id,
                Name = product.Name,
                OldPrice = product.OldPrice,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
            };

            if (product.ApprovedTotalReviews > 0)
                ppS.Preview = product.ApprovedRatingSum / product.ApprovedTotalReviews;
            AddLocales(_languageService, ppS.Locales, (locale, languageId) =>
            {
                locale.Name =_localizationService.GetLocalized(product,x => x.Name, languageId, false, false);
                locale.ShortDescription = _localizationService.GetLocalized(product,x => x.ShortDescription, languageId, false, false);
            });
            return Json(ppS);

        }

        #endregion
       

    }

}

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
using Nop.Core.Domain.Directory;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Extensions;
using Nop.Plugin.Widgets.RevolutionSlider.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Factories;
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
using Nop.Plugin.Widgets.RevolutionSlider.Helpers;
using Nop.Services.Messages;
using Nop.Plugin.Widgets.RevolutionSlider.Factories;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core.Domain.Seo;
using Nop.Web.Framework.Factories;

namespace Nop.Plugin.Widgets.RevolutionSlider.Controller
{
    [Area(AreaNames.Admin)]
   public class RevSliderController: BasePluginController
    {
        #region Fields
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;

        private readonly IRevSliderService _revSliderService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISettingService _settings;
        private readonly INotificationService _notificationService;
        private readonly IRevSliderModelFactory _RevSliderModelFactory;
        protected readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly Nop.Web.Areas.Admin.Factories.ICommonModelFactory _commonModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        public RevSliderController(ICacheKeyService cacheKeyService, IStaticCacheManager staticCacheManager, IRevSliderService revSliderService, 
            IPictureService pictureService, ILanguageService languageService,
            ILocalizationService localizationService, ILocalizedEntityService localizedEntityService, 
            IPermissionService permissionService, IStoreService storeService, 
            IStoreMappingService storeMappingService, IWorkContext workContext,
            ICustomerActivityService customerActivityService, IDateTimeHelper dateTimeHelper,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IProductService productService, IStoreContext storeContext, 
            IProductModelFactory productModelFactory, IPriceFormatter priceFormatter,
            ISettingService settings, INotificationService notificationService, 
            IRevSliderModelFactory revSliderModelFactory, IRepository<ProductReview> productReviewRepository,
            IAclService aclService,ICustomerService customerService, Nop.Web.Areas.Admin.Factories.ICommonModelFactory commonModelFactory,
            IUrlRecordService urlRecordService,ILocalizedModelFactory localizedModelFactory)
        {
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _revSliderService = revSliderService;
            _pictureService = pictureService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _permissionService = permissionService;
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _productService = productService;
            _storeContext = storeContext;
            _productModelFactory = productModelFactory;
            _priceFormatter = priceFormatter;
            _settings = settings;
            _notificationService = notificationService;
            _RevSliderModelFactory = revSliderModelFactory;
            _productReviewRepository = productReviewRepository;
            _aclService = aclService;
            _customerService = customerService;
            _commonModelFactory=commonModelFactory;
            _urlRecordService = urlRecordService;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion
        #region Utilities

        [NonAction]
        protected virtual void PrepareAclModel(RevSliderModel model, RevSlider revSlider, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && revSlider != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(revSlider).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }
        [NonAction]
        protected virtual void SaveRevSliderAcl(RevSlider revSlider, RevSliderModel model)
        {
            revSlider.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(revSlider);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(revSlider, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(RevSlider revSlider, RevSliderModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(revSlider,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }

        }
        [NonAction]
        protected virtual void UpdateLocales(RevSliderProduct revSliderProduct, RevSliderModel.AddRevSliderProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(revSliderProduct,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(revSliderProduct,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
            }

        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(RevSliderProduct revSlider)
        {
            var picture = _pictureService.GetPictureById(revSlider.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(revSlider.Name));
        }
        [NonAction]
        protected virtual void PrepareStoresMappingModel(RevSliderModel model, RevSlider revSlider, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && revSlider != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(revSlider).ToList();

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
        protected virtual void PrepareRevSliderProductModel( RevSliderModel.AddRevSliderProductModel model, RevSliderProduct product, bool setPredefinedValues, bool excludeProperties)
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
                Text = _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.Products.Fields.None"),
                Value = "0"
            });
            var products = RevolutionSlider.Helpers.SelectedListHelper.GetProductList(_revSliderService, _cacheKeyService, _staticCacheManager, true);
            foreach (var v in products)
                model.AvailableProducts.Add(v);

            //default values
            if (setPredefinedValues)
            {
                model.Published = true;
            }

        }

        #endregion
        #region RevSlider
        #region List
        public IActionResult Index()
        {
            return RedirectToAction("~/Plugins/Widgets.RevolutionSlider/Views/List.cshtml");
        }
        public virtual IActionResult List()
        {
            ////Trial Version
            //DateTime dtCreate = _settings.GetSettingByKey<DateTime>("RevSliderdt4.3");
            //if (dtCreate.AddDays(7) < DateTime.Now)
            //{
            //    _notificationService.WarningNotification(_localizationService.GetResource("Plugins.Widgets.RevolutionSlider.trialEnd"));
            //}
            //else
            //{
            //    _notificationService.WarningNotification(string.Format(_localizationService.GetResource("Plugins.Widgets.RevolutionSlider.trialWillEnd"), (7 - (DateTime.Now - dtCreate).Days)));
            //}

            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderSearchModel(new RevSliderSearchModel());

            return View("~/Plugins/Widgets.RevolutionSlider/Views/List.cshtml", model);


        }

        [HttpPost]
        public virtual IActionResult List(RevSliderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedDataTablesJson();
            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderListModel(searchModel);

            return Json(model);

        }

        [NonAction]
        protected virtual void SaveStoreMappings(RevSlider revSlider, RevSliderModel model)
        {
            revSlider.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(revSlider);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(revSlider, store.Id);
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

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderModel(new RevSliderModel(), null);
            //locales
            //  AddLocales(_languageService, model.Locales);

            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //  PrepareRevSliderModel(model, null, true,false);


            return View("~/Plugins/Widgets.RevolutionSlider/Views/Create.cshtml", model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(RevSliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var revSlider = model.ToEntity();
                revSlider.CreatedOnUtc = DateTime.UtcNow;
                revSlider.UpdatedOnUtc = DateTime.UtcNow;
                _revSliderService.InsertRevSlider(revSlider);

                //locales
                UpdateLocales(revSlider, model);
                //discounts
                _revSliderService.UpdateRevSlider(revSlider);
                //ACL (customer roles)
                SaveRevSliderAcl(revSlider, model);
                //Stores
                SaveStoreMappings(revSlider, model);
                _revSliderService.UpdateRevSlider(revSlider);
                //activity log
                _customerActivityService.InsertActivity("AddNewRevSlider", _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.ActivityLog.AddNewRevSlider"), revSlider);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = revSlider.Id });
            }
            ////If we got this far, something failed, redisplay form

            //prepare model
            model = _RevSliderModelFactory.PrepareRevSliderModel(model, null, true);
            ////ACL
            PrepareAclModel(model, null, true);
            ////Stores
            PrepareStoresMappingModel(model, null, true);
            return View("~/Plugins/Widgets.RevolutionSlider/Views/Create.cshtml", model);

        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var revSlider = _revSliderService.GetRevSliderById(id);
            if (revSlider == null || revSlider.Deleted)
                //No revSlider found with the specified id
                return RedirectToAction("List");

            //  var model = revSlider.ToModel();
            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderModel(null, revSlider);
            //locales
            //AddLocales(_languageService, model.Locales, (locale, languageId) =>
            //{
            //    locale.Name = _localizationService.GetLocalized(revSlider,x => x.Name, languageId, false, false);

            //});

            //ACL
            PrepareAclModel(model, revSlider, false);
            //Stores
            PrepareStoresMappingModel(model, revSlider, false);
            return View("~/Plugins/Widgets.RevolutionSlider/Views/Edit.cshtml", model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(RevSliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var revSlider = _revSliderService.GetRevSliderById(model.Id);
            if (revSlider == null || revSlider.Deleted)
                //No revSlider found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                revSlider = model.ToEntity(revSlider);
                revSlider.UpdatedOnUtc = DateTime.UtcNow;
                _revSliderService.UpdateRevSlider(revSlider);
                //locales
                UpdateLocales(revSlider, model);
                //ACL
                SaveRevSliderAcl(revSlider, model);
                //Stores
                SaveStoreMappings(revSlider, model);
                _revSliderService.UpdateRevSlider(revSlider);
                //activity log
                _customerActivityService.InsertActivity("EditRevSlider", _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.ActivityLog.EditRevSlider"), revSlider);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = revSlider.Id });
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //ACL
            PrepareAclModel(model, revSlider, true);
            //Stores
            PrepareStoresMappingModel(model, revSlider, true);
            return View("~/Plugins/Widgets.RevolutionSlider/Views/Edit.cshtml", model);

        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var revSlider = _revSliderService.GetRevSliderById(id);
            if (revSlider == null)
                //No revSlider found with the specified id
                return RedirectToAction("List");

            _revSliderService.DeleteRevSlider(revSlider);

            //activity log
            _customerActivityService.InsertActivity("DeleteRevSlider", _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.ActivityLog.DeleteRevSlider"), revSlider);

            _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Deleted"));
            return RedirectToAction("List");
        }
        #endregion
        #endregion

        #region WidgetZones

        [HttpPost]
        public virtual IActionResult WidgetZoneList(RevSliderWidgetZoneSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var revSlider = _revSliderService.GetRevSliderById(searchModel.RevSliderId)
                ?? throw new ArgumentException("No revSlider found with the specified id");

            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderWidgetZoneListModel(searchModel, revSlider);

            return Json(model);

        }

        public virtual IActionResult WidgetZoneUpdate(RevSliderModel.RevSliderWidgetZoneModel model)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var widgetzoneRevSlider = _revSliderService.GetRevSliderWidgetZoneById(model.Id);
            if (widgetzoneRevSlider == null)
                throw new ArgumentException("No widgetzone revSlider mapping found with the specified id");

            _revSliderService.UpdateRevSliderWidgetZone(widgetzoneRevSlider);

            return new NullJsonResult();
        }

        public virtual IActionResult WidgetZoneDelete(int id)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var widgetzoneRevSlider = _revSliderService.GetRevSliderWidgetZoneById(id);
            if (widgetzoneRevSlider == null)
                throw new ArgumentException("No widget zone revSlider mapping found with the specified id");

            _revSliderService.DeleteRevSliderWidgetZone(widgetzoneRevSlider);

            return new NullJsonResult();
        }

        public virtual IActionResult WidgetZoneAddPopup(int revSliderId)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var model = new RevSliderModel.AddRevSliderWidgetZoneModel();

            model.AvailableWidgetZones = SelectedListHelper.GetPublicWidgetZones();
            return View("~/Plugins/Widgets.RevolutionSlider/Views/WidgetZoneAddPopup.cshtml", model);

        }

        [HttpGet, HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult WidgetZoneAddPopup(RevSliderModel.AddRevSliderWidgetZoneModel model)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            if (model.WidgetZone != null)
            {
                var existingWidgetZoneRevSliders = _revSliderService.GetRevSliderWidgetZonesByRevSliderId(model.RevSliderId, showHidden: true);
                if (existingWidgetZoneRevSliders.FindWidgetZoneRevSlider(model.WidgetZone, model.RevSliderId) == null)
                {
                    _revSliderService.InsertRevSliderWidgetZone(
                        new RevSliderWidgetZone
                        {
                            RevSliderId = model.RevSliderId,
                            WidgetZone = model.WidgetZone,

                                });
                }
            }

            ViewBag.RefreshPage = true;
            return View("~/Plugins/Widgets.RevolutionSlider/Views/WidgetZoneAddPopup.cshtml", model);

        }

        #endregion

        #region Products
        [HttpPost]
        public virtual IActionResult ProductList(RevSliderProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var revSlider = _revSliderService.GetRevSliderById(searchModel.RevSliderId)
                ?? throw new ArgumentException("No revSlider found with the specified id");

            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderProductListModel(searchModel, revSlider);


            return Json(model);
        }


        public virtual IActionResult ProductUpdate(RevSliderModel.RevSliderProductModel model)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var RevSliderProduct = _revSliderService.GetRevSliderProductById(model.Id);
            if (RevSliderProduct == null)
                throw new ArgumentException("No product revSlider mapping found with the specified id");

            RevSliderProduct.DisplayOrder = model.DisplayOrder;
            _revSliderService.UpdateRevSliderProduct(RevSliderProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var RevSliderProduct = _revSliderService.GetRevSliderProductById(id);
            if (RevSliderProduct == null)
                throw new ArgumentException("No product revSlider mapping found with the specified id");

            //var revSliderId = RevSliderProduct.RevSliderId;
            _revSliderService.DeleteRevSliderProduct(RevSliderProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int revSliderId)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var model = new RevSliderModel.AddRevSliderProductModel();
            PrepareRevSliderProductModel(model, null, true, true);
            //locales
            AddLocales(_languageService, model.Locales);


            return View("~/Plugins/Widgets.RevolutionSlider/Views/ProductAddPopup.cshtml", model);

        }

        [HttpGet, HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(RevSliderModel.AddRevSliderProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //RevolutionSlider
                var productSlider = model.ToEntity();
                productSlider.CreatedOnUtc = DateTime.UtcNow;
                productSlider.UpdatedOnUtc = DateTime.UtcNow;
                productSlider.RevSliderId = model.RevSliderId;
                _revSliderService.InsertRevSliderProduct(productSlider);
                //locales
                UpdateLocales(productSlider, model);
                //update picture seo file name
                UpdatePictureSeoNames(productSlider);

                _revSliderService.UpdateRevSliderProduct(productSlider);
                ViewBag.RefreshPage = true;
            }

            //If we got this far, something failed, redisplay form
            PrepareRevSliderProductModel(model, null, false, true);
            
            return View("~/Plugins/Widgets.RevolutionSlider/Views/ProductAddPopup.cshtml", model);

        }
        //[HttpPost]
        //public virtual IActionResult ProductAddPopupList(AddProductToRevSliderSearchModel searchModel)
        //{
        //    if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
        //        return AccessDeniedDataTablesJson();

        //    //prepare model
        //    var model = _RevSliderModelFactory.PrepareAddProductToRevSliderListModel(searchModel);

        //    return Json(model);
        //}

        //[HttpGet, HttpPost]
        //[FormValueRequired("save")]
        //public virtual IActionResult ProductAddPopup(RevSliderModel.AddRevSliderProductModel model)
        //{
        //    if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
        //        return AccessDeniedView();

        //    if (model.SelectedProductIds != null)
        //    {
        //        foreach (var id in model.SelectedProductIds)
        //        {
        //            var product = _productService.GetProductById(id);
        //            if (product != null)
        //            {
        //                var existingRevSliderProducts = _revSliderService.GetRevSliderProductsByRevSliderId(model.RevSliderId, showHidden: true);
        //                if (existingRevSliderProducts.FindRevSliderProduct(id, model.RevSliderId) == null)
        //                {
        //                    _revSliderService.InsertRevSliderProduct(
        //                        new RevSliderProduct
        //                        {
        //                            RevSliderId = model.RevSliderId,
        //                            ProductId = id,
        //                            DisplayOrder = 1
        //                            //RevSliderWidgetZoneId=model.of

        //                        });
        //                }
        //            }
        //        }
        //    }

        //    ViewBag.RefreshPage = true;
        //    return View("~/Plugins/Widgets.RevolutionSlider/Views/ProductAddPopup.cshtml", new AddProductToRevSliderSearchModel());

        //}

        #endregion

        #region UrlRecord
        [HttpPost]
        public virtual IActionResult UrlRecordList(RevSliderUrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var revSlider = _revSliderService.GetRevSliderById(searchModel.RevSliderId)
                ?? throw new ArgumentException("No revSlider found with the specified id");

            //prepare model
            var model = _RevSliderModelFactory.PrepareRevSliderUrlRecordListModel(searchModel, revSlider);

            return Json(model);

        }

        [HttpPost]
        public virtual IActionResult UrlRecordAddPopupList(UrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedDataTablesJson();
            //prepare model
            var model = _commonModelFactory.PrepareUrlRecordSearchModel(new UrlRecordSearchModel());

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult SeNames(UrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                 return AccessDeniedDataTablesJson();
            searchModel.LanguageId = -1;
            searchModel.IsActiveId = 0;
            //prepare model
            var model = _commonModelFactory.PrepareUrlRecordListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult UrlRecordUpdate(RevSliderModel.RevSliderUrlRecordModel model)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var urlRecordRevSlider = _revSliderService.GetRevSliderUrlRecordById(model.Id);
            if (urlRecordRevSlider == null)
                throw new ArgumentException("No widgetzone revSlider mapping found with the specified id");

            _revSliderService.UpdateRevSliderUrlRecord(urlRecordRevSlider);

            return new NullJsonResult();
        }

        public virtual IActionResult UrlRecordDelete(int id)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            var urlRecordRevSlider = _revSliderService.GetRevSliderUrlRecordById(id);
            if (urlRecordRevSlider == null)
                throw new ArgumentException("No widget zone revSlider mapping found with the specified id");

            _revSliderService.DeleteRevSliderUrlRecord(urlRecordRevSlider);

            return new NullJsonResult();
        }

        public virtual IActionResult UrlRecordAddPopup(int revSliderId)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();

            //prepare model
            var urlRecordSearchModel = _commonModelFactory.PrepareUrlRecordSearchModel(new UrlRecordSearchModel());
            RevSliderModel.AddRevSliderUrlRecordModel model = new RevSliderModel.AddRevSliderUrlRecordModel
            {
                RevSliderId = revSliderId,
                urlRecordSearchModel = urlRecordSearchModel,
               
            };

            return View("~/Plugins/Widgets.RevolutionSlider/Views/UrlRecordAddPopup.cshtml", model);

        }

        [HttpGet, HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult UrlRecordAddPopup(RevSliderModel.AddRevSliderUrlRecordModel model)
        {
            if (!_permissionService.Authorize(RevolutionSliderPermissionProvider.ManageRevolutionSliderPermission))
                return AccessDeniedView();


            if (model.SelectedUrlRecordsIds != null)
            {
                foreach (var id in model.SelectedUrlRecordsIds)
                {
                    var urlRecord = _urlRecordService.GetUrlRecordById(id);
                    if (urlRecord != null)
                    {
                        var existingUrlRecords = _revSliderService.GetRevSliderUrlRecordsByRevSliderId(model.RevSliderId, showHidden: true);
                        if (existingUrlRecords.FindRevSliderUrlRecord(id, model.RevSliderId) == null)
                        {
                            _revSliderService.InsertRevSliderUrlRecord(
                                new RevSliderUrlRecord
                                {
                                    RevSliderId = model.RevSliderId,
                                    UrlRecordId=id,
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View("~/Plugins/Widgets.RevolutionSlider/Views/UrlRecordAddPopup.cshtml", model);

        }

        #endregion
        #region Ajax
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
            var reviews = _productReviewRepository.Table.Where(r => r.ProductId == product.Id);

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
            Action<RevSliderModel.AddRevSliderProductLocalizedModel, int> localizedModelConfiguration = null;

            RevSliderModel.AddRevSliderProductModel ppS = new RevSliderModel.AddRevSliderProductModel
            {
                Id = product.Id,
                Name = product.Name,
                OldPrice = product.OldPrice,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                //AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                //AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,


        };
            if (product.ApprovedTotalReviews > 0)
                ppS.Preview = product.ApprovedRatingSum / product.ApprovedTotalReviews;
            AddLocales(_languageService, ppS.Locales, (locale, languageId) =>
            {
                locale.Name = _localizationService.GetLocalized(product, x => x.Name, languageId, false, false);
                locale.ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, languageId, false, false);
            });
            return Json(ppS);

        }

        #endregion

    }
}

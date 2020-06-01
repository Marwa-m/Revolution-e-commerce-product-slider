using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Infrastructure.Cache;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
//using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Services
{
    public partial class RevSliderService : IRevSliderService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<RevSlider> _revSliderRepository;
        private readonly IRepository<RevSliderProduct> _revSliderProductRepository;
        private readonly IRepository<RevSliderWidgetZone> _revSliderWidgetZoneRepository;
        private readonly IRepository<RevSliderUrlRecord> _revSliderUrlRecordRepository;
        private readonly IRepository<UrlRecord> _UrlRecordRepository;

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly IUrlRecordService _urlRecordService;
        public RevSliderService(CatalogSettings catalogSettings, IAclService aclService,
            ICacheKeyService cacheKeyService, ICustomerService customerService, 
            IEventPublisher eventPublisher, ILocalizationService localizationService, 
            IRepository<AclRecord> aclRepository, IRepository<RevSlider> revSliderRepository,
            IRepository<RevSliderProduct> revSliderProductRepository, IProductModelFactory productModelFactory,
            IRepository<RevSliderWidgetZone> revSliderWidgetZoneRepository, 
            IRepository<Product> productRepository,IRepository<UrlRecord> UrlRecordRepository, IRepository<StoreMapping> storeMappingRepository, 
            IStaticCacheManager staticCacheManager, IStoreContext storeContext, IRepository<RevSliderUrlRecord> revSliderUrlRecordRepository,
            IStoreMappingService storeMappingService, IWorkContext workContext, ILanguageService languageService,
            IPriceFormatter priceFormatter,ICurrencyService currencyService, IPictureService pictureService
            , IWebHelper webHelper,IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _aclRepository = aclRepository;
            _revSliderRepository = revSliderRepository;
            _revSliderProductRepository = revSliderProductRepository;
            _revSliderWidgetZoneRepository = revSliderWidgetZoneRepository;
            _productRepository = productRepository;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _revSliderUrlRecordRepository = revSliderUrlRecordRepository;
            _UrlRecordRepository = UrlRecordRepository;
            _languageService = languageService;
            _productModelFactory = productModelFactory;
            _priceFormatter = priceFormatter;
            _currencyService = currencyService;
            _pictureService = pictureService;
            _webHelper = webHelper;
            _urlRecordService = urlRecordService;
        }
        #endregion

        #region RevSlider
        public RevSlider GetRevSliderById(int revSliderId)
        {
            if (revSliderId == 0)
                return null;

            return _revSliderRepository.ToCachedGetById(revSliderId);
        }
        public void InsertRevSlider(RevSlider revSlider)
        {
            if (revSlider == null)
                throw new ArgumentNullException(nameof(revSlider));

            _revSliderRepository.Insert(revSlider);

            //event notification
            _eventPublisher.EntityInserted(revSlider);
        }
        public void UpdateRevSlider(RevSlider revSlider)
        {
            if (revSlider == null)
                throw new ArgumentNullException("revSlider");

            _revSliderRepository.Update(revSlider);


            //event notification
            _eventPublisher.EntityUpdated(revSlider);
        }

        public IPagedList<RevSlider> GetAllRevSliders(string revSliderName = "", int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false
                            , bool? overridePublished = null)
        {
            var query = _revSliderRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(revSliderName))
                query = query.Where(c => c.Name.Contains(revSliderName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            //paging
            return new PagedList<RevSlider>(query, pageIndex, pageSize);
        }
        public void DeleteRevSlider(RevSlider revSlider)
        {
            if (revSlider == null)
                throw new ArgumentNullException(nameof(revSlider));

            revSlider.Deleted = true;
            UpdateRevSlider(revSlider);
            
            //event notification
            _eventPublisher.EntityDeleted(revSlider);
        }
        #endregion
        #region RevSliderProduct
        public void DeleteRevSliderProduct(RevSliderProduct revSliderProduct)
        {
            if (revSliderProduct == null)
                throw new ArgumentNullException(nameof(revSliderProduct));
            _revSliderProductRepository.Delete(revSliderProduct);
            //event notification
            _eventPublisher.EntityDeleted(revSliderProduct);

        }
        public List<ProductDetailsModel> GetProductDetailsByRevSliderId(int revSliderId)
        {
            //if (revSliderId == 0)
            //    return new PagedList<ProductDetailsModel>(new List<ProductDetailsModel>(), p);
            throw new NotImplementedException();
        }
        public RevSliderProduct GetRevSliderProductById(int revSliderProductId)
        {
            if (revSliderProductId == 0)
                return null;

            return _revSliderProductRepository.ToCachedGetById(revSliderProductId);
        }

        public IPagedList<RevSliderProduct> GetRevSliderProductsByRevSliderId(int revSliderId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (revSliderId == 0)
                return new PagedList<RevSliderProduct>(new List<RevSliderProduct>(), pageIndex, pageSize);

            var query = from pc in _revSliderProductRepository.Table
                        join p in _productRepository.Table on pc.ProductId equals p.Id
                        where pc.RevSliderId == revSliderId &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby pc.DisplayOrder, pc.Id
                        select pc;

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from pc in query
                            join c in _revSliderRepository.Table on pc.RevSliderId equals c.Id
                            join acl in _aclRepository.Table
                                on new
                                {
                                    c1 = c.Id,
                                    c2 = nameof(Category)
                                }
                                equals new
                                {
                                    c1 = acl.EntityId,
                                    c2 = acl.EntityName
                                }
                                into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select pc;
                }

                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from pc in query
                            join c in _revSliderRepository.Table on pc.RevSliderId equals c.Id
                            join sm in _storeMappingRepository.Table
                                on new
                                {
                                    c1 = c.Id,
                                    c2 = nameof(Category)
                                }
                                equals new
                                {
                                    c1 = sm.EntityId,
                                    c2 = sm.EntityName
                                }
                                into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || currentStoreId == sm.StoreId
                            select pc;
                }

                query = query.Distinct().OrderBy(pc => pc.DisplayOrder).ThenBy(pc => pc.Id);
            }

            var productCategories = new PagedList<RevSliderProduct>(query, pageIndex, pageSize);

            return productCategories;

        }
        public void InsertRevSliderProduct(RevSliderProduct revSliderProduct)
        {
            if (revSliderProduct == null)
                throw new ArgumentNullException(nameof(revSliderProduct));

            _revSliderProductRepository.Insert(revSliderProduct);

            //event notification
            _eventPublisher.EntityInserted(revSliderProduct);
        }
        public void UpdateRevSliderProduct(RevSliderProduct revSliderProduct)
        {
            if (revSliderProduct == null)
                throw new ArgumentNullException(nameof(revSliderProduct));

            _revSliderProductRepository.Update(revSliderProduct);

            //event notification
            _eventPublisher.EntityUpdated(revSliderProduct);
        }
        public virtual IPagedList<Product> GetAllProducts(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            var query = _productRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Published);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            // var products = query.ToCachedList(GetAllProductsCacheKey);

            // return new products;

            return new PagedList<Product>(query, pageIndex, pageSize);

        }

        #endregion
        #region WidgetZone
        public void UpdateRevSliderWidgetZone(RevSliderWidgetZone revSliderWidgetZone)
        {
            if (revSliderWidgetZone == null)
                throw new ArgumentNullException(nameof(revSliderWidgetZone));

            _revSliderWidgetZoneRepository.Update(revSliderWidgetZone);

            //event notification
            _eventPublisher.EntityUpdated(revSliderWidgetZone);
        }

        public void InsertRevSliderWidgetZone(RevSliderWidgetZone revSliderWidgetZone)
        {
            if (revSliderWidgetZone == null)
                throw new ArgumentNullException(nameof(revSliderWidgetZone));

            _revSliderWidgetZoneRepository.Insert(revSliderWidgetZone);

            //event notification
            _eventPublisher.EntityInserted(revSliderWidgetZone);
        }

        public void DeleteRevSliderWidgetZone(RevSliderWidgetZone revSliderWidgetZone)
        {
            if (revSliderWidgetZone == null)
                throw new ArgumentNullException(nameof(revSliderWidgetZone));
            _revSliderWidgetZoneRepository.Delete(revSliderWidgetZone);
            //event notification
            _eventPublisher.EntityDeleted(revSliderWidgetZone);
        }

        

        public RevSliderWidgetZone GetRevSliderWidgetZoneById(int RevSliderWidgetZoneId)
        {
            if (RevSliderWidgetZoneId == 0)
                return null;

            return _revSliderWidgetZoneRepository.GetById(RevSliderWidgetZoneId);
        }

        public List<string> GetRevSliderWidgetZones()
        {
            return _revSliderWidgetZoneRepository.Table.ToList().Select(x => x.WidgetZone).ToList();
        }

        public IPagedList<RevSliderWidgetZone> GetRevSliderWidgetZonesByRevSliderId(int revSliderId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (revSliderId == 0)
                return new PagedList<RevSliderWidgetZone>(new List<RevSliderWidgetZone>(), pageIndex, pageSize);

            var query = from po in _revSliderWidgetZoneRepository.Table
                        where po.RevSliderId == revSliderId
                        orderby po.Id
                        select po;


            var productRevSliders = new PagedList<RevSliderWidgetZone>(query, pageIndex, pageSize);
            return productRevSliders;

        }

        #endregion

        #region UrlRecord
        public void UpdateRevSliderUrlRecord(RevSliderUrlRecord revSliderUrlRecord)
        {
            if (revSliderUrlRecord == null)
                throw new ArgumentNullException(nameof(revSliderUrlRecord));

            _revSliderUrlRecordRepository.Update(revSliderUrlRecord);

            //event notification
            _eventPublisher.EntityUpdated(revSliderUrlRecord);
        }

        public void InsertRevSliderUrlRecord(RevSliderUrlRecord revSliderUrlRecord)
        {
            if (revSliderUrlRecord == null)
                throw new ArgumentNullException(nameof(revSliderUrlRecord));

            _revSliderUrlRecordRepository.Insert(revSliderUrlRecord);

            //event notification
            _eventPublisher.EntityInserted(revSliderUrlRecord);
        }

        public void DeleteRevSliderUrlRecord(RevSliderUrlRecord revSliderUrlRecord)
        {
            if (revSliderUrlRecord == null)
                throw new ArgumentNullException(nameof(revSliderUrlRecord));
            _revSliderUrlRecordRepository.Delete(revSliderUrlRecord);
            //event notification
            _eventPublisher.EntityDeleted(revSliderUrlRecord);
        }

        public IList<RevSlider> GetRevSlidersByUrlRecordName(string WidgetName = "")
        {
            throw new NotImplementedException();
        }

        public RevSliderUrlRecord GetRevSliderUrlRecordById(int RevSliderUrlRecordId)
        {
            if (RevSliderUrlRecordId == 0)
                return null;

            return _revSliderUrlRecordRepository.GetById(RevSliderUrlRecordId);
        }

        public List<UrlRecord> GetRevSliderUrlRecords()
        {
            return _revSliderUrlRecordRepository.Table.ToList().Select(x => x.UrlRecord).ToList();
        }

        public IPagedList<RevSliderModel.RevSliderUrlRecordModel> GetRevSliderUrlRecordsByRevSliderId(int revSliderId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (revSliderId == 0)
                return new PagedList<RevSliderModel.RevSliderUrlRecordModel>(new List<RevSliderModel.RevSliderUrlRecordModel>(), pageIndex, pageSize);

            var query = from po in _revSliderUrlRecordRepository.Table
                        where po.RevSliderId == revSliderId
                        orderby po.Id
                        select po;
            var RevSliderUrlRecords1 = query.ToList();
            var RevSliderUrlRecords = new List<RevSliderModel.RevSliderUrlRecordModel>();
            foreach (var item in RevSliderUrlRecords1)
            {
                var urlRecord = _UrlRecordRepository.GetById(item.UrlRecordId);
                if (urlRecord != null)
                { 
                RevSliderUrlRecords.Add(new RevSliderModel.RevSliderUrlRecordModel
                {
                    UrlRecordId = item.UrlRecordId,
                    Id = item.Id,
                    RevSliderId = item.RevSliderId,
                    UrlRecordName = urlRecord.Slug,
                    EntityName = urlRecord.EntityName,
                    IsActive = urlRecord.IsActive,
                    Language = urlRecord.LanguageId == 0
                        ? _localizationService.GetResource("Admin.System.SeNames.Language.Standard")
                        : _languageService.GetLanguageById(urlRecord.LanguageId)?.Name ?? "Unknown"

                });
                 }
            }
           var  RevSliderUrlRecordPage = new PagedList<RevSliderModel.RevSliderUrlRecordModel>(RevSliderUrlRecords, pageIndex, pageSize);
            return RevSliderUrlRecordPage;

        }

        #endregion


        public IList<RevSlider> GetRevSlidersByWidgetZoneName(string widgetName = "")
        {
            var currentPageUrl = _webHelper.GetThisPageUrl(false);
            var uri = new Uri(currentPageUrl);
            var urlRecord = _urlRecordService.GetBySlug(uri.LocalPath.TrimStart('/'));
    //        var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_KEY,
    //_workContext.WorkingLanguage.Id,
    //           _workContext.CurrentCustomer.Id,
    //           _storeContext.CurrentStore.Id,currentPageUrl);
    //        return _staticCacheManager.Get(cacheKey, () =>
    //        {
                var query = from of in _revSliderRepository.Table
                           join wz in _revSliderWidgetZoneRepository.Table on of.Id equals wz.RevSliderId
                            where wz.WidgetZone.Equals(widgetName) && of.Published == true
                            select of;
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowDateUtc = DateTime.UtcNow.Date;
                var nowTimeUtc = DateTime.UtcNow.TimeOfDay;

                //available dates
                query = query.Where(p =>
                    (!p.FromDate.HasValue || p.FromDate.Value <= nowDateUtc) &&
                    (!p.ToDate.HasValue || p.ToDate.Value >= nowDateUtc));
                query = query.Where(p =>
                    (!p.FromTimeUtc.HasValue || p.FromTimeUtc.Value < nowTimeUtc) &&
                    (!p.ToTimeUtc.HasValue || p.ToTimeUtc.Value > nowTimeUtc));

                if ((_storeContext.CurrentStore.Id > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!_catalogSettings.IgnoreAcl))
                {
                    if ( !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                        query = from c in query
                                join acl in _aclRepository.Table
                                    on new { c1 = c.Id, c2 = nameof(RevSlider) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (_storeContext.CurrentStore.Id > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from bp in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = bp.Id, c2 = nameof(RevSlider) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                                from sm in bp_sm.DefaultIfEmpty()
                                where !bp.LimitedToStores || _storeContext.CurrentStore.Id == sm.StoreId
                                select bp;

                    }

                }
                
                var revSliderList = query.ToList();
                var deletedList = new List<RevSlider>();
                foreach (var item in revSliderList)
                {
                        var RevUrlList = GetRevSliderUrlRecordsByRevSliderId(item.Id);
                        if (RevUrlList != null && RevUrlList.Count > 0)
                        {
                            if(urlRecord==null)
                            {
                                deletedList.Add(item);
                        continue;
                              //  break;
                            }
                           else if (!RevUrlList.Select(x => x.UrlRecordId).Contains(urlRecord.Id))
                            {
                                deletedList.Add(item);
                        continue;
                    }
                        }
                    switch (item.SchedulePatternId)
                    {
                        case (int)SchedulePattern.EveryDay:
                            break;
                        case (int)SchedulePattern.EveryMonth:
                            if (DateTime.Now.Day != 1)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.FromMondayToFriday:
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.FromSundayToThursday:
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnEvenDay:
                            if (DateTime.Now.Day % 2 != 0)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnFriday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnFridayAndSaturday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnMonday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnOddDay:
                            if (DateTime.Now.Day % 2 == 0)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSaturday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSaturdayAndSunday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSunday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnThursday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Thursday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnThursdayAndFriday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Thursday && DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnTuesday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Tuesday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnWednesday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Wednesday)
                                deletedList.Add(item);
                            break;

                    }

                }
                foreach (var s in deletedList)
                    revSliderList.Remove(s);


                return revSliderList;
            //});

        }
        public IList<RevSliderProductPresentaionModel> PrepareRevSliderProductPresentaionModel(int revSliderId)
        {
            if (revSliderId == 0)
                return new List<RevSliderProductPresentaionModel>();

    //        var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_KEY,
    //_workContext.WorkingLanguage.Id,
    //           _workContext.CurrentCustomer.Id,
    //           _storeContext.CurrentStore.Id);
    //        return _staticCacheManager.Get(cacheKey, () =>
    //        {
                var query = from po in _revSliderProductRepository.Table
                            where po.RevSliderId == revSliderId && po.Published==true
                            orderby po.DisplayOrder, po.Id
                            select po;


                var revproducts = query.ToList();
            var listModel = new List<RevSliderProductPresentaionModel>();
            if(revproducts!=null && revproducts.Count>0)
            {
                var productPresentationModel = new RevSliderProductPresentaionModel();
                foreach (var x in revproducts)
                {
                    productPresentationModel = new RevSliderProductPresentaionModel();
                    Product product = _productRepository.GetById(x.ProductId);
                    productPresentationModel = new RevSliderProductPresentaionModel();
                    productPresentationModel.Id = x.Id;
                    productPresentationModel.Name = _localizationService.GetLocalized(x, l => l.Name);;
                    productPresentationModel.IsExternal = x.IsExternal;
                    productPresentationModel.Link = x.Link;
                    productPresentationModel.Preview = x.Preview;
                    productPresentationModel.ProductId = x.ProductId;
                    productPresentationModel.RevSliderId = revSliderId;
                    productPresentationModel.ShortDescription = _localizationService.GetLocalized(x, l => l.ShortDescription);
                    productPresentationModel.OldPrice = (x.OldPrice > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.OldPrice, _workContext.WorkingCurrency)) : "");
                        productPresentationModel.Price = (x.Price > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.Price, _workContext.WorkingCurrency)) : "");
                        productPresentationModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 0, true);
                    productPresentationModel.ProductOverviewModel = _productModelFactory.PrepareProductOverviewModels(new[] { product }.AsEnumerable<Product>()).FirstOrDefault();
                    listModel.Add(productPresentationModel);
                }
                
            }
                //var prodModels = products.Select(x =>
                //{
                //    Product product = _productRepository.GetById(x.ProductId);

                //   // var pOverviewModel = _productModelFactory.PrepareProductOverviewModels(new[] { product }.AsEnumerable<Product>()).FirstOrDefault();
                //    var productPresentationModel = new RevSliderProductPresentaionModel
                //    {
                //        Id = x.Id,
                //        Name = x.Name,
                //        IsExternal = x.IsExternal,
                //        Link = x.Link,
                //        Preview = x.Preview,
                //        ProductId = x.ProductId,
                //        RevSliderId = revSliderId,
                //        ShortDescription = _localizationService.GetLocalized(x, l => l.ShortDescription),
                //        OldPrice = (x.OldPrice > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.OldPrice, _workContext.WorkingCurrency)) : ""),
                //        Price = (x.Price > 0 ? _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(x.Price, _workContext.WorkingCurrency)) : ""),
                //        PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 0, true),
                //        ProductOverviewModel= _productModelFactory.PrepareProductOverviewModels(new[] { product }.AsEnumerable<Product>()).FirstOrDefault(),

                //};
                //    return productPresentationModel;
                    
                //});
                return listModel;
            //});
        }

    }
}

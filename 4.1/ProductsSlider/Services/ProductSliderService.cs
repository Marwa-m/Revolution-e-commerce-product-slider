using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.ProductsSlider.Services
{
    public partial class ProductSliderService : IProductSliderService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTSLIDER_BY_ID_KEY = "Nop.productslider.id-{0}";

        private const string ProductSliders_PATTERN_KEY = "Nop.productslider.";
        public const string ProductSliders_Presentation_KEY = "Nop.pres.productsliders{0}-{1}";

        #endregion
        #region Fields
        private readonly ILanguageService _languageService;
        private readonly CommonSettings _commonSettings;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly IRepository<ProductSlider> _productSliderRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Product> _productRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;
        private readonly string _entityName;
        #endregion
        public ProductSliderService(ILanguageService languageService, CommonSettings commonSettings,IDataProvider dataProvider, CatalogSettings catalogSettings, IDbContext dbContext,
            IRepository<ProductSlider> productSliderRepository, IWorkContext workContext,
            IStoreMappingService storeMappingService, IRepository<StoreMapping> storeMappingRepository
            , IEventPublisher eventPublisher,ICacheManager cacheManager, IRepository<Product> productRepository,
            IStoreContext storeContext
            )
        {
            this._languageService = languageService;
            this._commonSettings = commonSettings;
            this._dataProvider = dataProvider;
            this._catalogSettings = catalogSettings;
            this._dbContext = dbContext;
            this._productSliderRepository = productSliderRepository;
            this._workContext = workContext;
            this._storeMappingService = storeMappingService;
            this._storeMappingRepository =storeMappingRepository ;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
            this._productRepository = productRepository;
            _storeContext = storeContext;
            _entityName = typeof(ProductSlider).Name;

        }
        public void DeleteProductSlider(ProductSlider productSlider)
        {
            if (productSlider == null)
                throw new ArgumentNullException("productSlider");
            _productSliderRepository.Delete(productSlider);

            //event notification
            _eventPublisher.EntityDeleted(productSlider);

        }
        public void DeleteProductSliders(IList<ProductSlider> productSliders)
        {
            if (productSliders == null)
                throw new ArgumentNullException("productSliders");

            foreach (var productSlider in productSliders)
            {
                _productSliderRepository.Delete(productSlider);
            }

            foreach (var productSlider in productSliders)
            {
                //event notification
                _eventPublisher.EntityDeleted(productSlider);
            }
        }


        public ProductSlider GetProductSliderById(int productSliderId)
        {
            if (productSliderId == 0)
                return null;

            string key = string.Format(PRODUCTSLIDER_BY_ID_KEY, productSliderId);
            return _cacheManager.Get(key, () => _productSliderRepository.GetById(productSliderId));
        }

        public void InsertProductSlider(ProductSlider productSlider)
        {
            if (productSlider == null)
                throw new ArgumentNullException("productSlider");

            _productSliderRepository.Insert(productSlider);

            //cache
            _cacheManager.RemoveByPattern(ProductSliders_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productSlider);
        }
        public virtual IPagedList<ProductSlider> SearchProductSliders(string Name = "", int storeId = 0,
            int pageIndex = 0, int pageSize = 0, bool showHidden = false,bool? overridePublished=null)
        {
            var query = _productSliderRepository.Table;
            if (!overridePublished.HasValue)
            {
                //process according to "showHidden"
                if (!showHidden)
                {
                    query = query.Where(p => p.Published);
                }
            }
            else if (overridePublished.Value)
            {
                //published only
                query = query.Where(p => p.Published);
            }
            else if (!overridePublished.Value)
            {
                //unpublished only
                query = query.Where(p => !p.Published);
            }
            if (!String.IsNullOrWhiteSpace(Name))
                query = query.Where(c => c.Name.Contains(Name));

            if (storeId > 0 )
            { 
                //Store mapping
                var storeQry = from sm in _storeMappingRepository.Table
                               where (sm.EntityName == _entityName && _storeContext.CurrentStore.Id == sm.StoreId)
                               select sm.EntityId;
                var storeMap = storeQry.ToList();

                //Store mapping
                query = from g in query
                        where !g.LimitedToStores || storeMap.Contains(g.Id)
                        select g;
            }


            var a = query.ToList();
            //paging
            return new PagedList<ProductSlider>(a, pageIndex, pageSize);

        }


        public void UpdateProductSlider(ProductSlider productSlider)
        {
            if (productSlider == null)
                throw new ArgumentNullException("productSlider");

            _productSliderRepository.Update(productSlider);

            //cache
            _cacheManager.RemoveByPattern(ProductSliders_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productSlider);

        }

        /// Gets all products
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
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

            var products = new PagedList<Product>(query, pageIndex, pageSize);
            return products;
        }

        public IList<ProductSlider> GetProductSlidersByIds(int[] productSliderIds)
        {
            if (productSliderIds == null || productSliderIds.Length == 0)
                return new List<ProductSlider>();

            var query = from p in _productSliderRepository.Table
                        where productSliderIds.Contains(p.Id)
                        select p;
            var productSliders = query.ToList();
            //sort by passed identifiers
            var sortedProductSliders = new List<ProductSlider>();
            foreach (int id in productSliderIds)
            {
                var productSlider = productSliders.Find(x => x.Id == id);
                if (productSlider != null)
                    sortedProductSliders.Add(productSlider);
            }
            return sortedProductSliders;
        }

        public virtual List<ProductSlider> GetProductSliderForShowing(int storeId)
        {
            //  var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            var query = _productSliderRepository.Table.Where(c => c.Published);
            //available dates
            query = query.Where(p =>
                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value <= DateTime.UtcNow) &&
                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value >= DateTime.UtcNow));

            if (_storeContext.CurrentStore.Id > 0)
            {

                //Store mapping
                var storeQry = from sm in _storeMappingRepository.Table
                               where (sm.EntityName == _entityName && _storeContext.CurrentStore.Id == sm.StoreId)
                               select sm.EntityId;
                var storeMap = storeQry.ToList();

                //Store mapping
                query = from g in query
                        where !g.LimitedToStores || storeMap.Contains(g.Id)
                        select g;
            }

           query= query.OrderBy(x => x.DisplayOrder);
            var a = query.ToList();

            return a;
        }

    }
}

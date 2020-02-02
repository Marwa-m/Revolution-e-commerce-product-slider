using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.ProductsSlider.Infrastructure.Cache
{
    public partial class ModelCacheEventConsumer :
    IConsumer<EntityInsertedEvent<ProductSlider>>,
    IConsumer<EntityUpdatedEvent<ProductSlider>>,
    IConsumer<EntityDeletedEvent<ProductSlider>>
    {
        /// <summary>
        /// Key for productsliders caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>

        public const string PRODUCTSLIDERS_BY_WIDGET_ZONE_KEY = "Nop.pres.productsliders.bywidgetzone-{0}-{1}-{2}";
        public const string PRODUCTSLIDERS_BY_WIDGET_ZONE_PATTERN_KEY = "Nop.pres.productsliders.bywidgetzone";
        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<ProductSlider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCTSLIDERS_BY_WIDGET_ZONE_PATTERN_KEY);

        }
        public void HandleEvent(EntityUpdatedEvent<ProductSlider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCTSLIDERS_BY_WIDGET_ZONE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductSlider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCTSLIDERS_BY_WIDGET_ZONE_PATTERN_KEY);
        }
    }

}

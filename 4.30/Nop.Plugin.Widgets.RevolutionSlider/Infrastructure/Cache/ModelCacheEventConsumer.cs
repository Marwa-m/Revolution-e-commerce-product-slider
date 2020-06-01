using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Infrastructure.Cache
{

    public partial class ModelCacheEventConsumer :
    IConsumer<EntityInsertedEvent<RevSlider>>,
    IConsumer<EntityUpdatedEvent<RevSlider>>,
    IConsumer<EntityDeletedEvent<RevSlider>>,
        IConsumer<EntityInsertedEvent<Setting>>,
        IConsumer<EntityUpdatedEvent<Setting>>,
        IConsumer<EntityDeletedEvent<Setting>>
    {
        private readonly IStaticCacheManager _staticCacheManager;

        /// <summary>
        /// Key for productsliders caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>



        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        /// <summary>
        /// Gets a key for caching
        /// </summary>





        public void HandleEvent(EntityInsertedEvent<RevSlider> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);

        }
        public void HandleEvent(EntityUpdatedEvent<RevSlider> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<RevSlider> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);
        }
        public void HandleEvent(EntityInsertedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);

        }
        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(RevSliderModelCacheDefaults.PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);
        }
    }

}

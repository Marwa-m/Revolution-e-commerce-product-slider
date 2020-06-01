using Nop.Core.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Infrastructure.Cache
{
  public static partial  class RevSliderModelCacheDefaults
    {
        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey ProductsListKey => new CacheKey("Nop.pres.admin.products.list-{0}", ProductsListPrefixCacheKey);
        public static string ProductsListPrefixCacheKey => "Nop.pres.admin.products.list";

        /// Key for productsliders caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>

        public const string PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey = "Nop.pres.RevSlider.bywidgetzone";
        public static CacheKey PRODUCTSLIDERS_BY_WIDGET_ZONE_KEY = new CacheKey("Nop.plugin.RevSlider.bywidgetzone-{0}-{1}-{2}-{3}", PRODUCTSLIDERS_BY_WIDGET_ZONE_PrefixCacheKey);

    }
}

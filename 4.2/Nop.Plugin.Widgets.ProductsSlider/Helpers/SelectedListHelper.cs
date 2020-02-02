using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.ProductsSlider.Services;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.ProductsSlider.Helpers
{
   public static class SelectedListHelper
    {
        public static List<SelectListItem> GetProductList(IProductSliderService productSliderService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (productSliderService == null)
                throw new ArgumentNullException("productSliderService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");
            var cacheKey = string.Format(NopModelCacheDefaults.VendorsListKey, showHidden);

            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var products = productSliderService.GetAllProducts(showHidden: showHidden);
                return products.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                    
                });
            }

            return result;
        }


        public static List<SelectListItem> GetPublicWidgetZones(string widgetzone)
        {
            
            var t = typeof(PublicWidgetZones);
            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in t.GetProperties())
            {
                if(item.Name.ToLower()==widgetzone.ToLower())
                {
                    result.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.GetValue(null).ToString(),
                        Selected=true
                    });
                }
                else
                {
                    result.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.GetValue(null).ToString()
                    });
                }
                
            }

            return result;
        }

    }
}

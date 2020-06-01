using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.RevolutionSlider.Infrastructure.Cache;
using Nop.Plugin.Widgets.RevolutionSlider.Services;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.RevolutionSlider.Helpers
{
   public static class SelectedListHelper
    {

        public static List<SelectListItem> GetProductList(IRevSliderService revSliderService,ICacheKeyService _cacheKeyService, IStaticCacheManager _staticCacheManager,  bool showHidden = false)
        {
            if (revSliderService == null)
                throw new ArgumentNullException("revSliderService");

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(RevSliderModelCacheDefaults.ProductsListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var products = revSliderService.GetAllProducts(showHidden: showHidden);
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

        public static List<SelectListItem> GetPublicWidgetZones()
        {

            var t = typeof(PublicWidgetZones);
            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in t.GetProperties())
            {
                result.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.GetValue(null).ToString()
                });

            }

            return result;
        }

    }
}

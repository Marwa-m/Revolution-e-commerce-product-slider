using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
   public class ProductSliderSettingsModel
    {
        public ProductSliderSettingsModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Fields.WidgetZoneName")]
        public string WidgetZoneName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Fields.WidgetZoneName")]
        public IList<SelectListItem> AvailableWidgetZones { get; set; }

    }
}

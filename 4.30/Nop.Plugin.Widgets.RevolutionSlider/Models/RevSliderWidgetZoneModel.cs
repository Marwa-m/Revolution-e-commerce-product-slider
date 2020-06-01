using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Models
{
    public partial class RevSliderWidgetZoneModel : BaseNopEntityModel
    {
        public RevSliderWidgetZoneModel()
        {
            WidgetZones = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.WidgetZone")]
        public string WidgetZone { get; set; }
        public IList<SelectListItem> WidgetZones { get; set; }
    }

}

using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Models
{
   public partial class RevSliderUrlRecordModel : BaseNopEntityModel
    {
        public RevSliderUrlRecordModel()
        {
            UrlRecords = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.UrlRecords.Fields.UrlRecord")]
        public string UrlRecord { get; set; }
        public IList<SelectListItem> UrlRecords { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.RevolutionSlider.Data
{
    public partial class RevSliderSearchModel : BaseSearchModel
    {
        #region Ctor

        public RevSliderSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchRevSliderName")]
        public string SearchName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }

}

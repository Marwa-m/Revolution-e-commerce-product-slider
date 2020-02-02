using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    public partial class ProductSliderSearchModel : BaseSearchModel
    {
        #region Ctor

        public ProductSliderSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchProductSliderName")]
        public string SearchName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }

}

using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    public partial class ProductSliderListModel : BaseNopModel
    {
        public ProductSliderListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchProductSliderName")]
        public string SearchName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }

}

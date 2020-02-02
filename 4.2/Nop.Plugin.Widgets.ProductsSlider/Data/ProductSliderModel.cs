using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    public partial class ProductSliderModel : BaseNopEntityModel, ILocalizedModel<ProductSliderLocalizedModel>
    {
        public ProductSliderModel()
        {
            Locales = new List<ProductSliderLocalizedModel>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableProducts = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ID")]
        public int ProductSliderId { get; set; }
        //store mapping
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.LimitedToStore")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.PictureThumbnailUrl")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Published")]
        public bool Published { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ProductId")]
        public int ProductId { get; set; }
        public IList<SelectListItem> AvailableProducts { get; set; }
        public ProductOverviewModel productOverviewModel {get;set;}
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.OldPrice")]
        public decimal OldPrice { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Link")]
        public string Link { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.IsExternal")]
        public bool IsExternal { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Preview")]
        public decimal Preview { get; set; }
        public IList<ProductSliderLocalizedModel> Locales { get; set; }
        //picture thumbnail
        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableStartDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableEndDateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }


    }
    public partial class ProductSliderLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

    }

}

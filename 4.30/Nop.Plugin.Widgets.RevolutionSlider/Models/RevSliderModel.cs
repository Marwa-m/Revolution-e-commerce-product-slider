using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Plugin.Widgets.RevolutionSlider.Models
{
        public partial class RevSliderModel : BaseNopEntityModel, ILocalizedModel<RevSliderLocalizedModel>
    {
        public RevSliderModel()
        {

            Locales = new List<RevSliderLocalizedModel>();
            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableSchedulePatterns = new List<SelectListItem>();
            RevSliderProductSearchModel = new RevSliderProductSearchModel();

        }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShowName")]
        public bool ShowName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.CustomStyle")]
        public string CustomStyle { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.HideOnMobile")]
        public bool HideOnMobile { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.DisplayOrder")]
      
        public int DisplayOrder { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromDate")]
        [UIHint("DateNullable")]
        public DateTime? FromDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToDate")]
        [UIHint("DateNullable")]
        public DateTime? ToDate { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromTime")]
        [UIHint("~.Plugins.Widgets.RevolutionSlider.Views.Shared.EditorTemplates.TimeNullable")]
        public TimeSpan? FromTimeUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToTime")]
        [UIHint("Plugins.Widgets.RevolutionSlider.Views.Shared.EditorTemplates.TimeNullable")]
        public TimeSpan? ToTimeUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.SchedulePattern")]
        public int SchedulePatternId { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.SchedulePattern")]
        public string SchedulePatternIdName { get; set; }
        public IList<RevSliderLocalizedModel> Locales { get; set; }
        //ACL (customer roles)
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AclCustomerRole")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.LimitedToStore")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableSchedulePatterns { get; set; }

        public RevSliderProductSearchModel RevSliderProductSearchModel { get; set; }

        #region setting slider
     
        #endregion
        #region Nested classes
        public partial class RevSliderProductModel : BaseNopEntityModel
        {
            public int RevSliderId { get; set; }

            public int ProductId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSliderProduct.Fields.Product")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSliderProduct.Fields.RevSlider")]
            public string RevSliderName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSliderProduct.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class RevSliderWidgetZoneModel : BaseNopEntityModel
        {
            public int RevSliderId { get; set; }

            public int WidgetZoneId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.WidgetZone")]
            public string WidgetZoneName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.RevSlider")]
            public string RevSliderName { get; set; }

        }
        public partial class AddRevSliderWidgetZoneModel : BaseNopModel
        {
            public AddRevSliderWidgetZoneModel()
            {
                AvailableWidgetZones = new List<SelectListItem>();
            }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.WidgetZone")]
            public string WidgetZone { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            public IList<SelectListItem> AvailableWidgetZones { get; set; }

            public int RevSliderId { get; set; }

        }

        public partial class AddRevSliderProductModel : BaseNopEntityModel, ILocalizedModel<AddRevSliderProductLocalizedModel>
        {
            public AddRevSliderProductModel()
            {
                Locales = new List<AddRevSliderProductLocalizedModel>();
                AvailableProducts = new List<SelectListItem>();
            }



            [UIHint("Picture")]
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.PictureThumbnailUrl")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Published")]
            public bool Published { get; set; }

            public DateTime? CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ProductId")]
            public int ProductId { get; set; }
            public IList<SelectListItem> AvailableProducts { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShortDescription")]
            public string ShortDescription { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Price")]
            public decimal Price { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.OldPrice")]
            public decimal OldPrice { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Link")]
            public string Link { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.IsExternal")]
            public bool IsExternal { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Preview")]
            public decimal Preview { get; set; }
            public IList<AddRevSliderProductLocalizedModel> Locales { get; set; }
            //picture thumbnail
            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.PictureThumbnailUrl")]
            public string PictureThumbnailUrl { get; set; }


            public string PrimaryStoreCurrencyCode { get; set; }

            public int RevSliderId { get; set; }
        }
        public partial class AddRevSliderProductLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name")]
            public string Name { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShortDescription")]
            public string ShortDescription { get; set; }

        }
        public partial class RevSliderUrlRecordModel : BaseNopEntityModel
        {
            public int RevSliderId { get; set; }

            public int UrlRecordId { get; set; }

            public string UrlRecordName { get; set; }
            public string EntityName { get; set; }
            public bool IsActive { get; set; }

            public string Language { get; set; }
        }
        public partial class AddRevSliderUrlRecordModel : BaseNopModel
        {
            public AddRevSliderUrlRecordModel()
            {
                urlRecordSearchModel =new UrlRecordSearchModel();
            }

            public UrlRecordSearchModel urlRecordSearchModel { get; set; }

            public int[] SelectedUrlRecordsIds { get; set; }
            public int RevSliderId { get; set; }

        }

        #endregion
    }
    public partial class RevSliderLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name")]
        public string Name { get; set; }


    }

}

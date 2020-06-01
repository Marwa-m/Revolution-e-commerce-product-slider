using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.RevolutionSlider.Models
{
    public partial class RevSliderProductPresentaionModel : BaseNopEntityModel
    {
        public RevSliderProductPresentaionModel()
        {
            ProductOverviewModel = new ProductOverviewModel();
        }

         public ProductOverviewModel ProductOverviewModel { get; set; }
        public int RevSliderId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }

        public string Link { get; set; }
        public bool IsExternal { get; set; }
        public decimal Preview { get; set; }
        //picture thumbnail
        public string PictureThumbnailUrl { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        /// <summary>
        /// The currency (in 3-letter ISO 4217 format) of the offer price 
        /// </summary>
        public string CurrencyCode { get; set; }

        public string OldPrice { get; set; }

        public string Price
        {
            get; set;

        }

    }

}

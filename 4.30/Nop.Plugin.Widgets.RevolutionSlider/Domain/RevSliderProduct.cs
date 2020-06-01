using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
   public class RevSliderProduct: BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }
        ///// <summary>
        ///// <summary>
        ///// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        ///// </summary>
        //public bool LimitedToStores { get; set; }
        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }

        /// <summary>
        /// Gets or sets a display order.
        /// This value is used when sorting associated productSliders (used with "grouped" products)
        /// This value is used when sorting home page productSliders
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// Gets or sets the productSlider identifier
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Gets or sets the date and time of productSlider creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of productSlider update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the link
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Gets or sets the IsExternal
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Gets or sets the Preview
        /// </summary>
        public decimal Preview { get; set; }


        ///// <summary>
        ///// Gets or sets the available start date and time
        ///// </summary>
        //public DateTime? AvailableStartDateTimeUtc { get; set; }
        ///// <summary>
        ///// Gets or sets the available end date and time
        ///// </summary>
        //public DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int RevSliderId { get; set; }

        /// <summary>
        /// Gets the slider
        /// </summary>
        public virtual RevSlider RevSlider { get; set; }

    }
}

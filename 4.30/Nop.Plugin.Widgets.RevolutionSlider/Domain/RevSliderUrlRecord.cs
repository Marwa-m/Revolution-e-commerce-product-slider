using Nop.Core;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
   public partial class RevSliderUrlRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the RevSlider identifier
        /// </summary>
        public int RevSliderId { get; set; }

        /// <summary>
        /// Gets the revSlider
        /// </summary>
        public virtual RevSlider RevSlider { get; set; }

        /// <summary>
        /// Gets or sets the RevSlider identifier
        /// </summary>
        public int UrlRecordId { get; set; }

        /// <summary>
        /// Gets the revSlider
        /// </summary>
        public virtual UrlRecord UrlRecord { get; set; }

    }
}

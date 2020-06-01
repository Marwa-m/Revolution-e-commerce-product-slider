using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
    public partial class RevSliderWidgetZone : BaseEntity
    {
        private ICollection<RevSliderWidgetZone> _revSliderWidgetZones;


        /// <summary>
        /// Gets or sets the Widget Zone
        /// </summary>
        public string WidgetZone { get; set; }

        /// <summary>
        /// Gets or sets the RevSlider identifier
        /// </summary>
        public int RevSliderId { get; set; }

        /// <summary>
        /// Gets the revSlider
        /// </summary>
        public virtual RevSlider RevSlider { get; set; }

        /// <summary>
        /// Gets or sets the collection of RevSliderWidgetZone
        /// </summary>
        public virtual ICollection<RevSliderWidgetZone> RevSliderWidgetZones
        {
            get { return _revSliderWidgetZones ?? (_revSliderWidgetZones = new List<RevSliderWidgetZone>()); }
            protected set { _revSliderWidgetZones = value; }
        }
    }

}

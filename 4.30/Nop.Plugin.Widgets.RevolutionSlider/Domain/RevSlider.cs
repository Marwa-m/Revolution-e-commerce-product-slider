﻿using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
    public partial class RevSlider : BaseEntity, ILocalizedEntity, IStoreMappingSupported, IAclSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the date and time of slider creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of slider update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int SchedulePatternId { get; set; }

        public TimeSpan? FromTimeUtc { get; set; }

        
        public TimeSpan? ToTimeUtc { get; set; }

        public bool ShowName { get; set; }
        public string CustomStyle { get; set; }
        public bool HideOnMobile { get; set; }
        public virtual ICollection<RevSliderUrlRecord> RevSliderUrlRecords { get; set; }
    }

}

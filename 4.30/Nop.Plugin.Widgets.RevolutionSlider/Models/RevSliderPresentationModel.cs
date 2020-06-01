using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Models
{
   public partial class RevSliderPresentationModel : BaseNopEntityModel, ILocalizedModel<RevSliderLocalizedModel>
    {
        public RevSliderPresentationModel()
        {
            Locales = new List<RevSliderLocalizedModel>();
          //  revSliderProductPresentaionModels = new IList<RevSliderProductPresentaionModels>();
        }

        public string Name { get; set; }
        public string CustomStyle { get; set; }
        public bool HideOnMobile { get; set; }

        public bool ShowName { get; set; }
        public IList<RevSliderLocalizedModel> Locales { get; set; }
        public IList<RevSliderProductPresentaionModel> revSliderProductPresentaionModels { get; set; }

    }
}

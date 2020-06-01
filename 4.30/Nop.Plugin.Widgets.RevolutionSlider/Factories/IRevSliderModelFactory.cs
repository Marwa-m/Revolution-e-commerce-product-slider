using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Models;

namespace Nop.Plugin.Widgets.RevolutionSlider.Factories
{
    public partial interface IRevSliderModelFactory
    {
        RevSliderSearchModel PrepareRevSliderSearchModel(RevSliderSearchModel searchModel);
        RevSliderListModel PrepareRevSliderListModel(RevSliderSearchModel searchModel);

        RevSliderModel PrepareRevSliderModel(RevSliderModel model, RevSlider revSlider, bool excludeProperties = false);


        RevSliderProductListModel PrepareRevSliderProductListModel(RevSliderProductSearchModel searchModel,RevSlider revSlider);
        #region widgetZone
        RevSliderWidgetZoneListModel PrepareRevSliderWidgetZoneListModel(RevSliderWidgetZoneSearchModel searchModel, RevSlider revSlider);

        #endregion

        #region UrlRecord
        RevSliderUrlRecordListModel PrepareRevSliderUrlRecordListModel(RevSliderUrlRecordSearchModel searchModel, RevSlider revSlider);

        #endregion
    }
}

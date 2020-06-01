using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Extensions
{
    public static class RevolutionSliderExtensions
    {
        public static RevSliderProduct FindRevSliderProduct(this IList<RevSliderProduct> source,
           int productId, int revSliderId)
        {
            foreach (var productRevSlider in source)
                if (productRevSlider.ProductId == productId && productRevSlider.RevSliderId == revSliderId)
                    return productRevSlider;

            return null;
        }

        public static RevSliderWidgetZone FindWidgetZoneRevSlider(this IList<RevSliderWidgetZone> source,
          string widgetzoneName, int revSliderId)
        {
            foreach (var widgetzoneRevSlider in source)
                if (widgetzoneRevSlider.WidgetZone == widgetzoneName && widgetzoneRevSlider.RevSliderId == revSliderId)
                    return widgetzoneRevSlider;

            return null;
        }

        public static RevSliderModel.RevSliderUrlRecordModel FindRevSliderUrlRecord(this IList<RevSliderModel.RevSliderUrlRecordModel> source,
          int UrlRecordId, int revSliderId)
        {
            foreach (var item in source)
                if (item.UrlRecordId == UrlRecordId && item.RevSliderId == revSliderId)
                    return item;

            return null;
        }
    }

}

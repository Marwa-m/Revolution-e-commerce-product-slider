using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Widgets.ProductsSlider.Data;

namespace Nop.Plugin.Widgets.ProductsSlider.Factories
{
    public partial interface IProductSliderModelFactory
    {
        ProductSliderSearchModel PrepareProductSliderSearchModel(ProductSliderSearchModel searchModel);
        ProductSliderListModel PrepareProductSliderListModel(ProductSliderSearchModel searchModel);

    }
}

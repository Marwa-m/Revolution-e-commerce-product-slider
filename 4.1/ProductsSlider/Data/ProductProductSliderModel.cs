using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    public class ProductProductSliderModel
    {
        public IList<ProductSliderLocalizedModel> Locales { get; set; }

        public ProductProductSliderModel()
        {
            Locales = new List<ProductSliderLocalizedModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public decimal Price { get; set; }

        public decimal OldPrice { get; set; }
        public int Preview { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? AvailableEndDateTimeUtc { get; set; }
    }

}

using FluentValidation;
using Nop.Data;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Data
{
    
    public class ProductSliderValidator : BaseNopValidator<ProductSliderModel>
    {
        public ProductSliderValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            // SetStringPropertiesMaxLength<ProductSlider>(dbContext);

            RuleFor(x => x.Preview).InclusiveBetween(0, 5)
    .WithMessage(localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Values.Fields.Preview.Between0To5"));
            RuleFor(x => x.ProductId)
                     .NotNull()
                     .WithMessage(localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Product.Required"));
            RuleFor(x => x.ProductId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Product.Required"));

            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliders.Name.Required"));
        }
    }
}

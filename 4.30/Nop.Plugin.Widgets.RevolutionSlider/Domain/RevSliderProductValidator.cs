using FluentValidation;
using Nop.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.RevolutionSlider.Models;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{

    public class RevSliderProductValidator : BaseNopValidator<RevSliderModel.AddRevSliderProductModel>
    {
        public RevSliderProductValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            // SetStringPropertiesMaxLength<ProductSlider>(dbContext);

            RuleFor(x => x.Preview).InclusiveBetween(0, 5)
    .WithMessage(localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Values.Fields.Preview.Between0To5"));
            RuleFor(x => x.ProductId)
                     .NotNull()
                     .WithMessage(localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Product.Required"));
            RuleFor(x => x.ProductId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Product.Required"));

            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSlider.Name.Required"));

            SetDatabaseValidationRules<RevSliderProduct>(dataProvider);
        }
    }
}

using FluentValidation;
using Nop.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
    public partial class RevSliderValidator : BaseNopValidator<RevSliderModel>
    {
        public RevSliderValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {

            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));

            SetDatabaseValidationRules<RevSliderProduct>(dataProvider);
        }
    }
}

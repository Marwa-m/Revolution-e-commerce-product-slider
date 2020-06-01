using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.RevolutionSlider.Factories;
using Nop.Plugin.Widgets.RevolutionSlider.Services;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //services
            builder.RegisterType<RevSliderService>().As<IRevSliderService>().InstancePerLifetimeScope();
            //Factories
            builder.RegisterType<RevSliderModelFactory>().As<IRevSliderModelFactory>().InstancePerLifetimeScope();


        }

        public int Order => 1;
    }
}

using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Plugin.Widgets.ProductsSlider.Services;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Domain.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_product_view_Rev_prods_slider";


        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //data context
            builder.RegisterPluginDataContext<ProductSliderObjectContext>(CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ProductSlider>>()
                .As<IRepository<ProductSlider>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            //services
            builder.RegisterType<ProductSliderService>().As<IProductSliderService>().InstancePerLifetimeScope();

        }

        public int Order
        {
            get { return 1; }
        }
    }
}

using Autofac;
using Autofac.Integration.Mvc;
using LiquorStore.Domain;
using LiquorStore.Services.Categories;
using LiquorStore.Services.Liquors;
using LiquorStore.Services.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace LiquorStore.Web.Extensions
{
    public class LiquorStoreDependencyResolver
    {
        public static IDependencyResolver WebDependencyResolver()
        {
            var builder = new ContainerBuilder();

            var liquorStoreWeb = Assembly.GetCallingAssembly();
            // Register the MVC controllers.
            builder.RegisterControllers(liquorStoreWeb);

            //Register model binders that require DI.
            builder.RegisterModelBinders(liquorStoreWeb);
            builder.RegisterModelBinderProvider();

            //Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            //Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            //Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // The Autofac ExtensibleActionInvoker attempts to resolve parameters
            // from the request lifetime scope IF the model binder can't bind
            // to the parameter.
            builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>();
           
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<LiquorService>().As<ILiquorService>();
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<ApplicationDbContext>().As<ApplicationDbContext>();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            return new AutofacDependencyResolver(container);
        }
    }
}
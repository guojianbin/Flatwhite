﻿using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Flatwhite.AutofacIntergration;
using Flatwhite.Provider;
using Flatwhite.WebApi.CacheControl;
using Flatwhite.WebApi2;
using log4net;
using Owin;

namespace Flatwhite.WebApi.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // OPTIONAL: I'm using log4net to debug, just implement your own log adaptor
            log4net.Config.XmlConfigurator.Configure();
            Global.Logger = new FlatwhiteLog4netAdaptor(LogManager.GetLogger("Flatwhite"));

            var config = new HttpConfiguration();
            var container = BuildAutofacContainer(config);
            
            WebApiConfig.Register(config);
            config.UseFlatwhiteCache(new FlatwhiteWebApiConfiguration
            {
                EnableStatusController = true,
                LoopbackAddress = null // Set it to web server loopback address if server is behind firewall
            });

            app.UseWebApi(config);
        }

        private IContainer BuildAutofacContainer(HttpConfiguration config)
        {
            var builder = new ContainerBuilder().EnableFlatwhite();

            // This will also be set to Global.CacheStrategyProvider in UseFlatwhiteCache method
            builder.RegisterType<WebApiCacheStrategyProvider>().As<ICacheStrategyProvider>().SingleInstance();

            // This is required by EtagHeaderHandler and OutputCacheAttribute when it builds the response
            builder.RegisterType<CacheResponseBuilder>().As<ICacheResponseBuilder>().SingleInstance();

            // This is required by CachControlHeaderHandlerProvider
            // NOTE: Register more instances of ICachControlHeaderHandler here
            builder.RegisterType<EtagHeaderHandler>().As<ICachControlHeaderHandler>().SingleInstance();
            
            



            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);


            // OPTIONAL: I'm using log4net to debug
            builder.RegisterInstance(LogManager.GetLogger("Flatwhite")).As<ILog>();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }
    }
}

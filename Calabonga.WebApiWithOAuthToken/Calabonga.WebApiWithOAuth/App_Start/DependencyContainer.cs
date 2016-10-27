using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using log4net;
using Owin;

namespace WebApplication1
{
    /// <summary>
    /// Dependancy Container
    /// </summary>
    public static class DependencyContainer {
        /// <summary>
        /// Initialize container
        /// </summary>
        /// <param name="app"></param>
        internal static IContainer Initialize(IAppBuilder app) {

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // -----------------------------------------------------------------
            // my services and DbContext registered here
            // -----------------------------------------------------------------
            // builder.RegisterType<FactService>().As<IFactService>();
            // builder.RegisterType<TagService>().As<ITagService>();
            // builder.RegisterType<ApplicationDbContext>().As<IContext>();
             builder.RegisterType<AccountMananger>().As<IAccountMananger>();
            // builder.RegisterType<Config>().As<IAppConfig>();
            // builder.RegisterType<CacheService>().As<ICacheService>();
            // builder.RegisterType<DefaultConfigSerializer>().As<IConfigSerializer>();

            builder.RegisterType<ApplicationOAuthProvider>().AsSelf().SingleInstance();

            builder.RegisterInstance(LogManager.GetLogger(typeof(Startup))).As<ILog>();

            return builder.Build();
        }
    }
}
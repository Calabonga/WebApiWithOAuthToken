using System;
using Autofac;
using Autofac.Integration.WebApi;
using Calabonga.Portal.Config;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApplication1;


[assembly: OwinStartup(typeof(Startup))]
namespace WebApplication1 {

    /// <summary>
    /// Start for Owin
    /// </summary>
    public class Startup {

        /// <summary>
        /// Server OAuthorization Options
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthAuthorizationServer { get; set; }

        public void Configuration(IAppBuilder app) {
            var config = ConfigurationBuilder.Create();
            var container = DependencyContainer.Initialize(app);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            var provider = container.Resolve<ApplicationOAuthProvider>();
            OAuthAuthorizationServer = new OAuthAuthorizationServerOptions {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/custom-token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = provider
            };
            app.UseOAuthAuthorizationServer(OAuthAuthorizationServer);
            app.UseBearerOnCookieAuthentication();
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseSpaWebApi();
        }
    }
}
using Owin;

namespace WebApplication1
{
    /// <summary>
    /// Static extensions for AppFunc
    /// </summary>
    public static class AppFuncExtensions {
        /// <summary>
        /// Setup to use WebApiAllication as default framework for Application
        /// </summary>
        /// <param name="app"></param>
        public static void UseSpaWebApi(this IAppBuilder app) {
            app.Use<SinglePageWithWebApi>();
        }

        /// <summary>
        /// Use bearer authentication on cookie
        /// </summary>
        /// <param name="app"></param>
        public static void UseBearerOnCookieAuthentication(this IAppBuilder app) {
            app.Use<BearerOnCookieAuthentication>();
        }
    }
}
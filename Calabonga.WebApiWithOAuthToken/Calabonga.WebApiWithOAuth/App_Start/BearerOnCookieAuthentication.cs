using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace WebApplication1
{
    /// <summary>
    /// Middleware for OWIN enables using bearer authentication over cookies
    /// </summary>
    public class BearerOnCookieAuthentication : OwinMiddleware {

        public BearerOnCookieAuthentication(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context) {
            var cookies = context.Request.Cookies;
            var cookie = cookies.FirstOrDefault(c => c.Key == ApplicationOAuthProvider.TokenName);
            if (!context.Request.Headers.ContainsKey("Authorization")) {
                if (!cookie.Equals(default(KeyValuePair<string, string>))) {
                    var ticket = cookie.Value;
                    context.Request.Headers.Add("Authorization", new[] { $"Bearer {ticket}" });
                }
            }
            await Next.Invoke(context);
        }
    }
}
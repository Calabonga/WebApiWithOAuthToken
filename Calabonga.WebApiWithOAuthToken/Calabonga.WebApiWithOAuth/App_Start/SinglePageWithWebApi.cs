using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace WebApplication1
{
    /// <summary>
    /// Web API application for Single Page Application
    /// </summary>
    public class SinglePageWithWebApi : OwinMiddleware {

        public SinglePageWithWebApi(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context) {
            var filePath = HttpContext.Current.Server.MapPath(string.Concat("~/", "views/index.html"));
            var content = File.ReadAllText(filePath);
            await context.Response.WriteAsync(content);
        }
    }
}
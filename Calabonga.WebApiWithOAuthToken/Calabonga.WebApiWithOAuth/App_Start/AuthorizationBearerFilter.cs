using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApplication1
{
    /// <summary>
    /// Custom authorization filter
    /// </summary>
    public class AuthorizationBearerFilter : Attribute, IAuthenticationFilter {

        public bool AllowMultiple { get { return false; } }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken) {

            var request = context.Request;
            var authorization = request.Headers.Authorization;
            if (authorization == null) {
                return null;
            }
            if (authorization.Scheme != "Bearer") return null;

            cancellationToken.ThrowIfCancellationRequested();

            var ticket = Startup.OAuthAuthorizationServer.AccessTokenFormat.Unprotect(authorization.Parameter);
            if (ticket == null) return Task.CompletedTask;

            // do validation with ticket
            var nameClaim = new Claim(ClaimTypes.Name, "UserName");
            var claims = new List<Claim> { nameClaim };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            context.Principal = principal;
            return Task.CompletedTask;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}
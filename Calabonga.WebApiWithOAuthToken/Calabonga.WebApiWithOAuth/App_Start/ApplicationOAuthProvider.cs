using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Calabonga.OperationResults;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using WebApplication1.Models;

namespace WebApplication1
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider {
        private readonly IAccountMananger _mobileAccountMananger;
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(IAccountMananger mobileAccountMananger) {
            _mobileAccountMananger = mobileAccountMananger;
            _publicClientId = DefaultAuthenticationTypes.ExternalBearer;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {

            var client = new LoginViewModel {
                Password = context.Password,
                UserName = context.UserName
            };

            var validateOperation = await _mobileAccountMananger.AuthorizeUserAsync(client);
            if (!validateOperation.Ok) {
                context.SetError("invalid_grant", validateOperation.GetMetadataMessages());
            }
            else {
                var oAuthIdentity = new ClaimsIdentity(validateOperation.Result.Claims, _publicClientId);
                var cookiesIdentity = new ClaimsIdentity(validateOperation.Result.Claims, _publicClientId);
                var properties = CreateProperties(context.UserName, GetType().Namespace);
                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
                context.Response.Cookies.Append(TokenName, context.Options.AccessTokenFormat.Protect(ticket));
                context.Validated(ticket);
            }
        }

        internal static string TokenName { get; } = "Token";

        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
            foreach (var property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null) {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context) {
            if (context.ClientId == _publicClientId) {
                var expectedRootUri = new Uri(context.Request.Uri, "/");
                if (expectedRootUri.AbsoluteUri == context.RedirectUri) {
                    context.Validated();
                }
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Create Authentication properties
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        private static AuthenticationProperties CreateProperties(string userName, string appName) {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "UserName", userName },
                { "ApplicationName", appName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
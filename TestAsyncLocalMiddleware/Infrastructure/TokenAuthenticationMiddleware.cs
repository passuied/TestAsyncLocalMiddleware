using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenAuthenticationMiddleware
    {
        private const string AuthCorpIdHeader = "x-auth-corp-id";
        private const string AuthUserHeader = "x-auth-user-id";
        private readonly RequestDelegate _next;
        private readonly ISecurityTokenAccessor _securityTokenAccessor;

        public TokenAuthenticationMiddleware(RequestDelegate next, ISecurityTokenAccessor securityTokenAccessor)
        {
            _next = next;
            this._securityTokenAccessor = securityTokenAccessor;
        }

        public async Task Invoke(HttpContext context, ILog log)
        {
            try
            {
                log.Trace("Authentication Middleware invoked");

                if (_securityTokenAccessor != null && context.Request.Headers.Any())
                    PopulateTokenAccessor(context.Request.Headers);


            }
            catch (Exception ex)
            {
                log.Error(string.Format(
                    "Error has occurred while trying authenticate the token in {0}. Error details: {1}",
                    nameof(TokenAuthenticationMiddleware), ex));

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Populate SecurityTokenAccessor.
        /// </summary>
        private void PopulateTokenAccessor(IHeaderDictionary headers)
        {
            var contextItems = new Dictionary<string, object>();
            //CorpId
            if (headers.ContainsKey(AuthCorpIdHeader))
                contextItems.Add(ContextBuilder.CorpIdHeader, headers[AuthCorpIdHeader].First());

            // UserId
            if (headers.ContainsKey(AuthUserHeader))
                contextItems.Add(ContextBuilder.UserIdHeader, headers[AuthUserHeader].First());

            _securityTokenAccessor.ContextItems = contextItems;

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
    }
}

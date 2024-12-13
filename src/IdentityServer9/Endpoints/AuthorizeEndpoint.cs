// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using IdentityServer9.Configuration;
using IdentityServer9.Endpoints.Results;
using IdentityServer9.Extensions;
using IdentityServer9.Hosting;
using IdentityServer9.ResponseHandling;
using IdentityServer9.Services;
using IdentityServer9.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer9.Endpoints
{
    internal class AuthorizeEndpoint : AuthorizeEndpointBase
    {
        public AuthorizeEndpoint(
           IEventService events,
           ILogger<AuthorizeEndpoint> logger,
           IdentityServerOptions options,
           IAuthorizeRequestValidator validator,
           IAuthorizeInteractionResponseGenerator interactionGenerator,
           IAuthorizeResponseGenerator authorizeResponseGenerator,
           IUserSession userSession)
            : base(events, logger, options, validator, interactionGenerator, authorizeResponseGenerator, userSession)
        {
        }

        public override async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            Logger.LogDebug("Start authorize request");

            NameValueCollection values;

            if (HttpMethods.IsGet(context.Request.Method))
            {
                values = context.Request.Query.AsNameValueCollection();
            }
            else if (HttpMethods.IsPost(context.Request.Method))
            {
                if (!context.Request.HasApplicationFormContentType())
                {
                    return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                }

                values = context.Request.Form.AsNameValueCollection();
            }
            else
            {
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            var user = await UserSession.GetUserAsync();
            var result = await ProcessAuthorizeRequestAsync(values, user, null);

            Logger.LogTrace("End authorize request. result type: {0}", result?.GetType().ToString() ?? "-none-");

            return result;
        }
    }
}

#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Enbrea.ApiKey.Tests
{
    public class ApiKeyFilterTests
    {
        [Fact]
        public async Task Denies_When_Missing_Policy_Returns_Unauthorized()
        {
            var http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse("8.8.8.8");
            var ctx = NewContext(http);

            var filter = NewFilter(null, new ApiKeyExtractor(new ApiKeyExtractorOptions()));
            await filter.OnActionExecutionAsync(ctx, () => Task.FromResult<ActionExecutedContext>(null));

            Assert.IsType<UnauthorizedResult>(ctx.Result);
        }

        [Fact]
        public async Task PrivateOnly_Allows_Loopback_But_Denies_Public_As_NotFound()
        {
            var policy = new ApiKeyPolicy { 
                PrivateOnly = true, 
                AllowLocal = true 
            };

            // Loopback allowed
            var localHttp = new DefaultHttpContext();
            localHttp.Connection.RemoteIpAddress = IPAddress.Loopback;
            var localCtx = NewContext(localHttp);
            var filter = NewFilter(policy);

            var nextCalled = false;
            await filter.OnActionExecutionAsync(localCtx, async () =>
            {
                nextCalled = true;
                return await Task.FromResult<ActionExecutedContext>(null);
            });
            Assert.True(nextCalled);
            Assert.Null(localCtx.Result);

            // Public denied
            var publicHttp = new DefaultHttpContext();
            publicHttp.Connection.RemoteIpAddress = IPAddress.Parse("1.2.3.4");
            var publicCtx = NewContext(publicHttp);

            await filter.OnActionExecutionAsync(publicCtx, () => Task.FromResult<ActionExecutedContext>(null));
            Assert.IsType<NotFoundResult>(publicCtx.Result);
        }

        [Fact]
        public async Task PublicMode_AllowCidrs_Bypasses_Key_Check()
        {
            var http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse("10.1.2.3");

            var ctx = NewContext(http);

            var policy = new ApiKeyPolicy { 
                PrivateOnly = false, 
                AllowCidrs = ["10.0.0.0/8"] 
            };
            var filter = NewFilter(policy);

            var nextCalled = false;
            await filter.OnActionExecutionAsync(ctx, async () =>
            {
                nextCalled = true;
                return await Task.FromResult<ActionExecutedContext>(null);
            });

            Assert.True(nextCalled);
            Assert.Null(ctx.Result);
        }

        [Fact]
        public async Task PublicMode_AllowLocal_Bypasses_Key_Check()
        {
            var http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Loopback;

            var ctx = NewContext(http);

            var policy = new ApiKeyPolicy { 
                AllowLocal = true, 
                PrivateOnly = false, 
                Keys = [] 
            };
            var filter = NewFilter(policy);

            var nextCalled = false;
            await filter.OnActionExecutionAsync(ctx, async () =>
            {
                nextCalled = true;
                return await Task.FromResult<ActionExecutedContext>(null);
            });

            Assert.True(nextCalled);
            Assert.Null(ctx.Result);
        }

        [Fact]
        public async Task PublicMode_With_Invalid_Key_Denies_Unauthorized()
        {
            var http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse("8.8.8.8");
            http.Request.Headers.Authorization = "ApiKey wrong";

            var ctx = NewContext(http);

            var policy = new ApiKeyPolicy { Keys = ["good"] };
            var filter = NewFilter(policy);

            await filter.OnActionExecutionAsync(ctx, () => Task.FromResult<ActionExecutedContext>(null));

            Assert.IsType<UnauthorizedResult>(ctx.Result);
        }

        [Fact]
        public async Task PublicMode_With_Valid_Key_Allows()
        {
            var http = new DefaultHttpContext();
            http.Connection.RemoteIpAddress = IPAddress.Parse("8.8.8.8");
            http.Request.Headers.Authorization = "ApiKey good";

            var ctx = NewContext(http);

            var policy = new ApiKeyPolicy
            {
                Keys = ["good"],
                AllowLocal = false,
                AllowCidrs = [],
                PrivateOnly = false
            };

            var filter = NewFilter(policy, new ApiKeyExtractor(new ApiKeyExtractorOptions()));
            var nextCalled = false;
            await filter.OnActionExecutionAsync(ctx, async () =>
            {
                nextCalled = true;
                return await Task.FromResult<ActionExecutedContext>(null);
            });

            Assert.True(nextCalled);
            Assert.Null(ctx.Result);
        }

        private static ActionExecutingContext NewContext(HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());
        }

        private static ApiKeyFilter NewFilter(IApiKeyPolicy policy, IApiKeyExtractor extractor = null)
        {
            StaticPolicyProvider.Current = policy;

            var services = new ServiceCollection().BuildServiceProvider();
            extractor ??= new ApiKeyExtractor(new ApiKeyExtractorOptions());

            return new ApiKeyFilter(typeof(StaticPolicyProvider), services, extractor, new ApiKeyDefaultErrorFactory(), NullLogger<ApiKeyFilter>.Instance);
        }
    }
}

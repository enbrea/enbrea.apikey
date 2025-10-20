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
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Enbrea.ApiKey.Tests
{
    public class ApiKeyExtractorTests
    {
        [Fact]
        public void From_Authorization_Header_With_ApiKey_Scheme()
        {
            var opts = new ApiKeyExtractorOptions
            {
                AcceptedAuthSchemes = ["ApiKey"]
            };

            var ctx = new DefaultHttpContext();
            var req = ctx.Request;
            req.Headers.Authorization = new StringValues(["ApiKey my-secret"]);

            var extractor = new ApiKeyExtractor(opts);
            var ok = extractor.TryGetApiKey(req, out var key);
            Assert.True(ok);
            Assert.Equal("my-secret", key);
        }

        [Fact]
        public void From_Custom_Header_And_Trim_Quotes()
        {
            var opts = new ApiKeyExtractorOptions
            {
                AcceptedHeaderNames = ["X-API-KEY"]
            };

            var ctx = new DefaultHttpContext();
            var req = ctx.Request;
            req.Headers["X-API-KEY"] = "\"secret-123\"";

            var extractor = new ApiKeyExtractor(opts);
            var ok = extractor.TryGetApiKey(req, out var key);
            Assert.True(ok);
            Assert.Equal("secret-123", key);
        }

        [Fact]
        public void From_Query_String()
        {
            var opts = new ApiKeyExtractorOptions
            {
                AcceptedQueryParamNames = ["api_key", "key"]
            };

            var ctx = new DefaultHttpContext();
            ctx.Request.QueryString = new QueryString("?api_key=abc123");

            var extractor = new ApiKeyExtractor(opts);
            var ok = extractor.TryGetApiKey(ctx.Request, out var key);
            Assert.True(ok);
            Assert.Equal("abc123", key);
        }

        [Fact]
        public void ReturnsFalse_When_Not_Found()
        {
            var opts = new ApiKeyExtractorOptions();
            var ctx = new DefaultHttpContext();
            var req = ctx.Request;

            var extractor = new ApiKeyExtractor(opts);
            var ok = extractor.TryGetApiKey(req, out var key);
            Assert.False(ok);
            Assert.Null(key);
        }
    }
}

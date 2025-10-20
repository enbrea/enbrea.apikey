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
using Xunit;

namespace Enbrea.ApiKey.Tests
{
    public class ErrorFactoryTests
    {
        [Fact]
        public void DefaultFactory_Returns_StatusCodes()
        {
            var http = new DefaultHttpContext();
            var f = new ApiKeyDefaultErrorFactory();

            var r1 = f.Create(http, ApiKeyError.InvalidKey);
            var unauth = Assert.IsType<UnauthorizedResult>(r1);
            Assert.Equal(StatusCodes.Status401Unauthorized, unauth.StatusCode);

            var r2 = f.Create(http, ApiKeyError.PrivateOnlyDenied);
            var notfound = Assert.IsType<NotFoundResult>(r2);
            Assert.Equal(StatusCodes.Status404NotFound, notfound.StatusCode);

            var r3 = f.Create(http, ApiKeyError.Misconfigured);
            var s500 = Assert.IsType<StatusCodeResult>(r3);
            Assert.Equal(StatusCodes.Status500InternalServerError, s500.StatusCode);
        }

        [Fact]
        public void ProblemDetailsFactory_Fills_Problem()
        {
            var http = new DefaultHttpContext();
            var problemFactory = new TestProblemDetailsFactory();
            var f = new ApiKeyProblemDetailsFactory(problemFactory);

            var result = f.Create(http, ApiKeyError.InvalidKey);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);

            var pd = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.False(string.IsNullOrEmpty(pd.Type));
            Assert.Contains("Unauthorized", pd.Title);
            Assert.Equal(http.Request.Path, pd.Instance);
        }
    }
}

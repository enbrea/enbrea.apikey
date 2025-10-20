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

namespace Enbrea.ApiKey
{
    /// <summary>
    /// A <see cref="StatusCodeResult" /> based factory
    /// </summary>
    public class ApiKeyDefaultErrorFactory : IApiKeyErrorResultFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyDefaultErrorFactory" /> class.
        /// </summary>
        public ApiKeyDefaultErrorFactory()
        {
        }

        /// <summary>
        /// Creates an <see cref="StatusCodeResult"/> for a given <see cref="ApiKeyError"/>.
        /// </summary>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <param name="error">The API key error classification</param>
        /// <returns>A <see cref="StatusCodeResult"/> instance</returns>
        public virtual IActionResult Create(HttpContext httpContext, ApiKeyError error)
        {
            return error switch
            {
                ApiKeyError.Misconfigured => new StatusCodeResult(StatusCodes.Status500InternalServerError),
                ApiKeyError.PrivateOnlyDenied => new NotFoundResult(),
                ApiKeyError.InvalidKey => new UnauthorizedResult(),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
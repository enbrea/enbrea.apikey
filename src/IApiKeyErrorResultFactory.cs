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
    /// A factory generating <see cref="IActionResult"/> implementations for a given <see cref="ApiKeyError"/>
    /// </summary>
    public interface IApiKeyErrorResultFactory
    {
        /// <summary>
        /// Creates an <see cref="IActionResult"/> implementation for a given <see cref="ApiKeyError"/>.
        /// </summary>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <param name="error">The API key error classification</param>
        /// <returns>An <see cref="IActionResult"/> implementation</returns>
        IActionResult Create(HttpContext httpContext, ApiKeyError error);
    }
}
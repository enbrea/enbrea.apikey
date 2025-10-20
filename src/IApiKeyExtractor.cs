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

namespace Enbrea.ApiKey
{
    /// <summary>
    /// Extracts an API key from an incoming <see cref="HttpRequest"/> using common patterns.
    /// </summary>
    public interface IApiKeyExtractor
    {
        /// <summary>
        /// Attempts to read an API key from the given HTTP request.
        /// </summary>
        /// <param name="fromRequest">The incoming HTTP request</param>
        /// <param name="apiKey">Contains the extracted API key; otherwise null</param>
        /// <returns>TRUE, if an API key was found using any configured pattern; otherwise FALSE.</returns>
        bool TryGetApiKey(HttpRequest fromRequest, out string apiKey);
    }
}
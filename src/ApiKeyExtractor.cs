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
using System.Net.Http.Headers;

namespace Enbrea.ApiKey
{
    /// <summary>
    /// Extracts an API key from an incoming <see cref="HttpRequest"/> using common patterns.
    /// </summary>
    public class ApiKeyExtractor : IApiKeyExtractor
    {
        private readonly ApiKeyExtractorOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyExtractor"/> class.
        /// </summary>
        /// <param name="options">Behavior configuration</param>
        public ApiKeyExtractor(ApiKeyExtractorOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Attempts to read an API key from the given HTTP request.
        /// </summary>
        /// <param name="fromRequest">The incoming HTTP request</param>
        /// <param name="apiKey">Contains the extracted API key; otherwise null</param>
        /// <returns>TRUE, if an API key was found using any configured pattern; otherwise FALSE.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromRequest"/> is <see langword="null"/>.</exception>
        public bool TryGetApiKey(HttpRequest fromRequest, out string apiKey)
        {
            ArgumentNullException.ThrowIfNull(fromRequest);

            apiKey = null;

            // Authorization header: e.g. ApiKey <key>
            if (fromRequest.Headers.TryGetValue("Authorization", out StringValues authValues))
            {
                foreach (var raw in authValues)
                {
                    if (AuthenticationHeaderValue.TryParse(raw, out var headerValue) &&
                        _options.AcceptedAuthSchemes.Any(s => s.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase)))
                    {
                        var normalizedKeyValue = Normalize(headerValue.Parameter);
                        if (!string.IsNullOrEmpty(normalizedKeyValue))
                        {
                            apiKey = normalizedKeyValue;
                            return true;
                        }
                    }
                }
            }

            // Custom header: e.g. X-API-KEY
            foreach (var headerName in _options.AcceptedHeaderNames)
            {
                if (fromRequest.Headers.TryGetValue(headerName, out var keyValues))
                {
                    var normalizedKeyValue = Normalize(keyValues.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)));
                    if (!string.IsNullOrEmpty(normalizedKeyValue))
                    {
                        apiKey = normalizedKeyValue;
                        return true;
                    }
                }
            }

            // Query string: e.g. ?api_key=<key>
            foreach (var param in _options.AcceptedQueryParamNames)
            {
                if (fromRequest.Query.TryGetValue(param, out var keyValues))
                {
                    var normalizedKeyValue = Normalize(keyValues.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)));
                    if (!string.IsNullOrEmpty(normalizedKeyValue))
                    {
                        apiKey = normalizedKeyValue;
                        return true;
                    }
                }
            }

            return false;

            static string Normalize(string s)
                => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Trim('"'); 
        }
    }
}

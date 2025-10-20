#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

namespace Enbrea.ApiKey
{
    /// <summary>
    /// Options for <see  cref="ApiKeyExtractor"/>
    /// </summary>
    public sealed class ApiKeyExtractorOptions
    {
        /// <summary>
        /// Accepted Authorization header schemes for carrying the API key
        /// </summary>
        public string[] AcceptedAuthSchemes = { "ApiKey" };

        /// <summary>
        /// Accepted custom header names that may contain the API key
        /// </summary>
        public string[] AcceptedHeaderNames = { "X-API-KEY" };

        /// <summary>
        /// Accepted URL query field names that may contain the API key
        /// </summary>
        public string[] AcceptedQueryParamNames = [];
    }
}


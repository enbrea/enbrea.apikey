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
    /// Implementation of <see cref="IApiKeyPolicy"/>
    /// </summary>
    public class ApiKeyPolicy : IApiKeyPolicy
    {
        /// <summary>
        /// List of trusted network ranges in CIDR (Classless Inter-Domain Routing) notation.
        /// Requests from these ranges are allowed without an AAPI key when <see cref="PrivateOnly"/> is false.
        /// In <see cref="PrivateOnly"/> mode, these ranges (plus loopback if enabled) are the only ones allowed.
        /// </summary>
        /// <example>
        /// <code language="json">
        /// "AllowCidrs": [ "10.0.0.0/8", "192.168.0.0/16", "fd00::/8" ]
        /// </code>
        /// </example>        
        public string[] AllowCidrs { get; set; } = [];

        /// <summary>
        /// Whether to allow requests from local loopback addresses.
        /// </summary>
        /// <remarks>
        /// Behind a proxy, ensure forwarded headers are configured so the effective client IP is correct.
        /// </remarks>
        public bool AllowLocal { get; set; } = true;

        /// <summary>
        /// A list of expected API key values to validate when the request is not allowed by
        /// <see cref="AllowLocal"/> or <see cref="AllowCidrs"/>, and <see cref="PrivateOnly"/> is false.
        /// Leave empty only if public access or using private-only access.
        /// </summary>
        public string[] Keys { get; set; } = [];

        /// <summary>
        /// If true, the endpoint is private: only loopback and <see cref="AllowCidrs"/> sources are permitted;
        /// <see cref="Keys"/> is ignored and public access is denied.
        /// </summary>
        public bool PrivateOnly { get; set; } = false;
    }
}

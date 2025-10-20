#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Numerics;

namespace Enbrea.ApiKey
{
    /// <summary>
    /// MVC action filter that enforces API key access control with optional network allow-listing.
    /// </summary>
    public sealed class ApiKeyFilter : IAsyncActionFilter
    {
        private readonly IApiKeyExtractor _apiKeyExtractor;
        private readonly IApiKeyPolicy _apiKeyPolicy;
        private readonly IApiKeyPolicyProvider _apiKeyPolicyProvider;
        private readonly IApiKeyErrorResultFactory _errorResultFactory;
        private readonly ILogger<ApiKeyFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyFilter" /> class.
        /// </summary>
        /// <param name="apiKeyPolicyProviderType">A type that implements <see cref="IApiKeyPolicyProvider"/>.</param>
        /// <param name="serviceProvider">Application service provider used to construct the provider via DI</param>
        /// <param name="apiKeyExtractor">Component that extracts an API key from the incoming request</param>
        /// <param name="errorResultFactory">Factory to produce <see cref="ProblemDetails" /></param>
        /// <param name="logger">Logger for audit/diagnostics.</param>
        public ApiKeyFilter(Type apiKeyPolicyProviderType, IServiceProvider serviceProvider, IApiKeyExtractor apiKeyExtractor, IApiKeyErrorResultFactory errorResultFactory, ILogger<ApiKeyFilter> logger)
        {
            _apiKeyPolicyProvider = (IApiKeyPolicyProvider)ActivatorUtilities.CreateInstance(serviceProvider, apiKeyPolicyProviderType);
            _apiKeyPolicy = _apiKeyPolicyProvider.Get();
            _apiKeyExtractor = apiKeyExtractor;
            _errorResultFactory = errorResultFactory;
            _logger = logger;
        }

        /// <summary>
        /// Executes the API key / network allow-list checks and short-circuits the pipeline if unauthorized.
        /// </summary>
        /// <param name="context">The current action executing context.</param>
        /// <param name="next">Continuation to invoke the action if authorized.</param>
        /// <returns>A task that completes when the action has run or the request has been terminated.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var remoteIp = httpContext.Connection.RemoteIpAddress;

            // Do we have an API key policy?
            if (_apiKeyPolicy is null)
            {
                _logger.LogError("Missing configuration for api key");

                context.Result = _errorResultFactory.Create(httpContext, ApiKeyError.InvalidKey);
                return;
            }

            // Private-only mode
            if (_apiKeyPolicy.PrivateOnly)
            {
                if (remoteIp is not null)
                {
                    if (_apiKeyPolicy.AllowLocal && (IPAddress.IsLoopback(remoteIp) || IsLocalAddress(remoteIp)))
                    {
                        await next(); return;
                    }
                    if (_apiKeyPolicy.AllowCidrs.Length > 0 && IsInAnyCidr(remoteIp, _apiKeyPolicy.AllowCidrs))
                    {
                        await next(); return;
                    }
                }

                _logger.LogWarning("Private-only health access denied to {Path} from {RemoteIp}", httpContext.Request.Path, remoteIp);

                context.Result = _errorResultFactory.Create(httpContext, ApiKeyError.PrivateOnlyDenied);
                return;
            }

            // Public mode
            if (remoteIp is not null)
            {
                if (_apiKeyPolicy.AllowLocal && (IPAddress.IsLoopback(remoteIp) || IsLocalAddress(remoteIp)))
                {
                    await next(); return;
                }
                if (_apiKeyPolicy.AllowCidrs.Length > 0 && IsInAnyCidr(remoteIp, _apiKeyPolicy.AllowCidrs))
                {
                    await next(); return;
                }
            }

            // Otherwise check the given API key 
            if (_apiKeyPolicy.Keys.Length > 0)
            {
                if (_apiKeyExtractor.TryGetApiKey(context.HttpContext.Request, out var providedKey))
                {
                    if (!string.IsNullOrWhiteSpace(providedKey))
                    { 
                        foreach (var definedKey in _apiKeyPolicy.Keys)
                        {
                            if (StringHelper.FixedTimeEquals(providedKey, definedKey))
                            {
                                await next();
                                return;
                            }
                        }
                    }
                }

                _logger.LogWarning("Unauthorized health access to {Path} from {RemoteIp}", httpContext.Request.Path, remoteIp);

                context.Result = _errorResultFactory.Create(httpContext, ApiKeyError.InvalidKey);
                return;
            }

            await next(); 
            return;
        }

        /// <summary>
        /// Returns TRUE if the given IP address is contained in any of the provided CIDR ranges.
        /// </summary>
        private static bool IsInAnyCidr(IPAddress ip, IEnumerable<string> cidrs)
        {
            return cidrs.Any(c => IsInCidr(ip, c));
        }

        /// <summary>
        /// Checks if an IP address is within a specific CIDR block (IPv4 or IPv6).
        /// </summary>
        private static bool IsInCidr(IPAddress ip, string cidr)
        {
            var parts = cidr.Split('/', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2) return false;

            if (!IPAddress.TryParse(parts[0], out var network)) return false;
            if (!int.TryParse(parts[1], out var prefixLen)) return false;
            if (ip.AddressFamily != network.AddressFamily) return false;

            var ipBits = ToBigInteger(ip);
            var netBits = ToBigInteger(network);
            var totalBits = ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;

            if (prefixLen < 0 || prefixLen > totalBits) return false;

            var mask = PrefixMask(totalBits, prefixLen);
            return (ipBits & mask) == (netBits & mask);
        }

        /// <summary>
        /// Treats IPv6 link-local as "local" in addition to formal loopback.
        /// </summary>
        /// <remarks>
        /// RFC1918 private ranges are not considered local here; use <see cref="IsInAnyCidr"/> for those.
        /// </remarks>
        private static bool IsLocalAddress(IPAddress ip)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return ip.IsIPv6LinkLocal || IPAddress.IPv6Loopback.Equals(ip);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Builds a left-aligned bit mask of the given prefix length for the address size.
        /// </summary>
        private static BigInteger PrefixMask(int totalBits, int prefix)
        {
            if (prefix == 0) return BigInteger.Zero;
            var ones = BigInteger.One;
            var maskLow = (ones << prefix) - 1;
            var shift = totalBits - prefix;
            return maskLow << shift;
        }

        /// <summary>
        /// Converts an <see cref="IPAddress"/> to a positive <see cref="BigInteger"/> for bitwise operations.
        /// </summary>
        private static BigInteger ToBigInteger(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();
            var unsigned = new byte[bytes.Length + 1];
            for (int i = 0; i < bytes.Length; i++) unsigned[i] = bytes[i];
            Array.Reverse(unsigned); // BigInteger expects little-endian
            return new BigInteger(unsigned);
        }
    }
}
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

namespace Enbrea.ApiKey
{
    /// <summary>
    /// Attribute that enforces API key–based access control on a controller or action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiKeyAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyAttribute"/> class.
        /// </summary>
        /// <param name="policyProviderType"> A type that implements <see cref="IApiKeyPolicyProvider"/>. This type will be 
        /// instantiated via DI when the filter executes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="policyProviderType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="policyProviderType"/> does not implement <see cref="IApiKeyPolicyProvider"/>.</exception>
        public ApiKeyAttribute(Type policyProviderType) : base(typeof(ApiKeyFilter))
        {
            ArgumentNullException.ThrowIfNull(policyProviderType);

            if (!typeof(IApiKeyPolicyProvider).IsAssignableFrom(policyProviderType))
            {
                throw new ArgumentException($"{policyProviderType.Name} must implement {nameof(IApiKeyPolicyProvider)}", nameof(policyProviderType));
            }

            Arguments = [policyProviderType];
        }
    }
}
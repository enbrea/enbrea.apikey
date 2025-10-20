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
    /// Abstraction that supplies the effective API key policy used by <see cref="ApiKeyFilter"/> at runtime.
    /// </summary>
    public interface IApiKeyPolicyProvider
    {
        /// <summary>
        /// Returns the current API key configuration used to authorize the request.
        /// </summary>
        /// <returns>An API key policy instance</returns>
        IApiKeyPolicy Get();
    }
}
#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

namespace Enbrea.ApiKey.Tests
{
    public sealed class StaticPolicyProvider : IApiKeyPolicyProvider
    {
        public static IApiKeyPolicy Current { get; set; } = new ApiKeyPolicy();
        public IApiKeyPolicy Get() => Current;
    }
}

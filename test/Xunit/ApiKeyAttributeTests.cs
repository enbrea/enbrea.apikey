#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using System;
using Xunit;

namespace Enbrea.ApiKey.Tests
{
    public class ApiKeyAttributeTests
    {
        [Fact]
        public void Sets_Filter_Argument_To_ProviderType()
        {
            var attr = new ApiKeyAttribute(typeof(StaticPolicyProvider));
            Assert.Contains(typeof(StaticPolicyProvider), attr.Arguments);
        }

        [Fact]
        public void Throws_On_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiKeyAttribute(null!));
        }

        [Fact]
        public void Throws_On_Wrong_Type()
        {
            Assert.Throws<ArgumentException>(() => new ApiKeyAttribute(typeof(NotAProvider)));
        }

        private sealed class NotAProvider { }
    }
}

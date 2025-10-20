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
    public class StringHelperTests
    {
        [Theory]
        [InlineData("abc", "abc", true)]
        [InlineData("abc", "abd", false)]
        [InlineData("", "", true)]
        public void FixedTimeEquals_Works(string a, string b, bool expected)
        {
            Assert.Equal(expected, StringHelper.FixedTimeEquals(a, b));
        }

        [Fact]
        public void Match_Ignores_InvalidPatterns()
        {
            Assert.True(StringHelper.Match("abc", ["(*invalid", "ab[c]"], TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void Match_ReturnsFalse_OnNullsOrEmpty()
        {
            Assert.False(StringHelper.Match(null, null, TimeSpan.FromSeconds(1)));
            Assert.False(StringHelper.Match("", ["a.*"], TimeSpan.FromSeconds(1)));
            Assert.False(StringHelper.Match("value", null, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void Match_ReturnsTrue_OnValidRegex()
        {
            Assert.True(StringHelper.Match("hello-123", ["hello-\\d+"], TimeSpan.FromSeconds(1)));
        }
    }
}

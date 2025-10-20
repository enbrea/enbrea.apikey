#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Enbrea.ApiKey
{
    /// <summary>
    /// Static string helper class
    /// </summary>
    public static class StringHelper
    {
        private static readonly ConcurrentDictionary<(string Pattern, RegexOptions Options), Regex> _regexCache = new();

        /// <summary>
        /// Compares two strings in constant time to mitigate timing attacks.
        /// </summary>
        /// <param name="left">First value</param>
        /// <param name="right">Second value</param>
        /// <returns>TRUE if equal, otherwise FALSE</returns>
        public static bool FixedTimeEquals(string left, string right)
        {
            var lBytes = System.Text.Encoding.UTF8.GetBytes(left);
            var rBytes = System.Text.Encoding.UTF8.GetBytes(right);
            return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(lBytes, rBytes);
        }

        /// <summary>
        /// Tries to match a value against a list of regex patterns
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="patterns">The list of regex patterns</param>
        /// <param name="timeout">A timeout to prevent the search from taking too long.</param>
        /// <returns>TRUE if matching, otherwise FALSE</returns>
        public static bool Match(string value, IEnumerable<string> patterns, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(value) || patterns is null)
            {
                return false;
            }

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern)) continue;
                try
                {
                    var regex = _regexCache.GetOrAdd(
                        ($@"\A(?:{pattern})\z", RegexOptions.CultureInvariant | RegexOptions.Compiled),
                        key => new Regex(key.Pattern, key.Options, timeout)
                    );

                    if (regex.IsMatch(value))
                    {
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore invalid regex
                }
            }

            return false;
        }
    }
}

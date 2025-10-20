#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using Microsoft.Extensions.DependencyInjection;

namespace Enbrea.ApiKey
{
    /// <summary>
    /// DI registration helpers for API-key extraction/validation.
    /// </summary>
    public static class ApiKeyServiceRegistration
    {
        /// <summary>
        /// Registers the default <see cref="ApiKeyExtractor"/> as a singleton with default <see cref="ApiKeyExtractorOptions"/> values.
        /// </summary>
        /// <param name="services">The DI service collection</param>
        /// <returns>The same <paramref name="services"/> for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
        public static IApiKeyServiceBuilder AddApiKeyValidation(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton(new ApiKeyExtractorOptions());
            services.AddSingleton<IApiKeyExtractor, ApiKeyExtractor>();
            services.AddSingleton<IApiKeyErrorResultFactory, ApiKeyDefaultErrorFactory>();
            return new ApiKeyServiceBuilder(services);
        }

        /// <summary>
        /// Registers the default <see cref="ApiKeyExtractor"/> as a singleton and allows the caller
        /// to configure the <see cref="ApiKeyExtractorOptions"/> instance used by the extractor.
        /// </summary>
        /// <param name="services">The DI service collection</param>
        /// <param name="configure">Delegate to populate <see cref="ApiKeyExtractorOptions"/> at startup</param>
        /// <returns>The same <paramref name="services"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is null</exception>
        public static IApiKeyServiceBuilder AddApiKeyValidation(this IServiceCollection services, Action<ApiKeyExtractorOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            var options = new ApiKeyExtractorOptions();
            configure(options);

            services.AddSingleton(options);
            services.AddSingleton<IApiKeyExtractor, ApiKeyExtractor>();
            services.AddSingleton<IApiKeyErrorResultFactory, ApiKeyDefaultErrorFactory>();
            return new ApiKeyServiceBuilder(services);
        }
    }
}
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
    /// Provides a builder for configuring API key authentication services and related error handling components within
    /// a dependency injection container.
    /// </summary>
    internal sealed class ApiKeyServiceBuilder : IApiKeyServiceBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyServiceBuilder" /> class.
        /// </summary>
        /// <param name="services">The collection of service descriptors to be configured for API key services.</param>
        public ApiKeyServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Configures the builder to use the default error factory for API key validation errors.
        /// </summary>
        /// <returns>The current <see cref="IApiKeyServiceBuilder"/> instance for method chaining.</returns>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Configures the builder to use the default error factory for API key validation errors.
        /// </summary>
        /// <returns>The current <see cref="IApiKeyServiceBuilder"/> instance for method chaining.</returns>
        public IApiKeyServiceBuilder UseDefaultErrorFactory()
        {
            return UseErrorResultFactory<ApiKeyDefaultErrorFactory>();
        }

        /// <summary>
        /// Configures the API key service builder to use the specified error result factory type for generating error
        /// responses.
        /// </summary>
        /// <returns>The current instance of the API key service builder for method chaining.</returns>
        public IApiKeyServiceBuilder UseErrorResultFactory<TErrorResultFactory>()
            where TErrorResultFactory : class, IApiKeyErrorResultFactory
        {
            Services.AddSingleton<IApiKeyErrorResultFactory, TErrorResultFactory>();
            return this;
        }

        /// <summary>
        /// Configures the builder to use a custom factory for generating problem details responses in the API key
        /// service pipeline.
        /// </summary>
        /// <returns>The current instance of <see cref="IApiKeyServiceBuilder"/> to allow for method chaining.</returns>
        public IApiKeyServiceBuilder UseProblemDetailsFactory()
        {
            return UseErrorResultFactory<ApiKeyProblemDetailsFactory>();
        }
    }
}
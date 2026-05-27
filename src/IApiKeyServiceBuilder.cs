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
    /// Builder interface for configuring API key services.
    /// </summary>
    public interface IApiKeyServiceBuilder
    {
        /// <summary>
        /// Gets the collection of service descriptors for dependency injection configuration.
        /// </summary>
        IServiceCollection Services { get; }
        
        /// <summary>
        /// Configures the builder to use the default error factory for API key validation errors.
        /// </summary>
        /// <returns>The current <see cref="IApiKeyServiceBuilder"/> instance for method chaining.</returns>
        IApiKeyServiceBuilder UseDefaultErrorFactory();

        /// <summary>
        /// Configures the API key service builder to use the specified error result factory type for generating error
        /// responses.
        /// </summary>
        /// <returns>The current instance of the API key service builder for method chaining.</returns>
        IApiKeyServiceBuilder UseErrorResultFactory<TErrorResultFactory>() 
            where TErrorResultFactory : class, IApiKeyErrorResultFactory;

        /// <summary>
        /// Configures the builder to use a custom factory for generating problem details responses in the API key
        /// service pipeline.
        /// </summary>
        /// <returns>The current instance of <see cref="IApiKeyServiceBuilder"/> to allow for method chaining.</returns>
        IApiKeyServiceBuilder UseProblemDetailsFactory();
    }
}
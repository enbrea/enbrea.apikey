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
    internal sealed class ApiKeyServiceBuilder : IApiKeyServiceBuilder
    {
        public ApiKeyServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IApiKeyServiceBuilder UseDefaultErrorFactory()
        {
            return UseErrorResultFactory<ApiKeyDefaultErrorFactory>();
        }

        public IApiKeyServiceBuilder UseErrorResultFactory<TErrorResultFactory>()
            where TErrorResultFactory : class, IApiKeyErrorResultFactory
        {
            Services.AddSingleton<IApiKeyErrorResultFactory, TErrorResultFactory>();
            return this;
        }

        public IApiKeyServiceBuilder UseProblemDetailsFactory()
        {
            return UseErrorResultFactory<ApiKeyProblemDetailsFactory>();
        }
    }
}
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
    public interface IApiKeyServiceBuilder
    {
        IServiceCollection Services { get; }
        
        IApiKeyServiceBuilder UseDefaultErrorFactory();

        IApiKeyServiceBuilder UseErrorResultFactory<TErrorResultFactory>() 
            where TErrorResultFactory : class, IApiKeyErrorResultFactory;

        IApiKeyServiceBuilder UseProblemDetailsFactory();
    }
}
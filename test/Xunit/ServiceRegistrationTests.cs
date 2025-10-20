#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Enbrea.ApiKey.Tests
{
    public class ServiceRegistrationTests
    {
        [Fact]
        public void AddApiKeyValidation_WithDefaultErrorFactory()
        {
            var services = new ServiceCollection();
            var builder = ApiKeyServiceRegistration.AddApiKeyValidation(services);

            Assert.NotNull(builder);
            Assert.Same(services, builder.Services);

            var sp = services.BuildServiceProvider();
            Assert.NotNull(sp.GetService<IApiKeyExtractor>());
            Assert.IsType<ApiKeyDefaultErrorFactory>(sp.GetService<IApiKeyErrorResultFactory>());
        }

        [Fact]
        public void AddApiKeyValidation_WithProblemDetialsFactory()
        {
            var services = new ServiceCollection();
            var builder = ApiKeyServiceRegistration.AddApiKeyValidation(services);

            Assert.NotNull(builder);
            Assert.Same(services, builder.Services);

            services.AddSingleton<ProblemDetailsFactory, TestProblemDetailsFactory>();
            builder.UseProblemDetailsFactory();

            var sp = services.BuildServiceProvider();
            Assert.NotNull(sp.GetService<IApiKeyExtractor>());
            Assert.IsType<ApiKeyProblemDetailsFactory>(sp.GetService<IApiKeyErrorResultFactory>());
        }
    }
}

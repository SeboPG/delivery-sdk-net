using KenticoCloud.Delivery.Tests.DIFrameworks.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KenticoCloud.Delivery.Tests.DIFrameworks
{
    public class AutoFacTests
    {
        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromAutoFacContainer()
        {
            var provider = DependencyInjectionFrameworksHelper
                .GetServiceCollection()
                .BuildAutoFacServiceProvider();

            var client = (DeliveryClient)provider.GetService<IDeliveryClient>();

            client.AssertDefaultDependencies();
        }
        
        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromCastleWindsorContainer_CustomCodeFirstModelProvider()
        {
            var provider = DependencyInjectionFrameworksHelper
                .GetServiceCollection()
                .AddScoped<ICodeFirstModelProvider, FakeModelProvider>()
                .BuildAutoFacServiceProvider();

            var client = (DeliveryClient)provider.GetService<IDeliveryClient>();

            client.AssertDefaultDependenciesWithCustomCodeFirstModelProvider<FakeModelProvider>();
        }
    }
}

using KenticoCloud.Delivery.Tests.DIFrameworks.Helpers;
using Xunit;

namespace KenticoCloud.Delivery.Tests.DIFrameworks
{
    public class SimpleInjectorTests
    {
        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromSimpleInjectorContainer()
        {
            var container = DependencyInjectionFrameworksHelper.CreateAndConfigureSimpleInjectorContainer();

            var client = (DeliveryClient) container.GetInstance<IDeliveryClient>();

            client.AssertDefaultDependencies();
        }

        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromSimpleInjectorContainer_CustomCodeFirstModelProvider()
        {
            var container = DependencyInjectionFrameworksHelper
                .CreateAndConfigureSimpleInjectorContainerWithCustomModelProvider();
            container.Register<ICodeFirstModelProvider, FakeModelProvider>();

            var client = (DeliveryClient) container.GetInstance<IDeliveryClient>();

            client.AssertDefaultDependenciesWithCustomCodeFirstModelProvider<FakeModelProvider>();
        }
    }
}
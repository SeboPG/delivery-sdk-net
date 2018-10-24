using KenticoCloud.Delivery.CodeFirst;
using KenticoCloud.Delivery.ContentLinks;
using Xunit;

namespace KenticoCloud.Delivery.Tests.DependencyInjectionFrameworks
{
    internal static class DeliveryClientAssertionExtensions
    {
        private const string ProjectId = "00a21be4-8fef-4dd9-9380-f4cbb82e260d";

        internal static void AssertDefaultDependencies(this DeliveryClient client)
            => client.AssertDefaultDependenciesWithCustomCodeFirstModelProvider<CodeFirstModelProvider>();

        internal static void AssertDefaultDependenciesWithCustomCodeFirstModelProvider<TCustomCodeFirstModelProvider>(this DeliveryClient client)
            where TCustomCodeFirstModelProvider : ICodeFirstModelProvider
        {
            Assert.NotNull(client);
            Assert.NotNull(client.CodeFirstPropertyMapper);
            Assert.NotNull(client.CodeFirstTypeProvider);
            Assert.NotNull(client.ContentLinkUrlResolver);
            Assert.NotNull(client.DeliveryOptions);
            Assert.NotNull(client.CodeFirstModelProvider);
            Assert.Equal(ProjectId, client.DeliveryOptions.ProjectId);

            Assert.IsType<TCustomCodeFirstModelProvider>(client.CodeFirstModelProvider);
            Assert.IsType<DefaultTypeProvider>(client.CodeFirstTypeProvider);
            Assert.IsType<DefaultContentLinkUrlResolver>(client.ContentLinkUrlResolver);
        }
    }
}

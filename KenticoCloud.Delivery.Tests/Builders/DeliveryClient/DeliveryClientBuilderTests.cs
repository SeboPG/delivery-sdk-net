using System;
using FakeItEasy;
using KenticoCloud.Delivery.ResiliencePolicy;
using RichardSzalay.MockHttp;
using Xunit;

namespace KenticoCloud.Delivery.Tests
{
    public class DeliveryClientBuilderTests
    {
        private const string ProjectId = "e5629811-ddaa-4c2b-80d2-fa91e16bb264";
        private const string PreviewEndpoint = "https://preview-deliver.test.com/{0}";
        private const string PreviewApiKey =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3YjJlM2FiOTBjOGM0ODVmYjdmZTczNWY0ZGM1NDIyMCIsImlhdCI6IjE1Mjg4ODc2MzQiLCJleHAiOiIxODc0NDg3NjM0IiwicHJvamVjd" +
            "F9pZCI6IjEyMzQ1Njc5OGFiY2RibGFibGEiLCJ2ZXIiOiIxLjAuMCIsImF1ZCI6InByZXZpZXcuZGVsaXZlci5rZW50aWNvY2xvdWQuY29tIn0.wtSzbNDpbEHR2Bj4LUTGsdgesg4b693TFuhRCRsDyoc";

        private readonly Guid _guid = new Guid(ProjectId);

        [Fact]
        public void BuildWithProjectIdOnly_Guid_ReturnsDeliveryClientWithProjectIdSet()
        {
            var deliveryClient = (DeliveryClient) DeliveryClientBuilder.BuildWithProjectIdOnly(_guid);

            Assert.Equal(_guid.ToString(), deliveryClient.DeliveryOptions.ProjectId);
        }

        [Fact]
        public void BuildWithProjectIdOnly_String_ReturnsDeliveryClientWithProjectIdSet()
        {
            var deliveryClient = (DeliveryClient) DeliveryClientBuilder.BuildWithProjectIdOnly(_guid.ToString());

            Assert.Equal(_guid.ToString(), deliveryClient.DeliveryOptions.ProjectId);
        }

        [Fact]
        public void BuildWithDeliveryOptionsOnly_ReturnsDeliveryClientWithDeliveryOptions()
        {
            var deliveryClient = (DeliveryClient)DeliveryClientBuilder
                .BuildWithOptionsOnly(builder => builder.WithProjectId(_guid).UseProductionApi.Build());

            Assert.Equal(_guid.ToString(), deliveryClient.DeliveryOptions.ProjectId);
        }

        [Fact]
        public void BuildWithProjectId_ReturnsDeliveryClientWithProjectIdSet()
        {
            var deliveryClient = (DeliveryClient) DeliveryClientBuilder.WithProjectId(ProjectId).Build();

            Assert.Equal(ProjectId, deliveryClient.DeliveryOptions.ProjectId);
        }

        [Fact]
        public void BuildWithDeliveryOptions_ReturnsDeliveryClientWithDeliveryOptions()
        {
            var guid = new Guid(ProjectId);

            var deliveryClient = (DeliveryClient) DeliveryClientBuilder
                .WithOptions(builder => builder
                    .WithProjectId(guid)
                    .UsePreviewApi(PreviewApiKey)
                    .WithCustomEndpoint(PreviewEndpoint)
                    .Build()
                ).Build();

            Assert.Equal(ProjectId, deliveryClient.DeliveryOptions.ProjectId);
            Assert.Equal(PreviewEndpoint, deliveryClient.DeliveryOptions.PreviewEndpoint);
        }

        [Fact]
        public void BuildWithOptionalSteps_ReturnsDeliveryClientWithSetInstances()
        {
            var mockCodeFirstModelProvider = A.Fake<ICodeFirstModelProvider>();
            var mockResiliencePolicyProvider = A.Fake<IResiliencePolicyProvider>();
            var mockCodeFirstPropertyMapper = A.Fake<ICodeFirstPropertyMapper>();
            var mockHttp = new MockHttpMessageHandler().ToHttpClient();

            var deliveryClient = (DeliveryClient) DeliveryClientBuilder
                .WithProjectId(ProjectId)
                .WithHttpClient(mockHttp)
                .WithCodeFirstModelProvider(mockCodeFirstModelProvider)
                .WithCodeFirstPropertyMapper(mockCodeFirstPropertyMapper)
                .WithResiliencePolicyProvider(mockResiliencePolicyProvider)
                .Build();

            Assert.Equal(ProjectId, deliveryClient.DeliveryOptions.ProjectId);
            Assert.Equal(mockCodeFirstModelProvider, deliveryClient.CodeFirstModelProvider);
            Assert.Equal(mockCodeFirstPropertyMapper, deliveryClient.CodeFirstPropertyMapper);
            Assert.Equal(mockResiliencePolicyProvider, deliveryClient.ResiliencePolicyProvider);
            Assert.Equal(mockHttp, deliveryClient.HttpClient);
        }

        [Fact]
        public void BuildWithoutOptionalStepts_ReturnsDeliveryClientWithDefaultImplementations()
        {
            var deliveryClient = (DeliveryClient) DeliveryClientBuilder
                .WithProjectId(_guid)
                .Build();

            Assert.NotNull(deliveryClient.CodeFirstModelProvider);
            Assert.NotNull(deliveryClient.CodeFirstPropertyMapper);
            Assert.NotNull(deliveryClient.CodeFirstTypeProvider);
            Assert.NotNull(deliveryClient.ContentLinkUrlResolver);
            Assert.NotNull(deliveryClient.HttpClient);
            Assert.NotNull(deliveryClient.InlineContentItemsProcessor);
            Assert.NotNull(deliveryClient.ResiliencePolicyProvider);
        }
    }
}

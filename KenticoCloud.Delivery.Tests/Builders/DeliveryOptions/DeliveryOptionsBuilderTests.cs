using System;
using Xunit;

namespace KenticoCloud.Delivery.Tests.Configuration
{
    public class DeliveryOptionsBuilderTests
    {
        private const string ProjectId = "550cec62-90a6-4ab3-b3e4-3d0bb4c04f5c";
        private const string PreviewApiKey = 
            "eyJ0eXAiOiwq14X65DLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiABCjJlM2FiOTBjOGM0ODVmYjdmZTDEFRQZGM1NDIyMCIsImlhdCI6IjE1Mjg454wexiLCJleHAiOiIxODc0NDg3NjqasdfwicHJvamVjdF9pZCI6Ij" +
            "g1OTEwOTlkN2458198ewqewZjI3Yzg5M2FhZTJiNTE4IiwidmVyIjoiMS4wLjAiLCJhdWQiewqgsdaWV3LmRlbGl2ZXIua2VudGljb2Nsb3VkLmNvbSJ9.wtSzbNDpbE55dsaLUTGsdgesg4b693TFuhRCRsDyoc";

        private const string SecuredApiKey =
            "eyJ0eXAiOiwq14X65DLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiABCjJlM2FiOTBjOGM0ODVmYjdmZTDEFRQZGM1ND123QEwclhdCI6IjE1Mjg454wexiLCJleHAiOiIxODc0NDg3NjqasdfwicHJvamVjdF9pZCI6Ij" +
            "g1OTEwOTlkN2458198ewqewZjI3Yzg5M2FhZTJiNTE4IiwidmVyIjoiMS4wLjAiLCJhdWQiewqgsdaWV3LmRlbGl2ZXIua2VudGljb2Nsb3VkLmNvbSJ9.wtSzbNDpbE55dsaLUTGsdgesg4b693TFuhRCRsDyoc";
        private readonly Guid _guid = new Guid(ProjectId);

        [Fact]
        public void BuildWithProjectIdAndUseProductionApi()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(ProjectId)
                .UseProductionApi
                .Build();

            Assert.Equal(deliveryOptions.ProjectId, ProjectId);
            Assert.False(deliveryOptions.UsePreviewApi);
            Assert.False(deliveryOptions.UseSecuredProductionApi);
        }

        [Fact]
        public void BuildWithProjectIdAndPreviewApi()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid)
                .UsePreviewApi(PreviewApiKey)
                .Build();

            Assert.Equal(ProjectId, deliveryOptions.ProjectId);
            Assert.True(deliveryOptions.UsePreviewApi);
            Assert.Equal(PreviewApiKey, deliveryOptions.PreviewApiKey);
        }

        [Fact]
        public void BuildWithProjectIdAndSecuredProductionApi()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(ProjectId)
                .UseSecuredProductionApi(SecuredApiKey)
                .Build();

            Assert.Equal(ProjectId, deliveryOptions.ProjectId);
            Assert.True(deliveryOptions.UseSecuredProductionApi);
            Assert.Equal(SecuredApiKey, deliveryOptions.SecuredProductionApiKey);
        }

        [Fact]
        public void BuildWithMaxRetryAttempts()
        {
            const int maxRetryAttempts = 10;

            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(ProjectId)
                .UseProductionApi
                .WithMaxRetryAttempts(maxRetryAttempts)
                .Build();

            Assert.Equal(deliveryOptions.MaxRetryAttempts, maxRetryAttempts);
        }

        [Fact]
        public void BuildWithWaitForLoadingNewContent()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(Guid.NewGuid())
                .UseProductionApi
                .WaitForLoadingNewContent
                .Build();

            Assert.True(deliveryOptions.WaitForLoadingNewContent);
        }

        [Fact]
        public void BuildWithEnabledResilienceLogic()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(Guid.NewGuid())
                .UseProductionApi
                .EnableResilienceLogic
                .Build();

            Assert.True(deliveryOptions.EnableResilienceLogic);
        }

        [Fact]
        public void BuildWithCustomEndpointForPreviewApi()
        {
            const string customEndpoint = "http://www.customPreviewEndpoint.com";

            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(ProjectId)
                .UsePreviewApi(PreviewApiKey)
                .WithCustomEndpoint(customEndpoint)
                .Build();

           Assert.Equal(deliveryOptions.PreviewEndpoint, customEndpoint);
        }

        [Fact]
        public void BuildWithCustomEndpointForProductionApi()
        {
            const string customEndpoint = "https://www.customProductionEndpoint.com";

            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(ProjectId)
                .UseProductionApi
                .WithCustomEndpoint(customEndpoint)
                .Build();

            Assert.Equal(deliveryOptions.ProductionEndpoint, customEndpoint);
        }
    }
}

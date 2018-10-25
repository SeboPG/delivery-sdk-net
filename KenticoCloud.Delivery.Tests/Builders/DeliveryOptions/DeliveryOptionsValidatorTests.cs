using System;
using Xunit;

namespace KenticoCloud.Delivery.Tests.Builders.DeliveryOptions
{
    public class DeliveryOptionsValidatorTests
    {
        private readonly Guid _guid = Guid.NewGuid();
        
        [Fact]
        public void ValidateOptionsWithNegativeMaxRetryAttempts()
        {
            var deliveryOptions = new Delivery.DeliveryOptions
            {
                ProjectId = _guid.ToString(),
                MaxRetryAttempts = -10
            };

            Assert.Throws<ArgumentException>(() => deliveryOptions.Validate());
        }

        [Fact]
        public void ValidateOptionsWithEmptyProjectId()
        {
            var deliveryOptions = new Delivery.DeliveryOptions {ProjectId = ""};

            Assert.Throws<ArgumentException>(() => deliveryOptions.Validate());
        }

        [Fact]
        public void ValidateOptionsWithNullProjectId()
        {
            var deliveryOptions = new Delivery.DeliveryOptions { ProjectId = null };

            Assert.Throws<ArgumentNullException>(() => deliveryOptions.Validate());
        }

        [Theory]
        [InlineData("123-456")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void ValidateOptionsWithEmptyGuidProjectId(string projectId)
        {
            var deliveryOptions = new Delivery.DeliveryOptions { ProjectId = projectId };

            Assert.Throws<ArgumentException>(() => deliveryOptions.Validate());
        }

        [Fact]
        public void ValidateOptionsWithNullPreviewApiKeys()
        {
            var deliveryOptionsStep = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid);

            Assert.Throws<ArgumentNullException>(() => deliveryOptionsStep.UsePreviewApi(null));
        }

        [Fact]
        public void ValidateOptionsWithNullSecuredApiKeys()
        {
            var deliveryOptionsStep = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid);

            Assert.Throws<ArgumentNullException>(() => deliveryOptionsStep.UseSecuredProductionApi(null));
        }

        [Fact]
        public void ValidateOptionsBuiltWithBuilderWithIncorrectApiKeyFormat()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid);

            Assert.Throws<ArgumentException>(() => deliveryOptions.UsePreviewApi("badPreviewApiFormat"));
        }

        [Fact]
        public void ValidateOptionsUseOfPreviewAndProductionApiSimultaneously()
        {
            const string previewApiKey = "previewApiKey";
            const string productionApiKey = "productionApiKey";

            var deliveryOptions = new Delivery.DeliveryOptions
            {
                ProjectId = _guid.ToString(),
                UsePreviewApi = true,
                PreviewApiKey = previewApiKey,
                UseSecuredProductionApi = true,
                SecuredProductionApiKey = productionApiKey
            };

            Assert.Throws<InvalidOperationException>(() => deliveryOptions.Validate());
        }

        [Fact]
        public void ValidateOptionsWithEnabledPreviewApiWithSetKey()
        {
            var deliveryOptions = new Delivery.DeliveryOptions
            {
                ProjectId = _guid.ToString(),
                UsePreviewApi = true
            };

            Assert.Throws<InvalidOperationException>(() => deliveryOptions.Validate());
        }

        [Fact]
        public void ValidateOptionsWithEnabledSecuredApiWithSetKey()
        {
            var deliveryOptions = new Delivery.DeliveryOptions
            {
                ProjectId = _guid.ToString(),
                UseSecuredProductionApi = true
            };

            Assert.Throws<InvalidOperationException>(() => deliveryOptions.Validate());
        }

        [Theory]
        [InlineData("")]
        [InlineData("ftp://abc.com")]
        public void ValidateOptionsWithInvalidEndpointFormat(string endpoint)
        {
            var deliveryOptionsSteps = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid)
                .UseProductionApi;

            Assert.Throws<ArgumentException>(() => deliveryOptionsSteps.WithCustomEndpoint(endpoint));
        }

        [Fact]
        public void ValidateOptionsBuiltWithBuilderWithEmptyProjectId()
        {
            var deliveryOptionsSteps = DeliveryOptionsBuilder.CreateInstance();

            Assert.Throws<ArgumentException>(() => deliveryOptionsSteps.WithProjectId(Guid.Empty));
        }
    }
}

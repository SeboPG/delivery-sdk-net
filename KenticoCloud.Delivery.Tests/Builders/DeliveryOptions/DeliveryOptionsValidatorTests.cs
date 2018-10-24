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
            var deliveryOptions = new Delivery.DeliveryOptions();

            Assert.Throws<ArgumentNullException>(() => deliveryOptions.Validate());
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
        public void ValidateOptionsBuiltWithBuilderWithIncorrectApiKeyFormat()
        {
            var deliveryOptions = DeliveryOptionsBuilder
                .CreateInstance()
                .WithProjectId(_guid);

            Assert.Throws<ArgumentException>(() => deliveryOptions.UsePreviewApi("badPreviewApiFormat"));
        }

        [Fact]
        public void ValidateOptionsBuiltWithBuilderWithEmptyProjectId()
        {
            var deliveryOptions = DeliveryOptionsBuilder.CreateInstance();

            Assert.Throws<ArgumentException>(() => deliveryOptions.WithProjectId(Guid.Empty));
        }
    }
}

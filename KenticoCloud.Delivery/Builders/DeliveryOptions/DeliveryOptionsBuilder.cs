using System;
using KenticoCloud.Delivery.Builders.DeliveryOptions;

namespace KenticoCloud.Delivery
{
    public class DeliveryOptionsBuilder : IOptionsPreviewOrProductionSteps, IOptionsMandatorySteps, IOptionsSteps
    {
        private readonly DeliveryOptions _deliveryOptions = new DeliveryOptions();

        public static IOptionsMandatorySteps CreateInstance()
            => new DeliveryOptionsBuilder();

        private DeliveryOptionsBuilder() {}

        IOptionsPreviewOrProductionSteps IOptionsMandatorySteps.WithProjectId(string projectId)
        {
            DeliveryOptionsValidator.ValidateProjectId(projectId);
            _deliveryOptions.ProjectId = projectId;

            return this;
        }

        IOptionsPreviewOrProductionSteps IOptionsMandatorySteps.WithProjectId(Guid projectId)
        {
            DeliveryOptionsValidator.ValidateProjectId(projectId);
            _deliveryOptions.ProjectId = projectId.ToString();

            return this;
        }

        IOptionsSteps IOptionsSteps.WaitForLoadingNewContent
        {
            get
            {
                _deliveryOptions.WaitForLoadingNewContent = true;

                return this;
            }
        }

        IOptionsSteps IOptionsSteps.EnableResilienceLogic
        {
            get
            {
                _deliveryOptions.EnableResilienceLogic = true;

                return this;
            }
        }

        IOptionsSteps IOptionsSteps.WithMaxRetryAttempts(int attempts)
        {
            DeliveryOptionsValidator.ValidateMaxRetryAttempts(attempts);
            _deliveryOptions.MaxRetryAttempts = attempts;

            return this;
        }

        IOptionsSteps IOptionsPreviewOrProductionSteps.UsePreviewApi(string previewApiKey)
        {
            previewApiKey.ValidateApiKey(nameof(previewApiKey));
            _deliveryOptions.PreviewApiKey = previewApiKey;
            _deliveryOptions.UsePreviewApi = true;

            return this;
        }
        IOptionsSteps IOptionsPreviewOrProductionSteps.UseProductionApi
            => this;

        IOptionsSteps IOptionsPreviewOrProductionSteps.UseSecuredProductionApi(string securedProductionApiKey)
        {
            securedProductionApiKey.ValidateApiKey(nameof(securedProductionApiKey));
            _deliveryOptions.SecuredProductionApiKey = securedProductionApiKey;
            _deliveryOptions.UseSecuredProductionApi = true;

            return this;
        }

        IOptionsSteps IOptionsSteps.WithCustomEndpoint(string endpoint)
        {
            endpoint.ValidateCustomEndpoint();
            if (_deliveryOptions.UsePreviewApi)
            {
                _deliveryOptions.PreviewEndpoint = endpoint;
            }
            else
            {
                _deliveryOptions.ProductionEndpoint = endpoint;
            }

            return this;
        }

        DeliveryOptions IOptionsBuildStep.Build()
        {
            _deliveryOptions.Validate();

            return _deliveryOptions;
        }
    }
}

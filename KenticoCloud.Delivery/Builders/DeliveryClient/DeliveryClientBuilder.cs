using System;
using System.Net.Http;
using KenticoCloud.Delivery.Builders.DeliveryClient;
using KenticoCloud.Delivery.Builders.DeliveryOptions;
using KenticoCloud.Delivery.InlineContentItems;
using KenticoCloud.Delivery.ResiliencePolicy;
using Microsoft.Extensions.DependencyInjection;

namespace KenticoCloud.Delivery
{
    public delegate DeliveryOptions BuildDeliveryOptions(IOptionsMandatorySteps builder);

    public sealed class DeliveryClientBuilder
    {
        /// <summary>
        /// Mandatory step of the <see cref="DeliveryClientBuilder"/> for specifying Kentico Cloud project id.
        /// </summary>
        /// <param name="projectId">The identifier of the Kentico Cloud project.</param>
        public static IDeliveryClientOptionalSteps WithProjectId(string projectId)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithProjectId(projectId);

        /// <summary>
        /// Mandatory step of the <see cref="DeliveryClientBuilder"/> for specifying Kentico Cloud project id.
        /// </summary>
        /// <param name="projectId">The identifier of the Kentico Cloud project.</param>
        public static IDeliveryClientOptionalSteps WithProjectId(Guid projectId)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithProjectId(projectId);

        /// <summary>
        /// Mandatory step of the <see cref="DeliveryClientBuilder"/> for specifying Kentico Cloud project settings.
        /// </summary>
        /// <param name="buildDeliveryOptions">A function that returns <see cref="DeliveryOptions"/> instance which specifies the Kentico Cloud project settings.</param>
        public static IDeliveryClientOptionalSteps WithOptions(BuildDeliveryOptions buildDeliveryOptions)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithDeliveryOptions(buildDeliveryOptions);

        /// <remarks>
        /// To provide custom implementations of interfaces used in Kentico Cloud use <seealso cref="WithProjectId(string)"/>
        /// </remarks>
        /// <summary>
        /// Creates implementation of the <see cref="IDeliveryClient"/> on which you can invoke methods for retrieving content and its metadata from the Kentico Cloud Delivery service.
        /// </summary>
        /// <param name="projectId">The identifier of the Kentico Cloud project.</param>
        public static IDeliveryClient BuildWithProjectIdOnly(string projectId)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithProjectIdOnly(projectId);

        /// <remarks>
        /// To provide custom implementations of interfaces used in Kentico Cloud use <seealso cref="WithProjectId(string)"/>
        /// </remarks>
        /// <summary>
        /// Creates implementation of the <see cref="IDeliveryClient"/> on which you can invoke methods for retrieving content and its metadata from the Kentico Cloud Delivery service.
        /// </summary>
        /// <param name="projectId">The identifier of the Kentico Cloud project.</param>
        public static IDeliveryClient BuildWithProjectIdOnly(Guid projectId)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithProjectIdOnly(projectId);

        /// <remarks>
        /// To provide custom implementations of interfaces used in Kentico Cloud use <seealso cref="WithOptions"/>
        /// </remarks>
        /// <summary>
        /// Creates implementation of the <see cref="IDeliveryClient"/> on which you can invoke methods for retrieving content and its metadata from the Kentico Cloud Delivery service.
        /// </summary>
        /// <param name="buildDeliveryOptions">A function that returns <see cref="DeliveryOptions"/> instance which specifies the Kentico Cloud project settings.</param>
        public static IDeliveryClient BuildWithOptionsOnly(BuildDeliveryOptions buildDeliveryOptions)
            => DeliveryClientBuilderImplementation.MandatorySteps.BuildWithDeliveryOptionsOnly(buildDeliveryOptions);
    }

    internal sealed class DeliveryClientBuilderImplementation : IDeliveryClientMandatorySteps, IDeliveryClientOptionalSteps
    {
        internal static IDeliveryClientMandatorySteps MandatorySteps => new DeliveryClientBuilderImplementation();

        private readonly IServiceCollection _serviceCollection;
        private HttpClient _httpClient;
        private DeliveryOptions _deliveryOptions;

        private DeliveryClientBuilderImplementation()
        {
            _serviceCollection = new ServiceCollection();
        }

        public IDeliveryClientOptionalSteps BuildWithDeliveryOptions(BuildDeliveryOptions buildDeliveryOptions)
        {
            var builder = DeliveryOptionsBuilder.CreateInstance();

            _deliveryOptions = buildDeliveryOptions(builder);

            return this;
        }

        public IDeliveryClientOptionalSteps BuildWithProjectId(string projectId)
            => BuildWithDeliveryOptions(builder => builder.WithProjectId(projectId).UseProductionApi.Build());

        public IDeliveryClientOptionalSteps BuildWithProjectId(Guid projectId)
            => BuildWithDeliveryOptions(builder => builder.WithProjectId(projectId).UseProductionApi.Build());

        IDeliveryClient IDeliveryClientMandatorySteps.BuildWithProjectIdOnly(string projectId)
            => BuildWithProjectId(projectId).Build();

        IDeliveryClient IDeliveryClientMandatorySteps.BuildWithProjectIdOnly(Guid projectId)
            => BuildWithProjectId(projectId).Build();

        IDeliveryClient IDeliveryClientMandatorySteps.BuildWithDeliveryOptionsOnly(
            BuildDeliveryOptions buildDeliveryOptions)
            => BuildWithDeliveryOptions(buildDeliveryOptions).Build();

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ??
                          throw new ArgumentNullException(nameof(httpClient), "Http client is not specified");

            _serviceCollection.AddSingleton(_httpClient);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithContentLinkUrlResolver(
            IContentLinkUrlResolver contentLinkUrlResolver)
        {
            if (contentLinkUrlResolver == null)
            {
                throw new ArgumentNullException(nameof(contentLinkUrlResolver));
            }

            _serviceCollection.AddSingleton(contentLinkUrlResolver);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithInlineContentItemsProcessor(
            IInlineContentItemsProcessor inlineContentItemsProcessor)
        {
            if (inlineContentItemsProcessor == null)
            {
                throw new ArgumentNullException(nameof(inlineContentItemsProcessor));
            }

            _serviceCollection.AddSingleton(inlineContentItemsProcessor);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithCodeFirstModelProvider(
            ICodeFirstModelProvider codeFirstModelProvider)
        {
            if (codeFirstModelProvider == null)
            {
                throw new ArgumentNullException(nameof(codeFirstModelProvider));
            }

            _serviceCollection.AddSingleton(codeFirstModelProvider);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithCodeFirstTypeProvider(
            ICodeFirstTypeProvider codeFirstTypeProvider)
        {
            if (codeFirstTypeProvider == null)
            {
                throw new ArgumentNullException(nameof(codeFirstTypeProvider));
            }

            _serviceCollection.AddSingleton(codeFirstTypeProvider);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithResiliencePolicyProvider(
            IResiliencePolicyProvider resiliencePolicyProvider)
        {
            if (resiliencePolicyProvider == null)
            {
                throw new ArgumentNullException(nameof(resiliencePolicyProvider));
            }

            _serviceCollection.AddSingleton(resiliencePolicyProvider);

            return this;
        }

        IDeliveryClientOptionalSteps IDeliveryClientOptionalSteps.WithCodeFirstPropertyMapper(
            ICodeFirstPropertyMapper propertyMapper)
        {
            if (propertyMapper == null)
            {
                throw new ArgumentNullException(nameof(propertyMapper));
            }

            _serviceCollection.AddSingleton(propertyMapper);

            return this;
        }

        IDeliveryClient IDeliveryClientBuildStep.Build()
        {
            _serviceCollection.AddDeliveryClient(_deliveryOptions);

            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var client = serviceProvider.GetService<IDeliveryClient>();

            return client;
        }
    }
}

using System;
using System.Net.Http;
using FakeItEasy;
using KenticoCloud.Delivery.InlineContentItems;
using KenticoCloud.Delivery.ResiliencePolicy;
using Microsoft.Extensions.Options;
using Polly;
using RichardSzalay.MockHttp;

namespace KenticoCloud.Delivery.Tests.Factories
{
    internal static class DeliveryClientFactories
    {
        private static readonly Guid Guid = Guid.NewGuid();
        private static readonly MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();
        private static ICodeFirstModelProvider _mockCodeFirstModelProvider = A.Fake<ICodeFirstModelProvider>();
        private static ICodeFirstPropertyMapper _mockCodeFirstPropertyMapper = A.Fake<ICodeFirstPropertyMapper>();
        private static IResiliencePolicyProvider _mockResiliencePolicyProvider = A.Fake<IResiliencePolicyProvider>();
        private static ICodeFirstTypeProvider _mockCodeFirstTypeProvider = A.Fake<ICodeFirstTypeProvider>();
        private static IContentLinkUrlResolver _mockContentLinkUrlResolver = A.Fake<IContentLinkUrlResolver>();
        private static IInlineContentItemsProcessor _mockInlineContentItemsProcessor = A.Fake<IInlineContentItemsProcessor>();

        internal static DeliveryClient InitializeDeliveryClientWithACustomTypeProvider(MockHttpMessageHandler httpMessageHandler = null)
        {
            var customTypeProvider = new CustomTypeProvider();
            var codeFirstModelProvider = new CodeFirstModelProvider(_mockContentLinkUrlResolver, null, customTypeProvider, new CodeFirstPropertyMapper());
            var client = GetMockedDeliveryClientWithProjectId(Guid, httpMessageHandler, codeFirstModelProvider, codeFirstTypeProvider: customTypeProvider);

            //A.CallTo(() => client.ResiliencePolicyProvider.Policy)
            //    .Returns(Policy.HandleResult<HttpResponseMessage>(result => true).RetryAsync(client.DeliveryOptions.MaxRetryAttempts));

            return client;
        }

        internal static DeliveryClient InitializeDeliveryClientWithCustomModelProvider(MockHttpMessageHandler httpMessageHandler = null, ICodeFirstPropertyMapper propertyMapper = null)
        {
            var codeFirstPropertyMapper = propertyMapper ?? _mockCodeFirstPropertyMapper;
            var codeFirstModelProvider = new CodeFirstModelProvider(null, null, _mockCodeFirstTypeProvider, codeFirstPropertyMapper);
            var client = GetMockedDeliveryClientWithProjectId(Guid, httpMessageHandler, codeFirstModelProvider);

            A.CallTo(() => client.ResiliencePolicyProvider.Policy)
                .Returns(Policy.HandleResult<HttpResponseMessage>(result => true).RetryAsync(client.DeliveryOptions.MaxRetryAttempts));
            return client;
        }

        internal static DeliveryClient GetMockedDeliveryClientWithProjectId(
            Guid projectId,
            MockHttpMessageHandler httpMessageHandler = null,
            ICodeFirstModelProvider codeFirstModelProvider = null,
            ICodeFirstPropertyMapper codeFirstPropertyMapper = null,
            IResiliencePolicyProvider resiliencePolicyProvider = null,
            ICodeFirstTypeProvider codeFirstTypeProvider = null,
            IContentLinkUrlResolver contentLinkUrlResolver = null,
            IInlineContentItemsProcessor inlineContentItemsProcessor = null
        )
        {
            if (codeFirstModelProvider != null) _mockCodeFirstModelProvider = codeFirstModelProvider;
            if (codeFirstPropertyMapper != null) _mockCodeFirstPropertyMapper = codeFirstPropertyMapper;
            if (resiliencePolicyProvider != null) _mockResiliencePolicyProvider = resiliencePolicyProvider;
            if (codeFirstTypeProvider != null) _mockCodeFirstTypeProvider = codeFirstTypeProvider;
            if (contentLinkUrlResolver != null) _mockContentLinkUrlResolver = contentLinkUrlResolver;
            if (inlineContentItemsProcessor != null) _mockInlineContentItemsProcessor = inlineContentItemsProcessor;
            var httpClient = httpMessageHandler != null ? httpMessageHandler.ToHttpClient() : MockHttp.ToHttpClient();

            var client = new DeliveryClient(
                Options.Create(new DeliveryOptions { ProjectId = projectId.ToString() }),
                httpClient,
                _mockContentLinkUrlResolver,
                _mockInlineContentItemsProcessor,
                _mockCodeFirstModelProvider,
                _mockResiliencePolicyProvider,
                _mockCodeFirstTypeProvider,
                _mockCodeFirstPropertyMapper
            );
            

            return client;
        }
    }
}

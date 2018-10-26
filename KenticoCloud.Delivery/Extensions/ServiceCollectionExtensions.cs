﻿using System;
using System.Net.Http;
using KenticoCloud.Delivery.CodeFirst;
using KenticoCloud.Delivery.ContentLinks;
using KenticoCloud.Delivery.InlineContentItems;
using KenticoCloud.Delivery.ResiliencePolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace KenticoCloud.Delivery
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a DeliveryClient instance to an IDeliveryClient interface in <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">A <see cref="ServiceCollection"/> instance for registering and resolving dependencies.</param>
        /// <param name="buildDeliveryOptions">A function that creates a <see cref="DeliveryOptions"/> instance.</param>
        /// <returns></returns>
        public static IServiceCollection AddDeliveryClient(this IServiceCollection services, BuildDeliveryOptions buildDeliveryOptions)
        {
            if (buildDeliveryOptions == null)
            {
                throw new ArgumentNullException(nameof(buildDeliveryOptions), "The function for creating Delivery options is null.");
            }

            return services
                .BuildOptions(buildDeliveryOptions)
                .RegisterDependencies();
        }

        /// <summary>
        /// Registers a DeliveryClient instance to an IDeliveryClient interface in <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">A <see cref="ServiceCollection"/> instance for registering and resolving dependencies.</param>
        /// <param name="deliveryOptions">A <see cref="DeliveryOptions"/> instance.</param>
        /// <returns></returns>
        public static IServiceCollection AddDeliveryClient(this IServiceCollection services, DeliveryOptions deliveryOptions)
        {
            if (deliveryOptions == null)
            {
                throw new ArgumentNullException(nameof(deliveryOptions), "The Delivery options object is not specified.");
            }

            return services
                .RegisterOptions(deliveryOptions)
                .RegisterDependencies();
        }

        /// <summary>
        /// Registers a DeliveryClient instance to an IDeliveryClient interface in <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">A <see cref="ServiceCollection"/> instance for registering and resolving dependencies.</param>
        /// <param name="configuration">a set of key/value application configuration properties.</param>
        /// <param name="configurationSectionName">The section name of the configuration that keeps the <see cref="DeliveryOptions"/> properties.</param>
        /// <returns></returns>
        public static IServiceCollection AddDeliveryClient(this IServiceCollection services, IConfiguration configuration, string configurationSectionName = "DeliveryOptions") 
            => services
                .LoadOptionsConfiguration(configuration, configurationSectionName)
                .RegisterDependencies();

        private static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.TryAddSingleton<IContentLinkUrlResolver, DefaultContentLinkUrlResolver>();
            services.TryAddSingleton<ICodeFirstTypeProvider, DefaultTypeProvider>();
            services.TryAddSingleton(new HttpClient());
            services.TryAddSingleton<IInlineContentItemsResolver<object>, ReplaceWithWarningAboutRegistrationResolver>();
            services.TryAddSingleton<IInlineContentItemsResolver<UnretrievedContentItem>, ReplaceWithWarningAboutUnretrievedItemResolver>();
            services.TryAddSingleton<IInlineContentItemsProcessor, InlineContentItemsProcessor>();
            services.TryAddSingleton<ICodeFirstModelProvider, CodeFirstModelProvider>();
            services.TryAddSingleton<ICodeFirstPropertyMapper, CodeFirstPropertyMapper>();
            services.TryAddSingleton<IResiliencePolicyProvider, DefaultResiliencePolicyProvider>();
            services.TryAddSingleton<IDeliveryClient, DeliveryClient>();

            return services;
        }

        private static IServiceCollection RegisterOptions(this IServiceCollection services, DeliveryOptions options)
        {
            services.TryAddSingleton(Options.Create(options));

            return services;
        }

        private static IServiceCollection LoadOptionsConfiguration(this IServiceCollection services, IConfiguration configuration, string configurationSectionName)
            => services
                .Configure<DeliveryOptions>(configurationSectionName == null 
                    ? configuration
                    : configuration.GetSection(configurationSectionName));

        private static IServiceCollection BuildOptions(this IServiceCollection services, BuildDeliveryOptions buildDeliveryOptions)
        {
            var builder = DeliveryOptionsBuilder.CreateInstance();
            var options = buildDeliveryOptions(builder);

            return services.RegisterOptions(options);
        }
    }
}

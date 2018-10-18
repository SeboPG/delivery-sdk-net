﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace KenticoCloud.Delivery.Tests.DIFrameworks.Helpers
{
    internal static class DependencyInjectionFrameworksHelper
    {
        private const string ProjectId = "00a21be4-8fef-4dd9-9380-f4cbb82e260d";

        internal static IServiceCollection GetServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDeliveryClient(new DeliveryOptions { ProjectId = ProjectId });

            return serviceCollection;
        }

        internal static IServiceProvider BuildAutoFacServiceProvider(this IServiceCollection serviceCollection)
        {
            var autoFacContainerBuilder = new ContainerBuilder();
            autoFacContainerBuilder.Populate(serviceCollection);

            var container = autoFacContainerBuilder.Build();

            return new AutofacServiceProvider(container);
        }

        internal static IServiceProvider BuildWindsorCastleServiceProvider(this IServiceCollection serviceCollection)
        {
            var castleContainer = new WindsorContainer();

            return WindsorRegistrationHelper.CreateServiceProvider(castleContainer, serviceCollection);
        }

        internal static IServiceProvider BuildUnityServiceProvider(this IServiceCollection serviceCollection)
        {
            var unityContainer = new UnityContainer();

            return serviceCollection.BuildServiceProvider(unityContainer);
        }

        internal static Container CreateAndConfigureSimpleInjectorContainer()
        {
            var serviceCollection = GetServiceCollection();
            var container = new Container();

            var appBuilder = new FakeApplicationBuilder
            {
                ApplicationServices = serviceCollection.BuildServiceProvider()
            };

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            serviceCollection.EnableSimpleInjectorCrossWiring(container);
            serviceCollection.UseSimpleInjectorAspNetRequestScoping(container);
            container.AutoCrossWireAspNetComponents(appBuilder);

            return container;
        }

        internal static Container CreateAndConfigureSimpleInjectorContainerWithCustomModelProvider()
        {
            var serviceCollection = GetServiceCollection();
            serviceCollection.AddScoped<ICodeFirstModelProvider, FakeModelProvider>();
            var container = new Container();

            var appBuilder = new FakeApplicationBuilder
            {
                ApplicationServices = serviceCollection.BuildServiceProvider()
            };

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            serviceCollection.EnableSimpleInjectorCrossWiring(container);
            serviceCollection.UseSimpleInjectorAspNetRequestScoping(container);
            container.AutoCrossWireAspNetComponents(appBuilder);

            return container;
        }
    }
}   

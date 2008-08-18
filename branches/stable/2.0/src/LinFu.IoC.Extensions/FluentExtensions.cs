using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Extensions;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    public static class FluentExtensions
    {
        /// <summary>
        /// Injects a <typeparamref name="TService"/> type
        /// into a <paramref name="container"/> using the
        /// given <paramref name="serviceName"/>
        /// </summary>
        /// <typeparam name="TService">The type of service to inject.</typeparam>
        /// <param name="container">The container that will hold the actual service service instance.</param>
        /// <param name="serviceName">The name of the service to create.</param>
        /// <returns>A non-null <see cref="IUsingLambda{TService}"/> instance.</returns>
        public static IUsingLambda<TService> Inject<TService>(this IServiceContainer container, string serviceName)
        {
            var context = new InjectionContext<TService>
            {
                ServiceName = serviceName,
                Container = container
            };

            return new UsingLambda<TService>(context);
        }

        /// <summary>
        /// Injects a <typeparamref name="TService"/> type
        /// into a <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to inject.</typeparam>
        /// <param name="container">The container that will hold the actual service service instance.</param>
        /// <returns>A non-null <see cref="IUsingLambda{TService}"/> instance.</returns>
        public static IUsingLambda<TService> Inject<TService>(this IServiceContainer container)
        {
            var context = new InjectionContext<TService>
            {
                ServiceName = string.Empty,
                Container = container
            };

            return new UsingLambda<TService>(context);
        }

        /// <summary>
        /// Converts a <see cref="Func{Type, IServiceContainer, TService}"/>
        /// lambda into an equivalent <see cref="Func{Type, IContainer, TService}"/>
        /// instance.
        /// </summary>
        /// <typeparam name="TService">The type of service to create.</typeparam>
        /// <param name="func">The lambda function to be converted.</param>
        /// <returns>The equivalent <see cref="Func{Type, IContainer, TService}"/>
        /// that delegates its calls back to the <paramref name="func"/> lambda function.</returns>
        internal static Func<Type, IContainer, TService> CreateAdapter<TService>(this Func<Type, IServiceContainer, TService> func)
        {
            Func<Type, IContainer, TService> adapter = (type, container) =>
            {
                var serviceContainer = container as IServiceContainer;
                return func(type, serviceContainer);
            };

            return adapter;
        }
    }
}

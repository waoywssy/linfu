using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.Finders;
using LinFu.IoC.Extensions.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IArgumentResolver"/> class.
    /// </summary>
    public class ArgumentResolver : IArgumentResolver
    {
        /// <summary>
        /// Generates constructor arguments from the given <paramref name="constructor"/>
        /// and <paramref name="container"/>.
        /// </summary>
        /// <param name="constructor">The constructor that will be used to instantiate an object instance.</param>
        /// <param name="container">The container that will provide the constructor arguments.</param>
        /// <returns>An array of objects that represent the arguments to be passed to the target constructor.</returns>
        public object[] ResolveFrom(ConstructorInfo constructor, IServiceContainer container)
        {
            var enumerableDefinition = typeof(IEnumerable<>);

            var argumentList = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                var parameterType = parameter.ParameterType;
                object currentArgument = null;

                // Determine if the parameter type is an IEnumerable<T> type
                // and generate the list if necessary
                if (parameterType.IsGenericType &&
                    parameterType.GetGenericTypeDefinition() == enumerableDefinition)
                {
                    AddListArgument(parameterType, container, argumentList);
                    continue;
                }

                // Instantiate the service type and build
                // the argument list
                currentArgument = container.GetService(parameterType);
                argumentList.Add(currentArgument);
            }

            return argumentList.ToArray();
        }

        /// <summary>
        /// Determines whether or not a parameter type is an existing
        /// list of available services and automatically constructs the
        /// service list and adds it to the <paramref name="argumentList"/>.
        /// </summary>
        /// <param name="parameterType">The current constructor parameter type.</param>
        /// <param name="container">The container that will provide the argument values.</param>
        /// <param name="argumentList">The list that will hold the arguments to be passed to the constructor.</param>
        private static void AddListArgument(Type parameterType, IServiceContainer container, ICollection<object> argumentList)
        {
            var elementType = parameterType.GetGenericArguments()[0];
            var baseElementDefinition = elementType.IsGenericType
                                            ? elementType.GetGenericTypeDefinition()
                                            : null;

            // There has to be at least one service
            Func<IServiceInfo, bool> condition =
                info => info.ServiceType == elementType;

            // If the element is a generic type,
            // we need to check for any available generic factory
            // instances that might be able to create the element type
            if (baseElementDefinition != null)
                condition = condition.Or(info => info.ServiceType == baseElementDefinition);

            if (!container.Contains(condition))
                return;

            var serviceList = new List<object>();

            // Build the IEnumerable<> list of services
            // that match the gvien condition
            var services = container.GetServices(condition);
            foreach (var service in services)
            {
                serviceList.Add(service);
            }

            object currentArgument = serviceList.AsEnumerable();
            argumentList.Add(currentArgument);
        }
    }
}

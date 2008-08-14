using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a loader class that can load
    /// <see cref="ILoaderPlugin{TTarget}"/> instances
    /// marked with a particular <typeparamref name="TAttribute"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">The target type being configured.</typeparam>
    /// <typeparam name="TAttribute">The attribute type that marks a type as a plugin type.</typeparam>
    public class PluginLoader<TTarget, TAttribute> : IActionLoader<ILoader<TTarget>, Type>
        where TAttribute : Attribute
    {        
        /// <summary>
        /// Determines if the PluginLoader can load the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">The target type that might contain the target <typeparamref name="TAttribute"/> instance.</param>
        /// <returns><c>true</c> if the type can be loaded; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {            
            var attributes = inputType.GetCustomAttributes(typeof (TAttribute), true)
                .Cast<TAttribute>();

            // The type must have a default constructor
            var defaultConstructor = inputType.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                return false;

            // The target must implement the ILoaderPlugin<TTarget> interface
            // and be marked with the custom attribute
            return attributes.Count() > 0 && 
                typeof (ILoaderPlugin<TTarget>).IsAssignableFrom(inputType);
        }

        /// <summary>
        /// Loads a set of actions from a <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="input">The target type to scan.</param>
        /// <returns>A set of actions which will be applied to the target <see cref="ILoader{T}"/> instance.</returns>
        public IEnumerable<Action<ILoader<TTarget>>> Load(Type input)
        {
            // Create the plugin instance
            var plugin = Activator.CreateInstance(input) as ILoaderPlugin<TTarget>;

            if (plugin == null)
                return new Action<ILoader<TTarget>>[0];

            // Assign it to the target loader
            Action<ILoader<TTarget>> result = loader => loader.Plugins.Add(plugin);

            // Package it into an array and return the result
            return new []{result};
        }
    }
}

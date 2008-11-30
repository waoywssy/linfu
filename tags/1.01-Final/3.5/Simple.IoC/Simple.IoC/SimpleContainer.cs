using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Simple.IoC
{
    public class SimpleContainer : IContainer
    {
        private readonly Dictionary<Type, object> _factories = new Dictionary<Type, object>();
        private readonly List<ICustomizeInstance> _customizers = new List<ICustomizeInstance>();
        private readonly List<IPropertyInjector> _propertyInjectors = new List<IPropertyInjector>();
        private readonly List<ITypeInjector> _injectors = new List<ITypeInjector>();
        private readonly List<ITypeSurrogate> _surrogates = new List<ITypeSurrogate>();
        private INamedFactoryStorage _storage = new DefaultNamedFactoryStorage();

        private static readonly MethodInfo _getServiceMethod;
        private static readonly MethodInfo _getNamedServiceMethod;
        static SimpleContainer()
        {

            foreach (MethodInfo method in typeof(IContainer).GetMethods())
            {
                if (method == null)
                    continue;

                if (method.Name != "GetService" || !method.IsGenericMethod)
                    continue;

                if (method.ReturnType != typeof(object))
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                int parameterCount = parameters == null ? 0 : parameters.Length;
                if (parameterCount == 0)
                    continue;

                // Find the generic GetService() method
                if (parameterCount == 1 && parameters[0].ParameterType == typeof(Type))
                    _getServiceMethod = method;

                // Find the generic GetService() method that uses
                // named services
                if (parameterCount == 2)
                    _getNamedServiceMethod = method;
            }
        }
        public SimpleContainer()
        {
            _propertyInjectors.Add(new DefaultPropertyInjector());
        }      

        #region IContainer Members
        public void AddFactory<T>(IFactory<T> factory)
        {
            _factories[typeof (T)] = factory;
        }
        public void AddFactory(Type itemType, IFactory factory)
        {
#if DEBUG
            Console.WriteLine("Adding Factory for type '{0}'", itemType.FullName);
#endif
            _factories[itemType] = factory;
        }
        public bool Contains(Type serviceType)
        {
            MethodInfo getServiceDefinition = typeof(SimpleContainer).GetMethod("GetService",
                BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(bool) }, null);

            // This is equivalent to: return GetService<T>() != null;
            MethodInfo getService = getServiceDefinition.MakeGenericMethod(serviceType);
            object result = getService.Invoke(this, new object[] { false });

            return result != null;
        }
        public bool Contains(string serviceName, Type serviceType)
        {
            MethodInfo getServiceDefinition = typeof(SimpleContainer).GetMethod("GetService",
                BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);

            // This is equivalent to: return GetService<T>(serviceName) != null;
            MethodInfo getService = getServiceDefinition.MakeGenericMethod(serviceType);
            object result = getService.Invoke(this, new object[] { serviceName });

            return result != null;
        }
        public virtual T GetService<T>() where T : class
        {
            return GetService<T>(true);
        }

        public virtual T GetService<T>(string serviceName) where T : class
        {
            T result = null;

            if (_storage == null || string.IsNullOrEmpty(serviceName))
            {
                // Use the nameless implementation by default
                result = GetService<T>(false);

                result = PostProcess<T>(string.Empty, result, true);

                return result;
            }

            // Use the named factory instance, if possible
            if (_storage.ContainsFactory<T>(serviceName))
            {
                IFactory<T> factory = _storage.Retrieve<T>(serviceName);
                result = factory.CreateInstance(this);
            }

            if (result == null)
                result = CreateInstance<T>();

            result = PostProcess(serviceName, result, true);

            if (result == null)
                throw new ServiceNotFoundException(serviceName, typeof(T));

            return result;
        }

        private ICustomizeInstance GetCustomizer<T>(string serviceName) where T : class
        {
            // Search for a customizer for this current service type
            ICustomizeInstance targetCustomizer = null;
            foreach (ICustomizeInstance customizer in _customizers)
            {
                if (customizer == null)
                    continue;

                if (!customizer.CanCustomize(serviceName, typeof(T), this))
                    continue;

                targetCustomizer = customizer;
                break;
            }
            return targetCustomizer;
        }
        public virtual T GetService<T>(bool throwOnError) where T : class
        {
            T result = null;

            result = CreateInstance<T>();

            return PostProcess(string.Empty, result, throwOnError);
        }

        public INamedFactoryStorage NamedFactoryStorage
        {
            get
            {
                return _storage; 
            }
            set
            {
                _storage = value;
            }
        }
        public virtual IList<ICustomizeInstance> Customizers
        {
            get { return _customizers; }
        }
        public virtual IList<ITypeInjector> TypeInjectors
        {
            get { return _injectors; }
        }
        public virtual IList<ITypeSurrogate> TypeSurrogates
        {
            get { return _surrogates; }
        }

        private T PostProcess<T>(string serviceName, T originalResult, bool throwOnError) where T : class
        {
            Type serviceType = typeof(T);
            T result = originalResult;
            
            if (result == null && _surrogates.Count > 0)
            {
                // Find a surrogate for the given type
                foreach (ITypeSurrogate surrogate in _surrogates)
                {
                    if (surrogate == null)
                        continue;

                    if (!surrogate.CanSurrogate(serviceName, serviceType))
                        continue;

                    result = surrogate.ProvideSurrogate(serviceName, serviceType) as T;
                }
            }

            if (result == null && throwOnError)
                throw new ServiceNotFoundException(serviceType);

            // Allow external clients to inject
            // properties into the objects as they see fit
            foreach (IPropertyInjector propertyInjector in _propertyInjectors)
            {
                // HACK: Skip property injection if the result is null
                if (result == null)
                    break;

                if (propertyInjector == null || !propertyInjector.CanInject(result, this))
                    continue;

                propertyInjector.InjectProperties(result, this);
            }

            ICustomizeInstance targetCustomizer = GetCustomizer<T>(serviceName);
            if (targetCustomizer != null && result != null)
                targetCustomizer.Customize(serviceName, typeof(T), result, this);

            if (TypeInjectors.Count == 0)
                return result;

            foreach (ITypeInjector currentInjector in TypeInjectors)
            {
                object currentResult = null;
                // Allow third-party users to intercept instances
                // returned from this container            
                if (!typeof(ITypeInjector).IsAssignableFrom(serviceType) && currentInjector.CanInject(serviceType, result))
                    currentResult = currentInjector.Inject(serviceType, result);

                // Make sure that the result is always a valid reference
                if (currentResult == null)
                    continue;

                result = (T)currentResult;
            }

            return result;
        }
        private T CreateInstance<T>() where T : class
        {
            Type serviceType = typeof(T);
            if (!_factories.ContainsKey(serviceType))
                return null;

            // Retrieve the factory
            object factoryInstance = _factories[serviceType];
            IFactory<T> factory = factoryInstance as IFactory<T>;
            T result = null;

            // Instantiate the object
            if (factory != null)
                result = factory.CreateInstance(this);

            // Use the non-generic IFactory instance if it doesn't work
            if (factory == null && factoryInstance is IFactory)
            {
                IFactory otherFactory = factoryInstance as IFactory;
                result = otherFactory.CreateInstance(this) as T;
            }
            
            if (result == null)
                return null;

            if (_propertyInjectors.Count == 0)
                return result;
            

            return result;
        }

        public void AddService<T>(T serviceInstance)
        {
            Type serviceType = typeof (T);
            _factories[serviceType] = new InstanceFactory<T>(serviceInstance);
        }
        public void AddService<T>(string serviceName, T serviceInstance)
        {
            _storage.Store<T>(serviceName, new InstanceFactory<T>(serviceInstance));
        }
        public IList<IPropertyInjector> PropertyInjectors
        {
            get { return _propertyInjectors; }
        }

        public object GetService(Type serviceType)
        {
            if (_getServiceMethod == null)
                throw new NotImplementedException();
            
            MethodInfo getServiceMethod = _getServiceMethod.MakeGenericMethod(serviceType);
            return getServiceMethod.Invoke(this, new object[] { serviceType });
        }

        public object GetService(Type serviceType, string serviceName)
        {
            if (_getNamedServiceMethod == null)
                throw new NotImplementedException();

            MethodInfo getNamedServiceMethod = _getNamedServiceMethod.MakeGenericMethod(serviceType);
            return getNamedServiceMethod.Invoke(this, new object[] { serviceType, serviceName });
        }
        #endregion
    }
}

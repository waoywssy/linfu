using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LinFu.IOC.Factories
{
    public class OncePerThreadFactory<T> : BaseFactory<T>
    {
        private readonly Func<IContainer, T> _createInstance;
        private static Dictionary<int, T> _storage = new Dictionary<int, T>();
        public OncePerThreadFactory(Func<IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        public override T CreateInstance(IContainer container)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            T result = default(T);
            lock (_storage)
            {
                // Create the service instance only once
                if (!_storage.ContainsKey(threadId))
                    _storage[threadId] = _createInstance(container);

                result = _storage[threadId];
            }

            return result;
        }
    }
}

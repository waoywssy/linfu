# Introduction #

LinFu's IoC container allows you to automatically inject services into a _ServiceContainer_ instance using a single _ImplementsAttribute_ declaration declared on a target class. For example, let's suppose that I have a service type named _IVehicle_ that I want exposed to the container:

```
    public interface IVehicle
    {
        void Move();
        void Park();
        IPerson Driver { get; set; }
        IEngine Engine { get; set; }
    }
```

...and let's further suppose that I wanted to expose the _Car_ class to the container so that it can implement the _IVehicle_ interface. Here's the _Car_ class declaration:

```
    [Implements(typeof(IVehicle), LifecycleType.OncePerRequest)]
    public class Car : IVehicle, IInitialize
    {
        private IEngine _engine;
        private IPerson _person;

        public IEngine Engine
        {
            get { return _engine; }
            set { _engine = value; }
        }
        public IPerson Driver
        {
            get { return _person; }
            set { _person = value; }
        }
        public void Move()
        {
            if (_engine == null || _person == null)
                return;

            _engine.Start();
            Console.WriteLine("{0} says: I’m moving!", _person.Name);
        }
        public void Park()
        {
            if (_engine == null || _person == null)
                return;

            _engine.Stop();
            Console.WriteLine("{0} says: I’m parked!", _person.Name);
        }


        public void Initialize(IServiceContainer container)
        {
            _engine = container.GetService<IEngine>();
            _person = container.GetService<IPerson>();
        }
    }
```

As you can see, the _Car_ class implementation is relatively straightforward, aside from the _ImplementsAttribute_ declaration, and the _IInitialize_ implementation. The _ImplementsAttribute_ declaration tells LinFu's IoC container loader to use the _Car_ class to implement the _IVehicle_ interface, and it also tells the loader to create a brand new _Car_ instance every time the _IVehicle_ service type is requested.

## Service Initialization ##

The _Initialize()_ method implementation of the _IInitialize_ interface allows us to initialize the _Car_ class with the respective _IEngine_ and _IPerson_ services that currently reside in the container. This feature allows us to inject properties into our own services without having to worry about the implementation details of the container instance itself.

## Service Lifecycle Types ##

In general, LinFu's IoC container supports the following lifecycle types:
  * OncePerRequest: A new instance is created on every service request.
  * OncePerThread: Only one service instance is created per thread.
  * Singleton: One and only one service instance is created regardless of the number of new requests.

## Loading Services into the Container ##

Assuming that the _Car_ class is located in an assembly named _CarLibrary.dll_, all we need to do to load the _Car_ implementation of the _IVehicle_ interface into the container is:

```
    string directory = AppDomain.CurrentDomain.BaseDirectory;
    var container = new ServiceContainer();

    // Load CarLibrary.dll; If you need load
    // all the libaries in a directory, use "*.dll" instead
    container.LoadFrom(directory, "CarLibrary.dll");
```

The container will automatically load the _Car_ implementation from _CarLibrary.dll_ into the container and manage the lifetime of each service. The only thing left for us to do at this point is to use the actual _IVehicle_ service itself:

```
    // Use the car instance
    IVehicle vehicle = container.GetService<IVehicle>();

    // Do something useful with the vehicle here
    vehicle.Move();
```

...and that's all you need to get your services up and running. The container will resolve the dependencies for you, leaving you to do more useful tasks.
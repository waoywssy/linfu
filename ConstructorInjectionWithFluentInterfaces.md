# Introduction #

## A Simple Example ##
Let's assume that you have a domain class with a constructor that is similar to the following:

```
    public class Car : IVehicle, IInitialize
    {
        private IEngine _engine;
        private IPerson _person;

        public Car(IEngine engine, IPerson driver)
        {
            _engine = engine;
            _person = driver;
        }

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

        #region IInitialize Members

        public void Initialize(IServiceContainer container)
        {
            _engine = container.GetService<IEngine>();
            _person = container.GetService<IPerson>();
        }

        #endregion
    }
```

## Fluent Interfaces ##
...and since the _Car_ class implements the _IVehicle_ interface, let's also assume that you want to inject the _Car_ class as an instance of the _IVehicle_ interface into your container. Here's the code to do it:

```
            // Configure the container inject instances
            // into the Car class constructor
            container.Inject<IVehicle>()
                .Using(ioc => new Car(ioc.GetService<IEngine>(),
                                      ioc.GetService<IPerson>()))
                                      .OncePerRequest();
```

That single statement of code will tell the container to use the _Car_ class instructor and instantiate it with whatever _IEngine_ and _IPerson_ service instances are currently available in the container. Furthermore, the call to _OncePerRequest()_ will ensure that a brand new car instance will be created on each service request.

### Other Lifecycle Types ###

If you need to create either a singleton service or a service that exists only once per running thread, you can easily change the lines of code above to one of the following:

```
            // Create a singleton service instance, OR
            container.Inject<IVehicle>()
                .Using(ioc => new Car(ioc.GetService<IEngine>(),
                                      ioc.GetService<IPerson>()))
                                      .AsSingleton();

            // Use this to create a once per thread instance:
            container.Inject<IVehicle>()
                .Using(ioc => new Car(ioc.GetService<IEngine>(),
                                      ioc.GetService<IPerson>()))
                                      .OncePerThread();
```

### Alternative Syntax ###

LinFu's IoC v2.0 container also supports injection for concrete service types ~~~that have default constructors without any parameters~~~. For example, assuming that the _Car_ class has the following constructors:
```
    public class Car : IVehicle, IInitialize
    {
        public Car() 
        {
        }

        public Car(IEngine engine, IPerson driver)
        {
        }
        // ...
    }
```

...you can inject the _Car_ class into the container using the following syntax:

```
            // Notice the similarity in syntax
            container.Inject<IVehicle>().Using<Car>().OncePerRequest();
```

As you can see, the syntax is very straightforward. LinFu.IoC will automatically choose the constructor with the most resolvable parameters from the container and inject the parameter values for you.

**NOTE:** You can also find out how to download the example project by clicking [here](DownloadingTheIoCExamples.md)
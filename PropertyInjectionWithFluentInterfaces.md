# Introduction #
## A Sample Service ##
Let's assume that we have a sample service named _IVehicle_, and a _Car_ class that implements that service type:

```
    public interface IVehicle
    {
        void Move();
        void Park();
        IPerson Driver { get; set; }
        IEngine Engine { get; set; }
    }

    public class Car : IVehicle
    {
        public IEngine Engine
        {
            get; set;
        }
        public IPerson Driver
        {
            get; set;
        }
        public void Move()
        {
            if (Engine == null || Driver == null)
                return;

            Engine.Start();
            Console.WriteLine("{0} says: I’m moving!", Driver.Name);
        }
        public void Park()
        {
            if (Engine == null || Driver == null)
                return;

            Engine.Stop();
            Console.WriteLine("{0} says: I’m parked!", Driver.Name);
        }
    }
```

...and suppose that I wanted to initialize both the _Engine_ and the _Driver_ properties every time a vehicle named "OldVehicle" was instantiated. Here's how you can do it:

```
            // Set the engine type
            container.Initialize<IVehicle>("OldVehicle")
                .With((ioc, vehicle) => vehicle.Engine = ioc.GetService<IEngine>("OldEngine"));

            // Set the person type
            container.Initialize<IVehicle>("OldVehicle")
                .With((ioc, vehicle) => vehicle.Driver = ioc.GetService<IPerson>("OldPerson"));
```

The _Initialize()_ method allows you to inject different services into service properties. You can even decide which properties and services should be injected on a per-service name basis. It's just that simple.

**NOTE:** You can also find out how to download the example project by clicking [here](DownloadingTheIoCExamples.md)
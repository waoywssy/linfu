using System.IO;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can dynamically configure
    /// <see cref="IServiceContainer"/> instances at runtime.
    /// </summary>
    public class Loader : Loader<IServiceContainer>
    {
        public Loader()
        {
            var directory = Path.GetDirectoryName(typeof (Loader).Assembly.Location);
            
            // HACK: Load all plugins by default
            LoadDirectory(directory, "LinFu*.dll");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can dynamically configure
    /// <see cref="IServiceContainer"/> instances at runtime.
    /// </summary>
    public class Loader
    {
        private IList<IContainerLoader> _loaders = new List<IContainerLoader>();

        /// <summary>
        /// Gets or sets the target container to load.
        /// </summary>
        public IServiceContainer Container
        {
            get; set;
        }

        /// <summary>
        /// The list of <see cref="IContainerLoader"/> instances
        /// that will be used to configure the target <see cref="Container"/>
        /// instance.
        /// </summary>
        public IList<IContainerLoader> ContainerLoaders
        {
            get
            {
                return _loaders;
            }
        }

        /// <summary>
        /// Loads the container with the files listed in 
        /// the target <paramref name="directory"/> that match 
        /// the given <paramref name="filespec">file pattern</paramref>.
        /// </summary>
        /// <param name="directory">The full path of the location to scan.</param>
        /// <param name="filespec">The wildcard file pattern string to use when specifying the target files.</param>
        public void LoadDirectory(string directory, string filespec)
        {
            // Determine which files exist
            var files = Directory.GetFiles(directory, filespec);

            foreach (var currentFile in files)
            {
                foreach (var loader in ContainerLoaders)
                {
                    if (loader == null)
                        continue;

                    if (loader.CanLoad(currentFile))
                        loader.Load(currentFile);
                }
            }
        }
    }
}

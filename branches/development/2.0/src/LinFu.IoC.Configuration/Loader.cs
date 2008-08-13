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
        private readonly IList<IContainerLoader> _loaders = new List<IContainerLoader>();

        /// <summary>
        /// Initializes the container with the default settings.
        /// </summary>
        public Loader()
        {
            DirectoryLister = new DefaultDirectoryLister();
        }

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
        /// Gets or sets the <see cref="IDirectoryListing"/> instance 
        /// responsible for returning a list of filenames
        /// to the loader for processing.
        /// </summary>
        public IDirectoryListing DirectoryLister { get; set; }


        /// <summary>
        /// Loads the container with the files listed in 
        /// the target <paramref name="directory"/> that match 
        /// the given <paramref name="filespec">file pattern</paramref>.
        /// </summary>
        /// <param name="directory">The full path of the location to scan.</param>
        /// <param name="filespec">The wildcard file pattern string to use when specifying the target files.</param>
        public void LoadDirectory(string directory, string filespec)
        {
            // Abort the load if the container
            // is invalid
            if (Container == null)
                return;

            // Determine which files exist
            var files = DirectoryLister.GetFiles(directory, filespec);
            var actionList = new List<Action<IServiceContainer>>();

            foreach (var currentFile in files)
            {
                foreach (var loader in ContainerLoaders)
                {
                    if (loader == null || !loader.CanLoad(currentFile))
                        continue;

                    var actions = loader.Load(currentFile);
                    if (actions.Count() == 0)
                        continue;

                    actionList.AddRange(actions);
                }
            }
            
            // Configure the container
            foreach(var action in actionList)
            {
                if (action == null)
                    continue;

                action(Container);
            }
        }
    }
}

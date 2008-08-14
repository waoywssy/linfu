using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{    
    public class Loader<TTarget>
    {
        private readonly IList<IActionLoader<TTarget, string>> _loaders = new List<IActionLoader<TTarget, string>>();
        private readonly IList<ILoaderPlugin<TTarget>> _plugins = new List<ILoaderPlugin<TTarget>>();
        /// <summary>
        /// Initializes the container with the default settings.
        /// </summary>
        public Loader()
        {
            DirectoryLister = new DefaultDirectoryLister();
        }

        /// <summary>
        /// Gets or sets the target to load.
        /// </summary>
        public TTarget Target
        {
            get;
            set;
        }

        public IList<ILoaderPlugin<TTarget>> Plugins
        {
            get { return _plugins; }
        }
        /// <summary>
        /// The list of <see cref="IActionLoader{TTarget, TInput}"/>
        /// instances responsible for configuring the <see cref="Target"/> instance.
        /// </summary>
        public IList<IActionLoader<TTarget, string>> FileLoaders
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
            if (Target == null)
                return;

            // Determine which files exist
            var files = DirectoryLister.GetFiles(directory, filespec);
            var actionList = new List<Action<TTarget>>();

            foreach (var currentFile in files)
            {
                foreach (var loader in FileLoaders)
                {
                    if (loader == null || !loader.CanLoad(currentFile))
                        continue;

                    var actions = loader.Load(currentFile);
                    if (actions.Count() == 0)
                        continue;

                    actionList.AddRange(actions);
                }
            }

            // Signal the beginning of the load
            foreach(var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.BeginLoad(Target);
            }

            // Configure the container
            foreach (var action in actionList)
            {
                if (action == null)
                    continue;

                action(Target);
            }

            // Signal the end of the load
            foreach (var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.EndLoad(Target);
            }
        }
    }
}

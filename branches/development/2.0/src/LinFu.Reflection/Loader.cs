using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    public class Loader<TTarget> : ILoader<TTarget>
    {
        private readonly List<IActionLoader<TTarget, string>> _loaders = new List<IActionLoader<TTarget, string>>();
        private readonly List<ILoaderPlugin<TTarget>> _plugins = new List<ILoaderPlugin<TTarget>>();
        private readonly List<Action<TTarget>> _actions = new List<Action<TTarget>>();
        /// <summary>
        /// Initializes the container with the default settings.
        /// </summary>
        public Loader()
        {
            DirectoryLister = new DefaultDirectoryLister();
        }
        /// <summary>
        /// The list of actions that will execute
        /// every time a target instance is configured.
        /// </summary>
        public IList<Action<TTarget>> QueuedActions
        {
            get { return _actions; }
        }

        /// <summary>
        /// The list of <see cref="ILoaderPlugin{TTarget}"/>
        /// instances that will be used to
        /// signal the beginning and end of the
        /// load sequence.
        /// </summary>
        public IList<ILoaderPlugin<TTarget>> Plugins
        {
            get { return _plugins; }
        }
        /// <summary>
        /// The list of <see cref="IActionLoader{TTarget, TInput}"/>
        /// instances responsible for configuring the target instance.
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
            // Determine which files exist
            var files = DirectoryLister.GetFiles(directory, filespec);

            foreach (var currentFile in files)
            {
                foreach (var loader in FileLoaders)
                {
                    if (loader == null || !loader.CanLoad(currentFile))
                        continue;

                    var actions = loader.Load(currentFile);
                    if (actions.Count() == 0)
                        continue;

                    _actions.AddRange(actions);
                }
            }
        }


        public void LoadInto(TTarget target)
        {
            // Abort the load if the container
            // is invalid
            if (ReferenceEquals(target, null))
                return;

            // Signal the beginning of the load
            foreach (var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.BeginLoad(target);
            }

            // Configure the container
            foreach (var action in QueuedActions)
            {
                if (action == null)
                    continue;

                action(target);
            }

            // Signal the end of the load
            foreach (var plugin in Plugins)
            {
                if (plugin == null)
                    continue;

                plugin.EndLoad(target);
            }
        }
    }
}

using System.Collections.Generic;

namespace LinFu.Reflection
{
    public interface ILoader<TTarget>
    {
        /// <summary>
        /// The list of <see cref="ILoaderPlugin{TTarget}"/>
        /// instances that will be used to
        /// signal the beginning and end of the
        /// load sequence.
        /// </summary>
        IList<ILoaderPlugin<TTarget>> Plugins { get; }

        /// <summary>
        /// The list of <see cref="IActionLoader{TTarget, TInput}"/>
        /// instances responsible for configuring the <see cref="Target"/> instance.
        /// </summary>
        IList<IActionLoader<TTarget, string>> FileLoaders { get; }

        /// <summary>
        /// Gets or sets the <see cref="IDirectoryListing"/> instance 
        /// responsible for returning a list of filenames
        /// to the loader for processing.
        /// </summary>
        IDirectoryListing DirectoryLister { get; set; }

        /// <summary>
        /// Loads the configuration using the files listed in 
        /// the target <paramref name="directory"/> that match 
        /// the given <paramref name="filespec">file pattern</paramref>.
        /// </summary>
        /// <param name="directory">The full path of the location to scan.</param>
        /// <param name="filespec">The wildcard file pattern string to use when specifying the target files.</param>
        void LoadDirectory(string directory, string filespec);

        /// <summary>
        /// Loads
        /// </summary>
        /// <param name="target"></param>
        void LoadInto(TTarget target);
    }
}
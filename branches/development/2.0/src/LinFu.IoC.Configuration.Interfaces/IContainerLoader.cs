using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a loader that reads a file and converts it
    /// into an equivalent set of a set of <see cref="Action{IServiceContainer}"/>
    /// instances that can be applied to a particular
    /// instance of an <see cref="IServiceContainer"/> class.
    /// </summary>
    public interface IContainerLoader
    {
        /// <summary>
        /// Determines whether or not the current <see cref="IContainerLoader"/>
        /// instance can load the current file type.
        /// </summary>
        /// <param name="filename">The filename and full path of the target file.</param>
        /// <returns>Returns <c>true</c> if the file can be loaded; otherwise, the result is <c>false</c>.</returns>
        bool CanLoad(string filename);

        /// <summary>
        /// Reads an input file using the given <paramref name="filename"/>
        /// and converts it into a set of <see cref="Action{IServiceContainer}"/>
        /// instances that can be applied to a target
        /// <see cref="IServiceContainer"/> class.
        /// </summary>
        /// <param name="filename">The target file to be loaded.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances to apply to a target type.</returns>
        IEnumerable<Action<IServiceContainer>> Load(string filename);
    }
}

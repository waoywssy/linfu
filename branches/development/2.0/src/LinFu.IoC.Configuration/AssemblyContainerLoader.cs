using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that loads configuration information
    /// from a given assembly.
    /// </summary>
    public class AssemblyContainerLoader : AssemblyTargetLoader<IServiceContainer>
    {        
    }
}

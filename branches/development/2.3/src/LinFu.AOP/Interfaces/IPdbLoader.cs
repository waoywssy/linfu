using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;
using System.IO;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can load PDB files from disk.
    /// </summary>
    public interface IPdbLoader
    {
        Assembly LoadModifiedAssembly(string pdbFile, MemoryStream assemblyStream);
        void LoadSymbols(string assemblyFileName, AssemblyDefinition assembly);        
        void SaveSymbols(AssemblyDefinition targetAssembly, Stream stream, bool hasSymbols);
    }
}

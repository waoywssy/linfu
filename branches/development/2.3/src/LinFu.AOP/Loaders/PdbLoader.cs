using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using System.Reflection;
using Mono.Cecil;
using System.IO;

namespace LinFu.AOP.Cecil.Loaders
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPdbLoader"/> interface.
    /// </summary>
    public class PdbLoader : IPdbLoader
    {
        public Assembly LoadModifiedAssembly(string pdbFile, MemoryStream assemblyStream)
        {
            // Load the modified assembly into memory
            var assemblyArray = assemblyStream.ToArray();
            bool hasSymbols = File.Exists(pdbFile);

            // Load up the assembly into the current application domain
            if (hasSymbols)
            {
                var pdbBytes = File.ReadAllBytes(pdbFile);
                return Assembly.Load(assemblyArray, pdbBytes);
            }

            return Assembly.Load(assemblyArray);
        }

        public void LoadSymbols(string assemblyFileName, AssemblyDefinition assembly)
        {
            string pdbFile = string.Format("{0}.pdb", assemblyFileName);
            bool hasSymbols = File.Exists(pdbFile);

            if (!hasSymbols)
                return;

            // Create a copy of the pdb file.
            // Mono.Cecil can only update the actual file and create a backup that can be restored later.
            // Again we want our original files to remain untouched.
            var pdbTempFileName = Path.GetTempFileName();
            File.Copy(pdbFile, pdbTempFileName, true);

            var mainModule = assembly.MainModule;
            mainModule.LoadSymbols();
        }

        public void SaveSymbols(AssemblyDefinition targetAssembly, Stream stream, bool hasSymbols)
        {
            // Update the debug symbols
            if (!hasSymbols)
                return;

            targetAssembly.MainModule.SaveSymbols();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Loaders
{
    /// <summary>
    /// Represents a loader that modifies a given assembly prior to being loaded from disk.
    /// </summary>
    public class JITWeaver : AssemblyLoader
    {
        private readonly List<Action<AssemblyDefinition>> _assemblyWeavers = new List<Action<AssemblyDefinition>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JITWeaver"/> class.
        /// </summary>
        public JITWeaver()
            : this(new PdbLoader())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JITWeaver"/> class.
        /// </summary>
        /// <param name="pdbLoader">The loader that will be responsible for loading the program debugging information into memory.</param>
        public JITWeaver(IPdbLoader pdbLoader)
        {
            PdbLoader = pdbLoader;
        }

        /// <summary>
        /// Modifies a given assembly prior to being loaded from disk.
        /// </summary>
        /// <param name="assemblyFile">The filename of the target assembly.</param>
        /// <returns>A valid assembly.</returns>
        public override Assembly Load(string assemblyFile)
        {
            var targetAssembly = AssemblyFactory.GetAssembly(assemblyFile);

            // Strongly-named assemblies cannot be modified
            if (targetAssembly.Name.HasPublicKey)
                return base.Load(assemblyFile);

            var assemblyFileName = Path.GetFileNameWithoutExtension(assemblyFile);

            if (PdbLoader != null)
                PdbLoader.LoadSymbols(assemblyFileName, targetAssembly);

            foreach (var action in AssemblyWeavers)
            {
                action(targetAssembly);

                // Verify the assembly at every step
                if (AssemblyVerifier == null)
                    continue;

                AssemblyVerifier.Verify(targetAssembly);
            }

            string pdbFile = string.Format("{0}.pdb", assemblyFileName);
            bool hasSymbols = File.Exists(pdbFile);

            var memoryStream = new MemoryStream();

            if (PdbLoader != null)
            {
                PdbLoader.SaveSymbols(targetAssembly, memoryStream, hasSymbols);
            }

            // Save the modifed assembly
            AssemblyFactory.SaveAssembly(targetAssembly, memoryStream);

            if (PdbLoader != null)
                return PdbLoader.LoadModifiedAssembly(pdbFile, memoryStream);

            return targetAssembly.ToAssembly();
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IPdbLoader"/> that will be used to load debug symbols into memory.
        /// </summary>
        public IPdbLoader PdbLoader
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value indicating the list of <see cref="Action{T}"/> delegates
        /// that will be used to modify the assemblies loaded into memory.
        /// </summary>
        public virtual IList<Action<AssemblyDefinition>> AssemblyWeavers
        {
            get { return _assemblyWeavers; }
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IVerifier"/>
        /// instance that will be used to ensure that the modified assemblies are valid.
        /// </summary>
        public virtual IVerifier AssemblyVerifier
        {
            get;
            set;
        }
    }
}
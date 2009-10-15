﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;
using LinFu.AOP.Weavers.Cecil;
using System.IO;
using Simple.IoC;
using Simple.IoC.Loaders;
namespace PostWeaver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ShowHelp();
                return;
            }
            string inputFile = args[0];

            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            var targetFile = inputFile;

            Console.WriteLine("PostWeaving Assembly '{0}' -> '{1}'", targetFile, targetFile);

            // Search for any custom method filters that might
            // be located in the same directory as the postweaver
            var programLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            SimpleContainer container = new SimpleContainer();
            
            var loader = new Loader(container);
            loader.LoadDirectory(programLocation, "*.dll");

            // Use the type filter if it exists
            LinFu.AOP.CecilExtensions.ITypeFilter typeFilter = null;
            if (container.Contains(typeof(LinFu.AOP.CecilExtensions.ITypeFilter)))
                typeFilter = container.GetService<LinFu.AOP.CecilExtensions.ITypeFilter>();

            IMethodFilter methodFilter = null;
            methodFilter = container.GetService<IMethodFilter>(false);

            var assembly = AssemblyFactory.GetAssembly(targetFile);
            assembly.InjectAspectFramework(typeFilter, methodFilter, true);
            assembly.Save(targetFile); 
        }


        private static void ShowHelp()
        {
            Console.WriteLine("PostWeaver syntax: PostWeaver [filename]");
        }
    }
}

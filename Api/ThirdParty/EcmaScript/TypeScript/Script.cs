

using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents a script or module within typescript.
    /// </summary>
    public partial class Script : IGeneratable
    {
        
        /// <summary>
        /// Where will this be saved to.
        /// </summary>
        public string FileName; 

        /// <summary>
        /// A list of imports for the current script/module
        /// </summary>
        public List<Import> Imports = [];

        /// <summary>
        /// A list of all nodes inside the script.
        /// </summary>
        public List<IGeneratable> Children = [];

        /// <summary>
        /// Inject custom lines of code into the script
        /// </summary>
        public List<string> Injected = [];

        /// <summary>
        /// Adds an import to the script/module
        /// </summary>
        /// <param name="import"></param>
        public void AddImport(Import import)
        {

            var childTypeClash = Children.OfType<TypeDefinition>().Where(child => import.Symbols.Contains(child.Name));

            if (childTypeClash.Any())
            {
                return;
            }

            var existing = Imports.Where(i => i.From == import.From);

            if (existing.Any())
            {
                var existingImport = existing.First();

                if (string.IsNullOrEmpty(existingImport.DefaultImport))
                {
                    existingImport.DefaultImport = import.DefaultImport;
                }

                foreach(var newSymbol in import.Symbols)
                {
                    if (!existingImport.Symbols.Contains(newSymbol))
                    {
                        existingImport.Symbols.Add(newSymbol);
                    }
                }
                return;
            }
            
            

            Imports.Add(import);
        }

        /// <summary>
        /// Adds a type definition, checking if it already exists.
        /// </summary>
        /// <param name="typeDefinition"></param>
        public void AddTypeDefinition(TypeDefinition typeDefinition)
        {
            
            if (typeDefinition.Name.Contains('`'))
            {
                return;
            }
            if (
                typeDefinition.Name == "T" || 
                typeDefinition.Name == "ID"
            )
            {
                // these are generic type names.
                return;
            }
            
            var existing = Children.Where(i => i is TypeDefinition definition && definition.Name == typeDefinition.Name);

            if (existing.Any()) 
            {
                // prevent dupes.
                return;
            }

            Children.Add(typeDefinition);
        }

        /// <summary>
        /// Add a single source line of code.
        /// </summary>
        /// <param name="sloc"></param>
        public void AddSLOC(string sloc)
        {
            Injected.Add(sloc);
        }

        /// <summary>
        /// Adds a child source generator inside the script.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IGeneratable child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Generate the TypeScript
        /// </summary>
        /// <returns></returns>
        public string CreateSource()
        {
            var source = "/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */" + Environment.NewLine;

            source += "// Imports" + Environment.NewLine;
            foreach(var import in Imports)
            {
                source += import.CreateSource();
            }
            source += Environment.NewLine;
            source += "// Module" + Environment.NewLine;

            foreach(var child in Children)
            {
                source += child.CreateSource() + Environment.NewLine;
            }

            source += string.Join(Environment.NewLine, Injected) + Environment.NewLine;

            return source;
        }

        /// <summary>
        /// No documentation for this entity type.
        /// </summary>
        public string GetTsDocumentation()
        {
            return null;
        }

        /// <summary>
        /// Adds a line to documentation
        /// </summary>
        /// <param name="line"></param>
        public void AddTsDocLine(string line)
        {
            return;
        }
    }
}
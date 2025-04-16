using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents a TypeScript class definition that can be generated dynamically.
    /// </summary>
    public partial class ClassDefinition : IGeneratable
    {
        /// <summary>
        /// The name of the TypeScript class.
        /// </summary>
        public string Name;

        /// <summary>
        /// The generic type template if the class has generics.
        /// </summary>
        public string GenericTemplate;
        
        /// <summary>
        /// The name of the class that this class extends, if any.
        /// </summary>
        public string Extends;

        /// <summary>
        /// A list of interfaces that this class implements.
        /// </summary>
        public List<string> Implements = [];

        /// <summary>
        /// All properties &amp; methods
        /// </summary>
        public List<IGeneratable> Children = [];

        /// <summary>
        /// Documentation for the TS Output
        /// </summary>
        public List<string> Documentation = [];

        public void AddMethod(ClassMethod method)
        {
            var existing = Children.OfType<ClassMethod>()
                                   .Where(child => child.Name == method.Name);
            if (existing.Any())
            {
                var existingMethod = existing.First();

                if (method.Arguments.Count > existingMethod.Arguments.Count)
                {
                    Children.Remove(existingMethod);
                    Children.Add(method);
                }
                return;
            }
            Children.Add(method);
        }

        /// <summary>
        /// Outputs the documentation for the current method.
        /// </summary>
        /// <returns></returns>
        public string GetTsDocumentation()
        {
            var src = "/**" + Environment.NewLine;
            foreach(var doc in Documentation)
            {
                src += "".PadLeft(4) + doc + Environment.NewLine;
            }
            return src + "*/" + Environment.NewLine;
        }

        /// <summary>
        /// Adds a line to documentation
        /// </summary>
        /// <param name="line"></param>
        public void AddTsDocLine(string line)
        {
            Documentation.Add(line);
        }

        /// <summary>
        /// Generates the TypeScript class definition as a source code string.
        /// </summary>
        /// <returns>The TypeScript class definition as a string.</returns>
        public string CreateSource()
        {
            var src = GetTsDocumentation() + "export class " + Name;

            if (!string.IsNullOrEmpty(GenericTemplate))
            {
                src += $"<{GenericTemplate}>";
            }
            if (!string.IsNullOrEmpty(Extends))
            {
                src += " extends " + Extends;
            }

            if (Implements.Any())
            {
                src += " implements " + string.Join(", ", Implements);
            }

            src += "{" + Environment.NewLine;
            
            foreach(var child in Children)
            {
                src += child.CreateSource() + Environment.NewLine;
            }

            src += "}" + Environment.NewLine;

            return src;
        }
    }
}

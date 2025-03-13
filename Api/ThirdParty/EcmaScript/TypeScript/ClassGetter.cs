using System;
using System.Collections.Generic;

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents a TypeScript class getter method.
    /// </summary>
    public partial class ClassGetter : IGeneratable
    {
        /// <summary>
        /// Gets or sets the name of the getter method.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the source code lines that define the getter method's body.
        /// </summary>
        public List<string> Source = [];

        /// <summary>
        /// What does the getter return
        /// </summary>
        public string ReturnType;

        /// <summary>
        /// Documentation for the TS Output
        /// </summary>
        public List<string> Documentation = [];
        
        /// <summary>
        /// Outputs the documentation for the current method.
        /// </summary>
        /// <returns></returns>
        public string GetTsDocumentation()
        {
            var src = "".PadLeft(4) + "/*";
            foreach(var doc in Documentation)
            {
                src += "".PadLeft(4) + doc + Environment.NewLine;
            }
            return src + Environment.NewLine + "".PadLeft(4) + "*/" + Environment.NewLine;
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
        /// Generates the TypeScript source representation of the getter method.
        /// </summary>
        /// <returns>A string containing the TypeScript getter method definition.</returns>
        public string CreateSource()
        {
            var src = GetTsDocumentation() + "".PadLeft(4) + "get " + Name + "()";
            
            if (!string.IsNullOrEmpty(ReturnType))
            {
                src += ": " + ReturnType;
            }

            src += " {" + Environment.NewLine;
            foreach (var line in Source)
            {
                src += "".PadLeft(8) + line + Environment.NewLine;
            }
            src += "".PadLeft(4) + "}" + Environment.NewLine;

            return src;
        }
    }
}

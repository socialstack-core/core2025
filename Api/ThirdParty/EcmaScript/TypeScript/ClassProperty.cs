using System;
using System.Collections.Generic;
using Api.ContentSync;

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents a property within a TypeScript class.
    /// </summary>
    public partial class ClassProperty : IGeneratable
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the TypeScript type of the property.
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the default value of the property.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the property (e.g., public, private, protected).
        /// Defaults to "public".
        /// </summary>
        public string Visibility { get; set; } = "public";

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
        /// Generates the TypeScript property definition as a source code string.
        /// </summary>
        /// <returns>The TypeScript property definition as a formatted string.</returns>
        public string CreateSource()
        {
            var src = "".PadLeft(4) + $"{Visibility} {PropertyName}: {PropertyType}";

            if (PropertyType == "string")
            {
                if (DefaultValue == null)
                {
                    DefaultValue = "";
                }
                src += $" = '{DefaultValue.Replace("'", "\\'")}'"; 
            } 
            else 
            {
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    src += " = " + DefaultValue +  " as " + PropertyType;
                }
            }
            src += ";" + Environment.NewLine;

            return src;
        }
    }
}

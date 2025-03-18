using System;
using System.Collections.Generic;

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents a method within a TypeScript class.
    /// </summary>
    public partial class ClassMethod : IGeneratable
    {
        /// <summary>
        /// Gets or sets the method's visibility modifier (e.g., public, private).
        /// </summary>
        public string Modifier { get; set; } = "public";

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the generic template definition for the method, if applicable.
        /// </summary>
        public string GenericTemplate { get; set; }

        /// <summary>
        /// Gets or sets the return type of the method.
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// Gets or sets the list of arguments for the method.
        /// </summary>
        public List<ClassMethodArgument> Arguments { get; set; } = [];

        /// <summary>
        /// Code injected into the function
        /// </summary>
        public List<string> Injected = [];

        /// <summary>
        /// Documentation for the TS Output
        /// </summary>
        public List<string> Documentation = [];

        /// <summary>
        /// Generates the TypeScript method definition as a source code string.
        /// </summary>
        /// <returns>The TypeScript method definition as a formatted string.</returns>
        public string CreateSource()
        {
            var src = GetTsDocumentation();

            src += "".PadLeft(4) + $"{Modifier} {Name}";

            var isConstructor = Name == "constructor";

			if (isConstructor)
            {
				src += " (";
			}
            else
            {
                src += " = (";
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (i > 0) 
                {
                    src += ", ";
                }
                src += Arguments[i].CreateSource();
            }

            src += ")";

            if (!string.IsNullOrEmpty(ReturnType))
            {
                src += $": {ReturnType} ";
            }

			if (!isConstructor)
			{
                // Using arrow syntax for this binding such that <Form> can just be given the func
				src += " => ";
			}

			src += "{" + Environment.NewLine;

			foreach (var sloc in Injected)
            {
                src += "".PadLeft(8) + sloc + Environment.NewLine;
            }
            src += "".PadLeft(4) + "}" + Environment.NewLine;

            return src;
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
        /// Outputs the documentation for the current method.
        /// </summary>
        /// <returns></returns>
        public string GetTsDocumentation()
        {
            var src = "".PadLeft(4) + "/**" + Environment.NewLine;
            foreach(var doc in Documentation)
            {
                src += "".PadLeft(6) + doc + Environment.NewLine;
            }
            return src + Environment.NewLine + "".PadLeft(4) + "*/" + Environment.NewLine;
        }
    }
}

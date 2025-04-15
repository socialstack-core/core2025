

using System.Collections.Generic;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new &amp; vastly improved source generation engine.
    /// made to be a lot more maintainable, and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {
        /// <summary>
        /// Holds all the scripts to keep imports nice and tidy etc...
        /// </summary>
        public static readonly List<Script> scripts = [];

        /// <summary>
        /// Ensures a script is held in the source generator 
        /// list, and that its available to other scripts.
        /// </summary>
        /// <param name="script"></param>
        public static void EnsureScript(Script script)
        {
            if (!scripts.Contains(script))
            {
                scripts.Add(script);
            }
        }
    }
}
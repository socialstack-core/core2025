

using System;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static void CreateIncludeClass(Type entityType, Script script)
        {

            var def = new ClassDefinition() {
                Name = entityType.Name + "Includes",
                Extends = "ApiIncludes"
            };

            // TODO: Build out again

            script.AddChild(def);
        }
    }
}


using System;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static void CreateIncludeClass(Type entityType, Script script)
        {
            Console.WriteLine($"Adding includes class to {script.FileName} from entity {entityType.Name}");

            var def = new ClassDefinition() {
                Name = entityType.Name + "Includes",
                Extends = "ApiIncludes"
            };


            script.AddChild(def);
        }
    }
}
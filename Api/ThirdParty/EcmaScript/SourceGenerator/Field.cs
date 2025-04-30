using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new and vastly improved source generation engine.
    /// Made to be a lot more maintainable and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="script"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        public static string OnField(Type fieldType, Script script, int currentDepth = 0)
        {
            var ecmaService = Services.Get<EcmaService>();

            // Unwrap nullable
            fieldType = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

            // Handle generic types (e.g., List<T>, Dictionary<K,V>)
            if (fieldType.IsGenericType)
            {
                return GetGenericFieldTypeString(fieldType, script);
            }

            // Check if mapped directly by type conversion
            if (ecmaService.TypeConversions.TryGetValue(fieldType, out var mappedType))
            {
                return mappedType;
            }

            var fieldTypeName = fieldType.Name;

            // If the type doesn't exist yet, try to create or import it
            if (!TypeDefinitionExists(fieldTypeName))
            {
                return TryImportCustomType(fieldType, script, currentDepth);
            }

            // If it exists but isn't in this script, import it
            var containerScript = GetScriptByContainingTypeDefinition(fieldTypeName);
            if (containerScript?.FileName != script.FileName)
            {
                script.AddImport(new()
                {
                    Symbols = [fieldTypeName],
                    From = "./" + fieldTypeName
                });
            }

            return fieldTypeName;
        }

        private static string GetGenericFieldTypeString(Type fieldType, Script script)
        {
            var typeName = fieldType.Name.Split('`')[0];
            var genericArgs = fieldType.GetGenericArguments();
            var resolvedArgs = string.Join(", ", genericArgs.Select(arg => OnField(arg, script)));

            if (IsList(fieldType))
            {
                return $"{resolvedArgs}[]";
            }

            if (typeName == "Dictionary" || typeName == "IReadOnlyDictionary")
            {
                typeName = "Record";
            }

            return $"{typeName}<{resolvedArgs}>";
        }

        private static string TryImportCustomType(Type fieldType, Script script, int currentDepth)
        {
            var fieldTypeName = fieldType.Name;

            if (IsEntity(fieldType))
            {
                script.AddImport(new()
                {
                    Symbols = [fieldTypeName],
                    From = "./" + fieldTypeName
                });
            }
            else if (currentDepth < 2)
            {
                // Safeguard depth to prevent recursion loops
                OnNonEntity(fieldType, script, currentDepth + 1);
            }

            return fieldTypeName;
        }
    }
}

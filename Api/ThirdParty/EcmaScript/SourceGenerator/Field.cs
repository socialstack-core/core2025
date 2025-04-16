
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using ImageMagick.Drawing;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new &amp; vastly improved source generation engine.
    /// made to be a lot more maintainable, and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {

        public static string OnField(Type fieldType, Script script, int currentDepth = 0)
        {
            var ecmaService = Services.Get<EcmaService>();

            // if there's an underlying type, grab it.
            var ulType = Nullable.GetUnderlyingType(fieldType); 
            if (ulType != null)
            {
                fieldType = ulType;
            }
        
            // if its a dictionary, output it as a Record with the same params.
            if (fieldType.IsGenericType)
            {
                var typeName = fieldType.Name.Split('`')[0]; // remove `1, `2, etc.
                var genericArgs = fieldType.GetGenericArguments();
                    var resolvedArgs = string.Join(", ", genericArgs.Select(arg => OnField(arg, script)));
                    
                
                if (IsList(fieldType))
                {
                    return $"{resolvedArgs}[]";
                } else {
                    if (typeName == "Dictionary")
                    {
                        typeName = "Record";
                    }
                    return $"{typeName}<{resolvedArgs}>";
                }
            }

            
            // first check if its been mapped already.
            if (ecmaService.TypeConversions.TryGetValue(fieldType, out string replaceType))
            {
                return replaceType;
            }
            // now we need to check whether the type actually exists.

            if (!TypeDefinitionExists(fieldType.Name))
            {
                // then we need to create it.

                if (IsEntity(fieldType))
                {
                    // it will be added elsewhere.
                    script.AddImport(new() {
                        Symbols = [fieldType.Name],
                        From = "./" + fieldType.Name
                    });
                }
                else {
                    if (currentDepth < 2)
                    {
                        // this part is potentially hazardous. 
                        OnNonEntity(fieldType, script, currentDepth + 1);
                    }
                }

                return fieldType.Name;

            }
            else
            {
                var existingTypeContainer = GetScriptByContainingTypeDefinition(fieldType.Name);

                if (existingTypeContainer.FileName != script.FileName)
                {
                    // import it
                    script.AddImport(new() {
                        Symbols = [fieldType.Name],
                        From = "./" + fieldType.Name
                    });
                }
            }

            return fieldType.Name;
        }
    }
}
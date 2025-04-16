using System;
using System.Linq;
using Api.EcmaScript.TypeScript;
using System.Reflection;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {

        public static ClassDefinition OnGenericControllerClass(Type t, Script script)
        {

            var definition = new ClassDefinition
            {
                Name = GetCleanTypeName(t),
                GenericTemplate = string.Join(", ",
                    t.GetGenericArguments().Select(arg => arg.Name)
                ) + ", ApiIncludes"
            };

            var includesProperty = new ClassProperty() {
                PropertyName = "includes?", 
                PropertyType = "ApiIncludes",
                DefaultValue = "undefined"
            };

            definition.Children.Add(includesProperty);

            script.AddImport(new Import() {
                Symbols = ["getJson"],
                From = "UI/Functions/WebRequest"
            });
            
            var apiUrlProperty = new ClassProperty() {
                PropertyName = "apiUrl",
                PropertyType = "string"
            };

            var ctor = new ClassMethod() {
                Name = "constructor",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "apiUrl",
                        Type = "string"
                    }
                ] , 
                Injected = [
                    "this.apiUrl = apiUrl"
                ]
            };

            definition.Children.Add(apiUrlProperty);

            definition.AddMethod(ctor);

            AddControllerMethods(t, definition, null, script);

            return definition;

        }

    }
}
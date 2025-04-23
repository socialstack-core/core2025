using System;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static ClassDefinition OnGenericControllerClass(Type t, Script script)
        {
            var typeName = GetCleanTypeName(t);
            var genericArgs = string.Join(", ", t.GetGenericArguments().Select(arg => arg.Name));

            var classDef = new ClassDefinition
            {
                Name = typeName,
                GenericTemplate = $"{genericArgs}, ApiIncludes"
            };

            AddImportsForGenericController(script);
            AddApiUrlProperty(classDef);
            AddIncludesProperty(classDef);
            AddConstructor(classDef);
            AddControllerMethods(t, classDef, null, script);

            return classDef;
        }

        private static void AddImportsForGenericController(Script script)
        {
            script.AddImport(new Import
            {
                Symbols = ["getOne", "getJson", "getText"],
                From = "UI/Functions/WebRequest"
            });
        }

        private static void AddIncludesProperty(ClassDefinition classDef)
        {
            classDef.Children.Add(new ClassProperty
            {
                PropertyName = "includes?",
                PropertyType = "ApiIncludes",
                DefaultValue = "undefined"
            });
        }

        private static void AddApiUrlProperty(ClassDefinition classDef)
        {
            classDef.Children.Add(new ClassProperty
            {
                PropertyName = "apiUrl",
                PropertyType = "string"
            });
        }

        private static void AddConstructor(ClassDefinition classDef)
        {
            var ctor = new ClassMethod
            {
                Name = "constructor",
                Arguments =
                [
                    new ClassMethodArgument
                    {
                        Name = "apiUrl",
                        Type = "string"
                    }
                ],
                Injected =
                [
                    "this.apiUrl = apiUrl"
                ]
            };

            classDef.AddMethod(ctor);
        }
    }
}

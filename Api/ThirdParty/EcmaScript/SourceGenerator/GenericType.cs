using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static TypeDefinition OnGenericTypeDefinition(Type t, Script script)
        {
            var ecmaService = Services.Get<EcmaService>();

            var genericStr = BuildGenericTypeString(t, ecmaService);
            var typeDefinition = new TypeDefinition
            {
                Name = GetCleanTypeName(t),
                FromType = t,
                GenericTemplate = genericStr,
            };

            HandleBaseType(t, script, ecmaService, typeDefinition);
            AddFieldsAndProperties(t, script, typeDefinition);

            return typeDefinition;
        }

        private static string BuildGenericTypeString(Type t, EcmaService ecmaService)
        {
            var sb = new StringBuilder();
            var genericArgs = t.GetGenericArguments();

            for (int i = 0; i < genericArgs.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(ecmaService.GetTypeConversion(genericArgs[i]));
            }

            return sb.ToString();
        }

        private static void HandleBaseType(Type t, Script script, EcmaService ecmaService, TypeDefinition typeDefinition)
        {
            if (t.BaseType == null) return;

            if (t.BaseType.IsGenericType)
            {
                var baseArgs = new StringBuilder();
                var genericArgs = t.GetGenericArguments();

                for (int i = 0; i < genericArgs.Length; i++)
                {
                    if (i > 0) baseArgs.Append(", ");

                    var resolvedType = GetResolvedType(genericArgs[i]);

                    if (ecmaService.TypeConversions.TryGetValue(resolvedType, out var convertedType))
                    {
                        baseArgs.Append(convertedType);
                    }
                    else if (script.Children.OfType<TypeDefinition>().Any(child => child.Name == resolvedType.Name))
                    {
                        baseArgs.Append(resolvedType.Name);
                    }
                    else
                    {
                        var typeScript = GetScriptByContainingTypeDefinition(resolvedType.Name);
                        if (typeScript != null)
                        {
                            baseArgs.Append(resolvedType.Name);
                            script.AddImport(new()
                            {
                                Symbols = [resolvedType.Name],
                                From = "./" + Path.GetFileNameWithoutExtension(typeScript.FileName)
                            });
                        }
                    }
                }

                typeDefinition.Inheritence = [GetCleanTypeName(t.BaseType) + "<" + baseArgs + ">"];
            }
            else
            {
                typeDefinition.Inheritence = [GetCleanTypeName(t.BaseType)];
            }
        }

        private static void AddFieldsAndProperties(Type t, Script script, TypeDefinition typeDefinition)
        {
            var members = t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                           .Where(m => m.DeclaringType == t && (m is FieldInfo || m is PropertyInfo));

            foreach (var member in members)
            {
                Type memberType;
                string name;

                if (member is FieldInfo field)
                {
                    memberType = field.FieldType;
                    name = field.Name;
                }
                else if (member is PropertyInfo property)
                {
                    memberType = property.PropertyType;
                    name = property.Name;
                }
                else
                {
                    continue;
                }

                string key = LcFirst(name) + (IsTypeNullable(memberType) ? "?" : "");
                string typeStr = memberType == t ? t.Name : OnField(memberType, script, 0);

                typeDefinition.AddProperty(key, typeStr);
            }
        }
    }
}

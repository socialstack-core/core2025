

using System;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {

        public static TypeDefinition OnGenericTypeDefinition(Type t, Script script)
        {
            var ecmaService = Services.Get<EcmaService>();

            string genericStr = "";

            bool isFirst = true;

            foreach(var arg in t.GetGenericArguments())
            {
                if (!isFirst) {
                    genericStr += ", ";
                }

                genericStr += ecmaService.GetTypeConversion(arg);

                if (isFirst) {
                    isFirst = false;
                }
            }

            var typeDefinition = new TypeDefinition() {
                Name = GetCleanTypeName(t),
                FromType = t,
                GenericTemplate = genericStr
            };

            foreach(var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.DeclaringType != t) 
                {
                    continue;
                }

                if (field.FieldType == t)
                {
                    // its a self ref
                    typeDefinition.AddProperty(LcFirst(field.Name) + '?', t.Name);                    
                }
                else
                {
                    var isNullable = IsTypeNullable(field.FieldType);
                    typeDefinition.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), OnField(field.FieldType, script, 0));
                }
            }

            foreach(var field in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.DeclaringType != t) 
                {
                    continue;
                }
                if (field.PropertyType == t)
                {
                    // its a self ref
                    typeDefinition.AddProperty(LcFirst(field.Name) + '?', t.Name);  
                }
                else
                {
                    var isNullable = IsTypeNullable(field.PropertyType);
                    typeDefinition.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), OnField(field.PropertyType, script, 0));
                }
            }

            return typeDefinition;
        }

    }
}
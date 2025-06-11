using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Api.Database;
using Api.Startup;

namespace Api.TypeScript.Objects
{
    /// <summary>
    /// Represents a list of generic types to be emitted as TypeScript type aliases.
    /// </summary>
    public class GenericTypeList : AbstractTypeScriptObject
    {
        private readonly List<Type> _contentTypes = [];

        private readonly List<Type> _requiredImports = [];

        /// <summary>
        /// Adds a .NET type to the list of types to be emitted as TypeScript types.
        /// </summary>
        /// <param name="type">The .NET type to include in the output.</param>
        public void AddContentType(Type type)
        {
            _contentTypes.Add(type);

            if (type == typeof(Content<>))
            {
                // load all required imports.
                
                foreach (var globalField in ContentFields.GlobalVirtualFields.Values)
                {
                    var virtualInfo = globalField.VirtualInfo;

                    Type virtualType = null;

                    if(virtualInfo.DynamicTypeField != null){
                        virtualType = typeof(object);
                    } else if(virtualInfo.ValueGeneratorType != null) {
                        #warning return type assumption: this requires context of the specific type that it is occuring on
                        virtualType = typeof(string);
                    } else {
                        virtualType = virtualInfo.Type;
                    }

                    if(virtualInfo.IsList){
                        virtualType = virtualType.MakeArrayType();
                    }

                    if (virtualType.IsArray)
                    {
                        _requiredImports.Add(virtualType.GetElementType());
                        continue;
                    }
                    
                    _requiredImports.Add(virtualType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Type> GetRequiredImports()
        {
            return _requiredImports;
        }

        /// <summary>
        /// Emits TypeScript code for each generic type in the list.
        /// </summary>
        /// <param name="builder">The output string builder.</param>
        /// <param name="svc">The TypeScript service context.</param>
        public override void ToSource(StringBuilder builder, TypeScriptService svc)
        {
            foreach (var type in _contentTypes)
            {
                builder.AppendLine();
                builder.Append($"export type {svc.GetGenericSignature(type)} = ");

                // Extend base type if applicable
                if (type.BaseType is not null && type.BaseType != typeof(object))
                {
                    builder.Append($"{svc.GetGenericSignature(type.BaseType)} & ");
                }

                builder.AppendLine("{");

                // Emit fields and properties
                foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    switch (member)
                    {
                        case FieldInfo field:
                            EmitField(builder, svc, field.Name, field.FieldType);
                            break;

                        case PropertyInfo prop:
                            EmitField(builder, svc, prop.Name, prop.PropertyType);
                            break;
                    }
                }

                // Emit global virtual fields if the type is Content<>
                if (type == typeof(Content<>))
                {
                    builder.AppendLine($"    // adding ({ContentFields.GlobalVirtualFields.Count}) global virtual fields.");
                    try
                    {
                        foreach (var globalField in ContentFields.GlobalVirtualFields.Values)
                        {
                            var virtualInfo = globalField.VirtualInfo;

                            Type virtualType = null;

                            if(virtualInfo.DynamicTypeField != null){
                                virtualType = typeof(object);
                            } else if(virtualInfo.ValueGeneratorType != null) {
                                // this one kinda requires context of the specific type that it is occuring on, in short
                                virtualType = typeof(string); // ..is an ugly assumption that happens to be about right atm
                            } else {
                                virtualType = virtualInfo.Type;
                            }

                            if(virtualInfo.IsList){
                                virtualType = virtualType.MakeArrayType();
                            }
                            var fieldName = TypeScriptService.LcFirst(globalField.VirtualInfo?.FieldName);
                            

                            EmitField(builder, svc, fieldName, virtualType);
                            _requiredImports.Add(virtualType);
                        }
                    }
                    catch (Exception ex)
                    {
                        builder.AppendLine($"    // Error writing global virtual fields: {ex.Message}");
                    }
                }

                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        /// <summary>
        /// Emits a single field or property as a TypeScript type line.
        /// </summary>
        private static void EmitField(StringBuilder builder, TypeScriptService svc, string name, Type type)
        {
           var tsField = new TypeScriptField
           {
               FieldName = name,
               FieldType = type
           };
           tsField.ToSource(builder, svc);
        }
    }
}

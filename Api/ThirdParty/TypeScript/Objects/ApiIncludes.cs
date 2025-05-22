using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Api.Startup;

namespace Api.TypeScript.Objects
{
    /// <summary>
    /// Represents a TypeScript code generator for "includes" structures,
    /// supporting dynamic property chaining for virtual fields and entity-specific includes.
    /// </summary>
    /// <remarks>
    /// This class is used to generate TypeScript source that enables inclusion chains, commonly
    /// used in query systems (e.g., GraphQL-style field selection or API expansions).
    /// 
    /// It builds a base <c>ApiIncludes</c> class and, for each added entity, a corresponding
    /// <c>{Entity}Includes</c> class with accessors for virtual fields defined via
    /// <see cref="HasVirtualFieldAttribute"/>.
    /// </remarks>
    public class ApiIncludes : AbstractTypeScriptObject
    {
        private List<Type> _entities = [];

        /// <summary>
        /// Registers an entity type to be included in the TypeScript generation output.
        /// </summary>
        /// <param name="entityType">
        /// The CLR <see cref="Type"/> of the entity for which a corresponding <c>{Entity}Includes</c>
        /// TypeScript class will be generated.
        /// </param>
        public void AddEntity(Type entityType)
        {
            _entities.Add(entityType);
        }

        /// <summary>
        /// Appends the TypeScript source code that defines inclusion helpers for all registered entities.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="StringBuilder"/> to append the TypeScript source output to.
        /// </param>
        /// <param name="svc">
        /// A <see cref="TypeScriptService"/> that provides utilities like naming and formatting helpers.
        /// </param>
        /// <remarks>
        /// Generates:
        /// <list type="bullet">
        /// <item><description>The base <c>ApiIncludes</c> class with virtual field accessors from global definitions.</description></item>
        /// <item><description>One <c>{Entity}Includes</c> class per entity, reflecting its own virtual fields.</description></item>
        /// </list>
        /// These classes allow client-side TypeScript code to fluently chain field access expressions for API consumption.
        /// </remarks>
        public override void ToSource(StringBuilder builder, TypeScriptService svc)
        {
            builder.AppendLine("export class ApiIncludes {");

            builder.AppendLine("    private text: string = '';");

            builder.AppendLine();
            builder.AppendLine("    constructor(existing: string = '', addition: string = ''){");
            builder.AppendLine("        this.text = existing + (addition.length != 0 ? '.' + addition : '');");
            builder.AppendLine("        if (this.text[0] && this.text[0] == '.'){");
            builder.AppendLine("             this.text = this.text.substring(1, this.text.length);");
            builder.AppendLine("        }");
            builder.AppendLine("    }");

            builder.AppendLine();
            builder.AppendLine("    getText = () => this.text;");

            // Generate virtual field accessors from global virtual fields.
            foreach (var kvp in ContentFields.GlobalVirtualFields)
            {
                builder.AppendLine($"    get {TypeScriptService.LcFirst(kvp.Key)}() {{");
                builder.AppendLine($"        return new ApiIncludes(this.getText(), '{TypeScriptService.LcFirst(kvp.Key)}');");
                builder.AppendLine("    }");
            }

            builder.AppendLine("}");

            // Generate entity-specific includes classes
            foreach (var entity in _entities)
            {
                builder.AppendLine();
                builder.AppendLine($"export class {entity.Name}Includes extends ApiIncludes {{");

                var virtuals = entity.GetCustomAttributes<HasVirtualFieldAttribute>();

                foreach (var virtualField in virtuals)
                {
                    builder.AppendLine($"    get {TypeScriptService.LcFirst(virtualField.FieldName)}() {{");
                    builder.AppendLine($"        return new {entity.Name}Includes(this.getText(), '{virtualField.FieldName.ToLower()}');");
                    builder.AppendLine("    }");
                }

                builder.AppendLine("}");
            }
        }
    }
}



using System;
using System.Linq;
using System.Reflection;
using Api.AvailableEndpoints;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="script"></param>
        public static void CreateIncludeClass(Type entityType, Script script)
        {

            var aes = Services.Get<AvailableEndpointService>();

            var modules = aes.ListByModule().Where(mod => mod.GetContentType() == entityType);

            if (!modules.Any())
            {
                return;
            }

            var module = modules.First();

            var def = new ClassDefinition() {
                Name = entityType.Name + "Includes",
                Extends = "ApiIncludes"
            };

            var virtuals = entityType.GetCustomAttributes<HasVirtualFieldAttribute>();

            foreach(var virtualField in virtuals)
            {
                def.Children.Add(new ClassGetter() {
                    Name = LcFirst(virtualField.FieldName),
                    ReturnType = virtualField.Type.Name + "Includes",
                    Source = [
                        "return new " + virtualField.Type.Name + "Includes(this.text, '" + LcFirst(virtualField.FieldName) + "');"
                    ]
                });
            }

            def.Children.Add(new ClassGetter() {
                Name = "all",
                ReturnType = def.Name,
                Source = [
                    "return new " + def.Name + "(this.text, '.*')"
                ]
            });

            script.AddChild(def);
        }
    }
}
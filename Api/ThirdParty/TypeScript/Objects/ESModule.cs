using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Database;
using Newtonsoft.Json.Linq;

namespace Api.TypeScript.Objects
{
    /// <summary>
    /// Represents a TypeScript module that contains various generated TypeScript constructs
    /// such as types, enums, controllers, and includes. Responsible for emitting the full
    /// source file content for the module.
    /// </summary>
    public class ESModule : AbstractTypeScriptObject
    {
        // === Fields and Module Members ===

        private string _fileName;

        private readonly List<Import> _normalImports = [];
        private readonly List<WebApis> _usedWebApis = [];
        private readonly List<TypeDefinition> _types = [];
        private readonly List<ESEnum> _enums = [];
        private readonly List<GenericTypeList> _contentTypes = [];
        private readonly List<AutoController> _autoControllers = [];
        private readonly List<NonEntityController> _nonEntityControllers = [];
        private readonly List<ApiIncludes> _apiIncludes = [];
        private readonly List<Type> _virtualFieldImportSymbols = [];
        private readonly HashSet<Type> _typeRegistry = [];
        private bool _isEntityModule = false;

        private static string[] IgnoreNamespaces =
        [
            "Api.Eventing", 
            "Api.WebSockets",
            "Microsoft.AspNetCore",
            "Api.Database"
        ];

        private static string[] IgnoreTypes =
        [
            "Api.Startup.ContentStream`2",
            "Api.Startup.ContentStreamSource`2",
            "Api.Startup.ContentStream",
            "Api.Startup.ContentStreamSource",
            "Api.Startup.AutoService`2",
        ];

        private EntityController _entityController;

        // === File and Import Metadata ===

        /// <summary>
        /// Sets the output file name for this module.
        /// </summary>
        public void SetFileName(string fileName) => _fileName = fileName;
        
        public void MarkAsEntityModule() => _isEntityModule = true;
        
        public bool IsEntityModule() => _isEntityModule;

        /// <summary>
        /// Gets the full TypeScript file path for this module.
        /// </summary>
        public string GetFileName() =>
            "TypeScript/Api/" + _fileName + (!_fileName.EndsWith(".ts") && !_fileName.EndsWith(".tsx") ? ".ts" : "");

        /// <summary>
        /// Gets the import path (relative to other TypeScript modules).
        /// </summary>
        public string GetImportPath() => "Api/" + _fileName;

        // === Type/Enum Registration ===

        /// <summary>
        /// Registers a CLR type for TypeScript code generation.
        /// </summary>
        public void AddType(Type type)
        {
            type = TypeScriptService.UnwrapTypeNesting(type);

            if (type == typeof(void) || type == typeof(ValueTask) ||
                type == typeof(object) || type == typeof(JObject) ||
                type.Namespace == "System")
            {
                return;
            }
            
            if (type.IsGenericTypeDefinition)
            {
                if (type.GetGenericTypeDefinition() == typeof(AutoService<>))
                {
                    return;
                }
                if (type.GetGenericTypeDefinition() == typeof(AutoController<>))
                {
                    return;
                }
                if (type.GetGenericTypeDefinition() == typeof(AutoController<,>))
                {
                    return;
                }
                if (type.Name.Contains("Service"))
                {
                    return;
                }
            }

            if (type.BaseType is not null && type.BaseType.IsGenericTypeDefinition)
            {
                if (type.BaseType.GetGenericTypeDefinition() == typeof(AutoService<>))
                {
                    return;
                }
                if (type.BaseType.GetGenericTypeDefinition() == typeof(AutoController<>))
                {
                    return;
                }
                if (type.BaseType.GetGenericTypeDefinition() == typeof(AutoController<,>))
                {
                    return;
                }
                if (type.BaseType.Name.Contains("Service"))
                {
                    return;
                }
            }

            if (type == typeof(JsonString))
            {
                return;
            }

            foreach (var ns in IgnoreNamespaces)
            {
                if (type.FullName is not null)
                {
                    if (type.FullName.StartsWith(ns))
                    {
                        return;
                    }
                }
            }
            
            foreach (var ns in IgnoreTypes)
            {
                if (type.FullName is not null)
                {
                    if (type.FullName.StartsWith(ns))
                    {
                        return;
                    }
                }
            }

            // Prevent recursive cycles
            if (!_typeRegistry.Add(type)) return;

            // Limit to one entity per module
            if (TypeScriptService.IsEntityType(type) &&
                _types.Any(t => TypeScriptService.IsEntityType(t.GetReferenceType())))
            {
                return;
            }

            _types.Add(new TypeDefinition(type, this));
        }

        /// <summary>
        /// Adds an enum to be emitted as a TypeScript enum.
        /// </summary>
        public void AddEnum(Type type)
        {
            if (_enums.Any(en => en.GetReferenceType() == type)) return;
            _enums.Add(new ESEnum(type, this));
        }
        
        /// <summary>
        /// Adds a non-entity controller.
        /// </summary>
        /// <param name="type"></param>
        public void AddNonEntityController(Type type)
        {
            _nonEntityControllers.Add(new NonEntityController(type, this));
        }

        /// <summary>
        /// Checks if the module already contains a definition for the given type.
        /// </summary>
        public bool HasTypeDefinition(Type type, out TypeDefinition typeDef)
        {
            typeDef = _types.FirstOrDefault(t => t.GetName() == type.Name);
            return typeDef != null;
        }

        // === Imports ===

        /// <summary>
        /// Declares that the current module should import a symbol from another module.
        /// </summary>
        public void Import(string symbol, ESModule from)
        {
            if (from == this) return;

            var existing = _normalImports.FirstOrDefault(i => i.from == from);
            if (existing == null)
            {
                _normalImports.Add(new Import { from = from, Symbols = [symbol] });
            }
            else if (!existing.Symbols.Contains(symbol))
            {
                existing.Symbols.Add(symbol);
            }
        }

        /// <summary>
        /// Declares that the current module should import a type from another module.
        /// </summary>
        public void Import(Type type, ESModule from) => Import(type.Name, from);

        // === Web API Usage ===

        /// <summary>
        /// Marks a web API function as used so that it's imported during generation.
        /// </summary>
        public void RequireWebApi(WebApis webApi) => _usedWebApis.Add(webApi);

        // === Module Composition ===

        public void AddGenericTypes(GenericTypeList ctnt) => _contentTypes.Add(ctnt);

        public void AddGenericController(AutoController ctrlr) => _autoControllers.Add(ctrlr);

        public void AddInclude(ApiIncludes include) => _apiIncludes.Add(include);

        public void SetEntityController(EntityController entityController) => _entityController = entityController;

        public void SetEntityController(Type controller, Type entity) =>
            SetEntityController(new EntityController(controller, entity, this));

        public List<Type> GetRequiredImports()
        {
            var required = new List<Type>();
            
            required.AddRange(_nonEntityControllers.SelectMany(ctrller => ctrller.GetRequiredImports()));
            _types.ForEach(typeDef =>
            {
                required.AddRange(typeDef.GetDependencies());
            });

            return required;
        }
        
        // === Output ===

        /// <summary>
        /// Emits the TypeScript source content for this module.
        /// </summary>
        public override void ToSource(StringBuilder builder, TypeScriptService svc)
        {
            if (IsEmpty()) return;

            builder.AppendLine("/**");
            builder.AppendLine(" * This file was automatically generated. DO NOT EDIT.");
            builder.AppendLine(" */\n");

            // Web API imports
            if (_usedWebApis.Count != 0)
            {
                var apis = new List<string> { "ApiList" };

                if (_usedWebApis.Contains(WebApis.GetList)) apis.Add("getList");
                if (_usedWebApis.Contains(WebApis.GetOne)) apis.Add("getOne");
                if (_usedWebApis.Contains(WebApis.GetJson)) apis.Add("getJson");
                apis.Add("getText");

                builder.AppendLine($"import {{ {string.Join(", ", apis)} }} from 'UI/Functions/WebRequest';\n");
            }

            // Import requirements
            _types.ForEach(t => {
                t.ImportVirtualFields(svc.modules);
            });

            if (_normalImports.Count > 0)
            {
                builder.AppendLine("// IMPORTS");
                _normalImports.ForEach(i => i.ToSource(builder, svc));
                builder.AppendLine();
            }

            if (_enums.Count > 0)
            {
                builder.AppendLine("// ENUMS");
                _enums.ForEach(e => e.ToSource(builder, svc));
                builder.AppendLine();
            }

            if (_contentTypes.Count > 0)
            {
                builder.AppendLine("// OPEN GENERICS");
                _contentTypes.ForEach(g => g.ToSource(builder, svc));
                builder.AppendLine();
            }

            builder.AppendLine("// TYPES");
            _types.ForEach(type =>
            {
                if (!_normalImports.Any(import => import.Symbols.Contains(type.GetName())))
                {
                    type.ToSource(builder, svc);
                }
            });

            if (_entityController != null)
            {
                builder.AppendLine("// ENTITY CONTROLLER");
                _entityController.ToSource(builder, svc);
            }

            if (_autoControllers.Count > 0)
            {
                builder.AppendLine("// AUTO CONTROLLERS");
                _autoControllers.ForEach(ac => ac.ToSource(builder, svc));
            }
            
            if (_nonEntityControllers.Count > 0)
            {
                builder.AppendLine("// NON-ENTITY CONTROLLERS");
                _nonEntityControllers.ForEach(ac => ac.ToSource(builder, svc));
            }

            if (_apiIncludes.Count > 0)
            {
                builder.AppendLine("// INCLUDES");
                _apiIncludes.ForEach(inc => inc.ToSource(builder, svc));
            }
        }

        /// <summary>
        /// Creates a new empty module for a given entity and adds it to the given list.
        /// </summary>
        public static ESModule Empty(Type entityType, List<ESModule> modules)
        {
            var module = new ESModule();
            module.SetFileName(entityType.Name);
            modules.Add(module);
            return module;
        }

        /// <summary>
        /// Returns true if the module is empty and does not need to be emitted.
        /// </summary>
        public bool IsEmpty()
        {
            return _normalImports.Count == 0 &&
                   _enums.Count == 0 &&
                   _entityController == null &&
                   _contentTypes.Count == 0 &&
                   _apiIncludes.Count == 0 &&
                   _autoControllers.Count == 0 &&
                   _nonEntityControllers.Count == 0;
        }
    }

    /// <summary>
    /// Represents the specific Web API utilities that a TypeScript module might need to import.
    /// </summary>
    public enum WebApis
    {
        GetText,
        GetJson,
        GetOne,
        GetList
    }
}

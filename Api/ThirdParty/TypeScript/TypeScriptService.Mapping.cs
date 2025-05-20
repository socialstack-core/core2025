using System;
using System.Collections.Generic;

namespace Api.TypeScript
{
    public partial class TypeScriptService : AutoService
    {
        /// <summary>
        /// Adds the mechanic to allow overrides.
        /// </summary>
        private readonly Dictionary<Type, string> _overwriteTypes = [];

        private readonly List<Type> _ignoreTypes = [];

        /// <summary>
        /// Sets or replaces the TypeScript overwrite mapping for a given .NET type.
        /// </summary>
        public void SetTypeOverwrite(Type type, string overwrite)
        {
            if (type is null || string.IsNullOrWhiteSpace(overwrite))
            {
                return;
            }

            _overwriteTypes[type] = overwrite; // replaces or adds
        }

        /// <summary>
        /// Gets the TypeScript overwrite string for a given .NET type.
        /// </summary>
        /// <param name="type">The type to lookup.</param>
        /// <returns>The overwrite string if one exists; otherwise, null.</returns>
        public string GetTypeOverwrite(Type type)
        {
            return type is null ? null : _overwriteTypes.GetValueOrDefault(type);
        }

        public void AddIgnoreType(Type type)
        {
            _ignoreTypes.Add(type);
        }

        public bool IsIgnoreType(Type type)
        {
            return _ignoreTypes.Contains(type);
        }
    }
}
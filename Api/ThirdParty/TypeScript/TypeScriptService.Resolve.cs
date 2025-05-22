using System;
using System.Text;
using System.Threading.Tasks;
using Api.Startup;
using Google.Protobuf.WellKnownTypes;
using Type = System.Type;

namespace Api.TypeScript
{
    public partial class TypeScriptService : AutoService
    {
        /// <summary>
        /// Resolves the actual underlying type of a given <see cref="System.Type"/> by:
        /// - Unwrapping <see cref="Nullable{T}"/> to <c>T</c>
        /// - Extracting the inner type from <see cref="Task{TResult}"/> and <see cref="ValueTask{TResult}"/>
        /// - Mapping non-generic <see cref="Task"/> and <see cref="ValueTask"/> to <see cref="void"/>
        /// </summary>
        /// <param name="t">The type to resolve.</param>
        /// <returns>The resolved underlying type, if applicable; otherwise, the original type.</returns>
        /// <remarks>
        /// This method is useful for determining the "real" return type of asynchronous or nullable constructs,
        /// which are commonly used in API controllers and service contracts.
        /// </remarks>
        public Type ResolveType(Type t)
        {
            // Unwrap Nullable<T> to T
            t = Nullable.GetUnderlyingType(t) ?? t;

            // Handle Task<T> or ValueTask<T>
            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                {
                    // Extract and return the inner result type
                    return t.GetGenericArguments()[0];
                }
            }

            // Map Task or ValueTask (non-generic) to void
            if (t == typeof(Task) || t == typeof(ValueTask))
            {
                return typeof(void);
            }

            // Return type as-is if no transformation is required
            return t;
        }

        /// <summary>
        /// Generates a readable generic type signature from a given .NET type.
        /// For example, Dictionary&lt;string, List&lt;int&gt;&gt; will return "Dictionary&lt;string, List&lt;int&gt;&gt;".
        /// Handles both open and closed generic types.
        /// </summary>
        /// <param name="t">The type to generate a signature for.</param>
        /// <returns>A string representation of the generic type.</returns>
        public string GetGenericSignature(Type t)
        {
            string Resolve(Type type)
            {
                // Handle arrays with correct rank
                if (type.IsArray)
                {
                    var elementType = type.GetElementType()!;
                    var baseType = Resolve(elementType);

                    // Append [] for each dimension
                    var rank = type.GetArrayRank();
                    for (int i = 0; i < rank; i++)
                    {
                        baseType += "[]";
                    }
                    return baseType;
                }

                // Handle nullable types as (T | null)
                var underlying = Nullable.GetUnderlyingType(type);
                if (underlying != null)
                {
                    return $"({Resolve(underlying)} | null)";
                }

                // Custom type overwrites (C# → TS mapping)
                var overwrite = GetTypeOverwrite(type);
                if (overwrite is not null)
                {
                    return overwrite;
                }

                // Non-generic
                if (!type.IsGenericType)
                {
                    return type.Name;
                }

                // Special cases
                if (type.GetGenericTypeDefinition() == typeof(ContentStream<,>))
                {
                    return "ApiList<" + Resolve(type.GetGenericArguments()[0]) + ", " +
                           Resolve(type.GetGenericArguments()[1]) + ">";
                }

                if (type.GetGenericTypeDefinition() == typeof(ValueTask<>) ||
                    type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    return Resolve(type.GetGenericArguments()[0]);
                }

                if (IsEnumerable(type))
                {
                    var itemType = type.GetGenericArguments()[0];
                    return Resolve(itemType) + "[]";
                }

                // General generics
                var baseName = type.Name.Contains('`') ? type.Name[..type.Name.IndexOf('`')] : type.Name;
                var sb = new StringBuilder();
                sb.Append(baseName);
                sb.Append('<');

                var args = type.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(Resolve(args[i]));
                }

                sb.Append('>');
                return sb.ToString();
            }

            return Resolve(t);
        }



    }
}
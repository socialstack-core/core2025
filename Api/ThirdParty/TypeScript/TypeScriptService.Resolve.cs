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
            if (Nullable.GetUnderlyingType(t) != null)
            {
                t = Nullable.GetUnderlyingType(t);   
            }
            
            
            var overwrite = GetTypeOverwrite(t);

            if (overwrite is not null)
            {
                return overwrite;
            }

            
            // If the type is not generic at all, return its simple name
            if (!t.IsGenericType)
            {
                return t.Name;
            }
            if (t.GetGenericTypeDefinition() == typeof(ContentStream<,>))
            {
                return "ApiList<" + GetGenericSignature(t.GetGenericArguments()[0]) + ", " +
                       GetGenericSignature(t.GetGenericArguments()[1]) + ">";
            }
            if (t.GetGenericTypeDefinition() == typeof(ValueTask<>) || t.GetGenericTypeDefinition() == typeof(Task<>))
            {
                t = t.GetGenericArguments()[0];
            }
            // check now unwrapped from the valuetask.
            if (!t.IsGenericType)
            {
                return t.Name;
            }

            if (IsEnumerable(t))
            {
                // List<...> for instance.
                // this outputs for typescript.
                // so we need to swap List<..> => ..[]
                
                var dataType = t.GetGenericArguments()[0];

                return GetGenericSignature(dataType) + "[]";
            }

            // Base name before the backtick (e.g., "Dictionary`2" => "Dictionary")
            var baseName = t.Name.Contains('`') ? t.Name[..t.Name.IndexOf('`')] : t.Name;

            var sb = new StringBuilder();
            sb.Append(baseName);
            sb.Append('<');

            // Use GetGenericArguments() to include both open (T) and closed (int, string) generic types
            var args = t.GetGenericArguments();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                // Recursive call to support nested generic types
                sb.Append(GetGenericSignature(args[i]));
            }

            sb.Append('>');
            return sb.ToString();
        }
    }
}
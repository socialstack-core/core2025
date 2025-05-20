using System;
using Api.Database;
using Api.TypeScript.Objects;
using Api.Users;

namespace Api.TypeScript
{
    public partial class TypeScriptService : AutoService
    {
        public static bool IsEntityType(Type t)
        {
            while (t != null && t != typeof(object))
            {
                if (t.IsGenericType)
                {
                    var genericDef = t.GetGenericTypeDefinition();

                    if (genericDef == typeof(Content<>) ||
                        genericDef == typeof(UserCreatedContent<>) ||
                        genericDef == typeof(VersionedContent<>))
                    {
                        return true;
                    }
                }

                t = t.BaseType;
            }

            return false;
        }
        
        public bool  IsControllerDescendant(Type t)
        {
            while (t != null && t != typeof(object))
            {
                if (t.IsGenericType)
                {
                    var genericDef = t.GetGenericTypeDefinition();

                    if (genericDef == typeof(AutoController<>) || 
                        genericDef == typeof(AutoController<,>))
                    {
                        return true;
                    }
                }
                else if (t == typeof(AutoController))
                {
                    return true;
                }

                t = t.BaseType;
            }

            return false;
        }

        public bool IsEntityController(Type t, out Type entityType)
        {
            entityType = null;

            while (t != null && t != typeof(object))
            {
                if (t.IsGenericType)
                {
                    var genericDef = t.GetGenericTypeDefinition();

                    if (genericDef == typeof(AutoController<>) || genericDef == typeof(AutoController<,>))
                    {
                        entityType = t.GetGenericArguments()[0];
                        return true;
                    }
                }

                t = t.BaseType;
            }

            return false;
        }

    }
}

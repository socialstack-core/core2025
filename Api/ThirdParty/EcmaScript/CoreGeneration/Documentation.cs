

using System;
using System.Reflection;
using Api.AvailableEndpoints;

namespace Api.EcmaScript
{
    public partial class EcmaService : AutoService
    {
        /// <summary>
        /// Retrieves the documentation for a given type.
        /// </summary>
        /// <param name="type">The type to get the documentation for.</param>
        /// <returns>The documentation content (CSDoc) for the type.</returns>
        public XmlDocType GetTypeDocumentation(Type type)
        {
            var doc = endpointService.Documentation;
            var result = doc.GetType(type.FullName, false);

            return result;
        }

        /// <summary>
        /// Gets the method documentation
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public XmlDocMember GetMethodDocumentation(MethodInfo methodInfo)
        {
            var result = GetTypeDocumentation(methodInfo.DeclaringType);

            if (result != null && result.Members.TryGetValue(methodInfo.Name, out XmlDocMember member))
            {
                if (member.Type == XmlDocMemberType.Method)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the property documentation
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public XmlDocMember GetPropertyDocumentation(PropertyInfo propertyInfo)
        {
            var result = GetTypeDocumentation(propertyInfo.DeclaringType);

            if (result.Members.TryGetValue(propertyInfo.Name, out XmlDocMember member))
            {
                if (member.Type == XmlDocMemberType.Property)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the property documentation
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public XmlDocMember GetPropertyDocumentation(FieldInfo propertyInfo)
        {
            var result = GetTypeDocumentation(propertyInfo.DeclaringType);

            if (result.Members.TryGetValue(propertyInfo.Name, out XmlDocMember member))
            {
                if (member.Type == XmlDocMemberType.Property)
                {
                    return member;
                }
            }
            return null;
        }
    }
}
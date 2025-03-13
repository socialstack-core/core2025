using System;

namespace Api.Startup
{
    /// <summary>
    /// An attribute to specify the return type of a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ReturnsAttribute : Attribute
    {
        /// <summary>
        /// Gets the return type specified by the attribute.
        /// </summary>
        public readonly Type ReturnType;


        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsAttribute"/> class.
        /// </summary>
        /// <param name="type">The return type of the method.</param>
        public ReturnsAttribute(Type type)
        {
            ReturnType = type;
        }
    }
}
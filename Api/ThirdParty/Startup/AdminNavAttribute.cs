using System;

namespace Api.Startup
{
    /// <summary>
    /// This attribute can be assigned to entities
    /// and allows a custom nav icon to be assigned.
    /// </summary>
    /// <param name="icon"></param>
    [AttributeUsage(AttributeTargets.Class)]
    public class AdminNavAttribute(string icon, string label = null) : Attribute
    {
        /// <summary>
        /// The admin panel icon to use.
        /// </summary>
        public readonly string Icon = icon;
        
        /// <summary>
        /// Overwrite the label
        /// </summary>
        public readonly string Label = label;
    }
}
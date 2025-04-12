using System;

namespace Api.Startup
{
    /// <summary>
    /// This attribute allows you to specify a custom create page that gets used in
    /// InstallAdminPages()
    /// </summary> 
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomCreatePageAttribute(
        string component
        ) : Attribute
    {
        /// <summary>
        /// The target component for the custom create page
        /// </summary>
        public string Component = component;
    }

    /// <summary>
    /// This attribute allows you to specify a custom edit page that gets used in
    /// InstallAdminPages()
    /// </summary> 
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEditPageAttribute(
        string component
        ) : Attribute
    {
        /// <summary>
        /// The target component for the custom edit page
        /// </summary>
        public string Component = component;
    }

    /// <summary>
    /// This attribute allows you to specify a custom list page that gets used in
    /// InstallAdminPages()
    /// </summary> 
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomListPageAttribute(
        string component
        ) : Attribute
    {
        /// <summary>
        /// The target component for the custom list page
        /// </summary>
        public string Component = component;
    }

}
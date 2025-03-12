namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// Represents an argument for a TypeScript class method.
    /// </summary>
    public partial class ClassMethodArgument : IGeneratable
    {
        /// <summary>
        /// Gets or sets the name of the argument.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the TypeScript type of the argument.
        /// </summary>
        public string Type;

        /// <summary>
        /// Gets or sets the default value of the argument, if any.
        /// </summary>
        public string DefaultValue;

        /// <summary>
        /// Generates the TypeScript source representation of the argument.
        /// </summary>
        /// <returns>A string representing the argument in TypeScript syntax.</returns>
        public string CreateSource()
        {
            var src = $"{Name}: {Type}";

            if (!string.IsNullOrEmpty(DefaultValue))
            {
                if (Type == "string")
                {
                    src += $" = '{DefaultValue.Replace("'", "\\'")}'";
                }
                else
                {
                    src += $" = {DefaultValue}";
                }
            }
            return src;
        }
    }
}

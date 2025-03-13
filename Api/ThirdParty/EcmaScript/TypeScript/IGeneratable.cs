

namespace Api.EcmaScript.TypeScript
{
    /// <summary>
    /// The most primitive thing in the TypeScript module
    /// </summary>
    public interface IGeneratable
    {
        /// <summary>
        /// Takes relevant information and outputs TypeScript Source Code.
        /// </summary>
        /// <returns></returns>
        public string CreateSource();
        
        /// <summary>
        /// Returns the TSDocumentation for each entity type.
        /// </summary>
        /// <returns></returns>
        public string GetTsDocumentation();

        /// <summary>
        /// Allows a line to be entered.
        /// </summary>
        /// <param name="line"></param>
        public void AddTsDocLine(string line);
    }
}
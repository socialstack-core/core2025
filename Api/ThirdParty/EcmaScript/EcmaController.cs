using Microsoft.AspNetCore.Mvc;

namespace Api.EcmaScript
{
    /// <summary>
    /// Instanced automatically.
    /// </summary>
    [Route("v1/ecma")]
    public partial class EcmaController : AutoController
    {
        /// <summary>
        /// A test endpoint
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public string TestResponse()
        {
            return "Hello world";
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.ClearScript;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {

        private static readonly string[] IgnoreTypes = [
            "ID",
            "T",
            "Record",
            "React.FC"
        ];

        /// <summary>
        /// 
        /// </summary>
        public static void Validate()
        {
            scripts.ForEach(script => {

                ValidateTypes(script);

            });
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Api.CanvasRenderer;

namespace Api.EcmaScript
{
    public partial class EcmaService : AutoService
    {

        /// <summary>
		/// Dev watcher mode only. Outputs a tsconfig.json file which lists all available JS/ JSX/ TS/ TSX files.
		/// </summary>
		private void BuildTypescriptAliases(List<UIBundle> sourceBuilders)
		{
			// Do any builders have typescript files in them?
			var ts = false;
			foreach (var builder in sourceBuilders)
			{
				if (builder.HasTypeScript)
				{
					ts = true;
					break;
				}
			}

			if (!ts)
			{
				return;
			}

			var output = new StringBuilder();
			
			output.Append("{\r\n\"compilerOptions\": {\"jsx\": \"react-jsx\", \"paths\": {");
			var first = true;
			
			foreach (var builder in sourceBuilders)
			{
				var rootSegment = "\":[\"" + "./" + builder.RootName + "/Source/";

				foreach (var kvp in builder.FileMap)
				{
					var file = kvp.Value;

					if (file.FileType != SourceFileType.Javascript)
					{
						continue;
					}

					if (first)
					{
						first = false;
					}
					else
					{
						output.Append(',');
					}

					var firstDot = file.FileName.IndexOf('.');
					var nameNoType = firstDot == -1 ? file.FileName : file.FileName.Substring(0, firstDot);
					
					var modPath = file.ModulePath;
					var modPathDot = file.ModulePath.LastIndexOf('.');
					
					
					if(modPathDot != -1){
						// It has a filetype - strip it:
						modPath = modPath.Substring(0, modPathDot);
					}
					
					output.Append('"');
					output.Append(modPath);
					output.Append(rootSegment);
					output.Append(file.RelativePath.Replace('\\', '/') + '/' + nameNoType);
					output.Append("\"]");
				}
			}

			output.Append("}}}");

			// tsconfig.json:
			var json = output.ToString();

			var tsMeta = Path.GetFullPath("TypeScript");

			// Create if doesn't exist:
			Directory.CreateDirectory(tsMeta);
			
			// Write tsconfig file out:
			File.WriteAllText(Path.Combine(tsMeta, "tsconfig.generated.json"), json);
			
			
			/*
			var globalsPath = Path.Combine(tsMeta, "typings.d.ts");

			if (!File.Exists(globalsPath))
			{
				File.WriteAllText(globalsPath, "import * as react from \"react\";\r\n\r\ndeclare global {\r\n\ttype React = typeof react;\r\n\tvar global: any;\r\n}");
			}
			*/
		}
    }
}
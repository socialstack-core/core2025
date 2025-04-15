

using System.Collections.Generic;
using System.IO;
using System.Text;
using Api.CanvasRenderer;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    public partial class EcmaService
    {
        /// <summary>
        /// Adds the CRUD Functionality to a class.
        /// </summary>
        /// <param name="baseControllerClass"></param>
        private void AddCrudFunctionality(ClassDefinition baseControllerClass)
        {
            var listMethod = new ClassMethod
            {
                Name = "list",
                ReturnType = "Promise<ApiList<EntityType>>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "where",
                        Type = "Partial<Record<keyof(EntityType), string | number | boolean>>",
                        DefaultValue = "{}"
                    },
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ], 
                Injected = [
                    "return getList(this.apiUrl + '/list', { where }, { method: 'POST', includes: includes?.map(include => include.toString()) })"
                ]
            };
            var oneMethod = new ClassMethod() {
                Name = "load", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "id",
                        Type = "number"
                    },
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl + '/' + id, { includes: includes?.map(include => include.toString()) })"
                ]
            };
            var createMethod = new ClassMethod() {
                Name = "create", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl, entity)"
                ]
            };
            var updateMethod = new ClassMethod() {
                Name = "update", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl + '/' + entity.id, entity)"
                ]
            };

            var deleteMethod = new ClassMethod() {
                Name = "delete",
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entityId",
                        Type = "number" 
                    },                    
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ], 
                Injected = [
                    "return getOne(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: includes?.map(include => include.toString()) })"
                ]
            };

            var constructorMethod = new ClassMethod() {
                Name = "constructor",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "apiUrl", 
                        Type = "string"
                    }
                ],
                Injected = [
                    "this.apiUrl = apiUrl;"
                ]
            };

            baseControllerClass.Children.Add(new ClassProperty() {
                PropertyName = "includes",
                PropertyType = "IncludeSet | null",
                DefaultValue = "null"
            });
            
            baseControllerClass.Children.Add(constructorMethod);
            baseControllerClass.Children.Add(listMethod);
            baseControllerClass.Children.Add(oneMethod);
            baseControllerClass.Children.Add(createMethod);
            baseControllerClass.Children.Add(updateMethod);
            baseControllerClass.Children.Add(deleteMethod);

        }

        

        private void AddBaseIncludeFunctionality()
        {
            

            var classDef = new ClassDefinition() {
                Name = "ApiIncludes",
            };

            var property = new ClassProperty() {
                PropertyName = "text",
                PropertyType = "string",
                Visibility = "protected" // musn't be accessible outside of the class, but must be accessible to children classes
            };

            var constructor = new ClassMethod() {
                Name = "constructor",
                Arguments = [
                    new() {
                        Name = "prev?",
                        Type = "string"
                    },
                    new() {
                        Name = "extra?",
                        Type = "string"
                    }
                ],
                Injected = [
                    "this.text = (prev ? prev + '.' : '') + (extra || '');"
                ]
            };
            var toString = new ClassMethod() {
                Name = "toString",
                ReturnType = "string",
                Injected = [
                    "return this.text;"
                ]
            };
            
            classDef.Children.Add(constructor);
            classDef.Children.Add(property);
            classDef.Children.Add(toString);

            IncludesScript.AddChild(classDef);
        }

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
				var rootSegment = "\":[\"";

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
					output.Append(file.Path.Replace('\\', '/'));
					output.Append("\"]");
				}
			}

            output.Append(", \"Api/*\": [\"TypeScript/Api/*\"]");

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
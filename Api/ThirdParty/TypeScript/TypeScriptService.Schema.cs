using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Api.CanvasRenderer;
using Api.Database;
using Api.Startup;
using Api.TypeScript.Objects;
using Api.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.TypeScript
{
    public partial class TypeScriptService : AutoService
    {
        internal List<ESModule> modules = [];
        
        public void CreateApiSchema(SourceFileContainer container)
        {
            // kept in as to not break anything.
            ContextGenerator.SaveToFile("TypeScript/Config/Session.tsx");
            
            var allContent = _aes.ListByModule();

            foreach (var module in allContent)
            {
                var entity = module.GetContentType();
                var controller = module.ControllerType;

                if (entity is not null)
                {
                    AddType(entity);
                }

                if (controller is not null)
                {
                    AddType(controller);   
                }
            }
            
            var includesScript = ESModule.Empty(typeof(void), modules);
            includesScript.SetFileName("Includes");

            var includes = new ApiIncludes();
            
            includesScript.AddInclude(includes);
            
            var content = ESModule.Empty(typeof(Content<>), modules);
            
            // this can then be imported via:
            // Api/Content.
            content.SetFileName("Content");

            var generics = new GenericTypeList();
            content.AddGenericTypes(generics);
            
            content.RequireWebApi(WebApis.GetList);
            content.RequireWebApi(WebApis.GetJson);
            content.RequireWebApi(WebApis.GetOne);
            
            generics.AddContentType(typeof(Content<>));
            generics.AddContentType(typeof(UserCreatedContent<>));
            generics.AddContentType(typeof(VersionedContent<>));
            
            content.AddType(typeof(ListFilter));
            
            content.AddGenericController(new Objects.AutoController(content));
            
            
            GetAllTypes().ForEach(type =>
            {
                if (type.IsEnum)
                {
                    // handle differently.
                    
                    return;
                }

                if (IsControllerDescendant(type))
                {
                    if (IsEntityController(type, out var entityType))
                    {
                        var module = modules.FirstOrDefault(mod => mod.HasTypeDefinition(type, out _)) ?? ESModule.Empty(type, modules);
                        module.SetFileName(entityType.Name);
                        
                        
                        module.Import("Content", content);
                        module.Import("UserCreatedContent", content);
                        module.Import("VersionedContent", content);
                        module.Import("AutoController", content);
                        
                        module.Import(GetGenericSignature(entityType) + "Includes", includesScript);
                        includes.AddEntity(entityType);

                        content.Import("ApiIncludes", includesScript);
                        
                        
                        // create entity.
                        // create controller.
                        // create includes
                        // step 4... free lollipops

                        if (!module.HasTypeDefinition(entityType, out _))
                        {
                            module.AddType(entityType);   
                        }
                        module.SetEntityController(type, entityType);
                    }
                    else
                    {
                        if (type.BaseType == typeof(ControllerBase) || type.BaseType == typeof(AutoController))
                        {
                            var module = ESModule.Empty(type, modules);
                            module.SetFileName(type.Name);
                            module.AddNonEntityController(type);
                        }
                    }
                }
            });
            
            modules.ForEach(module =>
            {
                var builder = new StringBuilder();
                module.ToSource(builder, this);
                
                container.Add(module.GetFileName(), builder.ToString());
                File.WriteAllText(module.GetFileName(), builder.ToString());
            });


        }
    }
}
using System;

namespace Api.EcmaScript.Markdown
{
    public static partial class MarkdownGeneration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        public static void AddUsage(Type entityType)
        {
            var document = GetDocument(entityType);

            document.AddHeading("Usage", 3);
            document.AddParagraph("This entity works with some core components, such as Form & Loop");

            // add form example
            document.AddHeading("Form", 4);
            
            // add create functionality
            document.AddHeading("Create", 5);
            document.AddCodeBlock(@$"
<Form action={{{entityType.Name}Api.create}}>
    {{/* Add children here */}}
</Form>", "tsx");

            // add update functionality
            document.AddHeading("Update", 5);
            document.AddCodeBlock(@$"
<Form action={{(values) => {{ 
    return {entityType.Name}Api.update(id, values); 
}}}}>
    {{/* Add children here */}}
</Form>", "tsx");

            // add loop example
            document.AddHeading("Loop", 4);
            document.AddCodeBlock(@$"
<Loop over={{{entityType.Name}Api}} filter={{ /* filters */ }}>
    {{(entity: {entityType.Name}) => {{
        return (
            // Loop logic here
        );
    }}
</Loop>", "tsx");
        }
    }
}

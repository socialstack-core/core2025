using System;
using System.Text;

namespace Api.TypeScript.Objects
{
    public class TypeScriptField : AbstractTypeScriptObject
    {
        public string FieldName { get; set; }
    
        public Type FieldType { get; set; }
    
        public override void ToSource(StringBuilder builder, TypeScriptService svc)
        {
            builder.AppendLine(
            $"    {TypeScriptService.LcFirst(FieldName)}{(TypeScriptService.IsNullable(FieldType) ? "?:" : ":")} {svc.GetGenericSignature(FieldType)}"
            );
        }
    }
}
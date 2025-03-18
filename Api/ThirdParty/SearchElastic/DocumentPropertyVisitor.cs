using Nest;
using System.Reflection;
using System;

namespace Api.SearchElastic
{
    /// <summary>
    /// No longer used but good example of synamically extending auto mapping
    /// </summary>
    public class AddKeyWordSubField : Attribute { }

    /// <summary>
    /// No longer used but good example of synamically extending auto mapping
    /// </summary>
    public class DocumentPropertyVisitor : NoopPropertyVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="attribute"></param>
        public override void Visit(ITextProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
        {
            base.Visit(type, propertyInfo, attribute);

            var wsaf = propertyInfo.GetCustomAttribute<AddKeyWordSubField>();
            if (wsaf != null)
            {
                // Add a keyword sub field to allow sorting etc (can also be achieved by adding no attribute to string field)
                type.Fields = new Properties
                {
                    {
                        "keyword",
                        new KeywordProperty
                        {
                            IgnoreAbove = 256,
                            Normalizer = "useLowercase"
                        }
                    }
                };
            }
        }
    }
}


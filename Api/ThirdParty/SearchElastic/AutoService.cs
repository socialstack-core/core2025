using Api.Contexts;
using Api.Startup;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Api.Tags;

public partial class AutoService
{
    /// <summary>
    /// Find unique values within content for filtering etc
    /// </summary>
    /// <returns></returns>
    public virtual ValueTask<List<Tag>> ExtractTaxonomy(Context context, string key)
    {
        // This service doesn't have a content type thus can just safely do nothing at all.
        return new ValueTask<List<Tag>>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fieldName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual ValueTask<bool> HasBooleanValue(Context context, string fieldName, object data)
    {
        // This service doesn't have a content type thus can just safely do nothing at all.
        return new ValueTask<bool>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual ValueTask<bool> HasMatchingValue(Context context, string fieldName, string fieldValue, object data)
    {
        // This service doesn't have a content type thus can just safely do nothing at all.
        return new ValueTask<bool>();
    }

}
public partial class AutoService<T, ID>
{

    /// <summary>
    /// Extract any taxonomy items
    /// </summary>
    /// <returns></returns>
    public override async ValueTask<List<Tag>> ExtractTaxonomy(Context context, string key)
    {
        JsonField keyField = null;

        // Get the field info:
        var fieldInfo = await GetJsonStructure(context);

        foreach (var fieldPair in fieldInfo.AllFields)
        {
            if (fieldPair.Key.ToLower() == key.ToLower())
            {
                keyField = fieldPair.Value;
                break;
            }
        }

        if (keyField == null)
        {
            return null;
        }

        // find any content
        var filter = Where();
        var contentObjects = await filter.ListAll(context);

        if (contentObjects == null || !contentObjects.Any())
        {
            return null;
        }

        var keys = new List<Tag>();

        // Check each object in the matching service
        foreach (var contentObject in contentObjects)
        {
            var keyValue = keyField.FieldInfo.GetValue(contentObject) as string;

            if (!string.IsNullOrWhiteSpace(keyValue) || ! keys.Any(s => s.Name.Equals(keyValue, StringComparison.InvariantCultureIgnoreCase)))
            {
                keys.Add(new Tag() { Name = keyValue , Order = 1});
            }
        }

        if (keys.Count == 0)
        {
            return null;
        }

        return keys.OrderBy(s => s.Name).ToList();

    }

    /// <summary>
    /// Check the data object for a boolean value and pass back the value 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fieldName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public override async ValueTask<bool> HasBooleanValue(Context context, string fieldName, object data)
    {
        JsonField booleanField = null;

        // Get the field info:
        var fieldInfo = await GetJsonStructure(context);

        foreach (var fieldPair in fieldInfo.AllFields)
        {
            if (fieldPair.Key.ToLower() == fieldName.ToLower())
            {
                booleanField = fieldPair.Value;
                break;
            }
        }

        if (booleanField == null)
        {
            return false;
        }

        return (booleanField.FieldInfo.GetValue(data) as bool?).GetValueOrDefault(false);
    }

    /// <summary>
    /// Check the data object for a matching field and value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public override async ValueTask<bool> HasMatchingValue(Context context, string fieldName, string fieldValue, object data)
    {
        JsonField matchingField = null;

        // Get the field info:
        var fieldInfo = await GetJsonStructure(context);

        foreach (var fieldPair in fieldInfo.AllFields)
        {
            if (fieldPair.Key.ToLower() == fieldName.ToLower())
            {
                matchingField = fieldPair.Value;
                break;
            }
        }

        if (matchingField == null)
        {
            return false;
        }

        if (matchingField.FieldInfo.GetValue(data) == null)
        {
            return false;
        }
        
        return matchingField.FieldInfo.GetValue(data).ToString().Equals(fieldValue, StringComparison.InvariantCultureIgnoreCase);
    }


    /// <summary>
    /// Check the data object for a value and pass back the value 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fieldName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async ValueTask<object> GetFieldValue(Context context, string fieldName, object data)
    {
        JsonField field = null;

        // Get the field info:
        var fieldInfo = await GetJsonStructure(context);

        foreach (var fieldPair in fieldInfo.AllFields)
        {
            if (fieldPair.Key.ToLower() == fieldName.ToLower())
            {
                field = fieldPair.Value;
                break;
            }
        }

        if (field == null)
        {
            return null;
        }

        return field.FieldInfo.GetValue(data);
    }


}


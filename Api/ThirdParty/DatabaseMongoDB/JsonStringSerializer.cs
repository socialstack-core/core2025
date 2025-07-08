using Api.Database;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

/// <summary>
/// A serializer for the JsonString type.
/// </summary>
public class JsonStringSerializer : IBsonSerializer<JsonString>
{
    /// <summary>
    /// The type that this serializer serializes (JsonString in this case).
    /// </summary>
    public Type ValueType => typeof(JsonString);

    /// <summary>
    /// Serialize the given value.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="args"></param>
    /// <param name="value"></param>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonString value)
    {
        var val = value.ValueOf();

        if (string.IsNullOrWhiteSpace(val))
        {
            context.Writer.WriteNull();
            return;
        }

        // Parse the JSON string into a BsonDocument
        var doc = BsonDocument.Parse(val);
        BsonDocumentSerializer.Instance.Serialize(context, doc);
    }

    /// <summary>
    /// Deserialize from a contextual bson document to he value.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public JsonString Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        // Deserialize the subdocument into a BsonDocument
        var doc = BsonDocumentSerializer.Instance.Deserialize(context);

        // Convert the BsonDocument back into a JSON string
        var json = doc.ToJson(new JsonWriterSettings {
            OutputMode = JsonOutputMode.RelaxedExtendedJson,
            // Turning on indentation but setting the chars to
            // blank eliminates most of the default spaces it adds.
			Indent = true,
			NewLineChars = "",
			IndentChars = ""
		});

        return new JsonString(json);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (JsonString)value);
}
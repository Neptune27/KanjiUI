using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KBE.Components.Kanji.Mazii;

internal class EmptyStringToArrayConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rootElement = JsonDocument.ParseValue(ref reader);

        // if its array return new instance or null
        if (reader.TokenType == JsonTokenType.String)
        {
            // return default(T); // if you want null value instead of new instance
            return (T)Activator.CreateInstance(typeof(T));
        }
        else
        {
            var text = rootElement.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(text, options);
        }
    }
    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KBE.Components.Translator;


public class GoogleTranslationPASentence
{
    [JsonPropertyName("trans")]
    public string Trans { get; set; }

    [JsonPropertyName("orig")]
    public string Orig { get; set; }
}

public class GoogleTranslationPA
{
    [JsonPropertyName("translation")]
    public string Translation { get; set; }

    [JsonPropertyName("sentences")]
    public List<GoogleTranslationPASentence> Sentences { get; set; }

    [JsonPropertyName("sourceLanguage")]
    public string SourceLanguage { get; set; }
}

[JsonSerializable(typeof(GoogleTranslationPA))]
internal partial class GoogleTranslationPaJsonContext : JsonSerializerContext
{
}
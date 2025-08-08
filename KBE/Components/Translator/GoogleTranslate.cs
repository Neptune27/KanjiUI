using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using KBE.Components.Settings;
using KBE.Components.Utils;

namespace KBE.Components.Translator;

public class GoogleTranslate
{

    private static string ToProtectedString(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        return text.Replace("\n", "[[CN]]").Replace("\r", "[[CR]]");
    }

    private static string ToNormalized(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        return text.Replace("[[CN]]", "\n").Replace("[[CR]]", "\r");

    }

    public static async ValueTask<string> TranslatePA(string input, string fromCode, string toCode, IProgress<int>? progress)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }


        var inputChunks = ToProtectedString(input).ToChunks(Setting.Instance.TranslateChunkSize);
        var totalChunk = inputChunks.Count();
        string translation = "";

        var total = 0.0;
        foreach (var chunk in inputChunks)
        {
            var url =
                $"https://translate-pa.googleapis.com/v1/translate?params.client=gtx&query.source_language={fromCode}&query.target_language={toCode}&query.text={Uri.EscapeDataString(chunk)}&key=AIzaSyDLEeFI5OtFBwYBIoK_jj5m32rZK5CkCXA&data_types=TRANSLATION&data_types=SENTENCE_SPLITS&data_types=BILINGUAL_DICTIONARY_FULL";

            total += 1;
            try
            {
                using HttpClient client = new();
                var result = await client.GetStringAsync(url);
                var jsonData = JsonSerializer.Deserialize(result, GoogleTranslationPaJsonContext.Default.GoogleTranslationPA);

                if (jsonData == null) { return ""; }
               
                Setting.Logger.Information("Done parsing {@total} chunk", total);
                translation += jsonData.Translation;
                progress?.Report((int)(total / totalChunk * 100));
            }
            catch (HttpRequestException)
            {
                throw new TranslatorFetchSizeException($"Fetch size {Setting.Instance.TranslateChunkSize} is too large!");
            }

        }
        return ToNormalized(translation);


    }
    public static async ValueTask<string> Translate(string input, string fromCode, string toCode,
        IProgress<int>? progress)
    {
        //TODO Make the correct way to split
        if (String.IsNullOrEmpty(input))
        {
            return "";
        }

        var inputChunks = input.ToChunks(Setting.Instance.TranslateChunkSize);
        var totalChunk = inputChunks.Count();
        string translation = "";

        var total = 0.0;
        using HttpClient client = new();

        foreach ( var chunk in inputChunks )
        {
            var url =
                $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromCode}&tl={toCode}&dt=t&q={Uri.EscapeDataString(chunk)}";

            total += 1;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<List<dynamic>>(result);

                if (jsonData == null) { return ""; }

                try
                {
                    var index = 0;
                    while (true)
                    {
                        translation += jsonData[0][index][0];
                        index++;
                    }

                }
                catch (IndexOutOfRangeException)
                {
						Setting.Logger.Information("Done parsing {@total} chunk", total);
                    progress?.Report((int)(total / totalChunk * 100));
                }
            }
            catch (HttpRequestException)
            {
                throw new TranslatorFetchSizeException($"Fetch size {Setting.Instance.TranslateChunkSize} is too large!");
            }
            
        }
        return translation;

    }
}

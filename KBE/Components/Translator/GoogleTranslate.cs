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

namespace KBE.Components.Translator
{
    public class GoogleTranslate
    {
        private static IEnumerable<string> Chunks(string str, int maxChunkSize)
        {
            if (maxChunkSize <= 0)
            {
                throw new TranslatorFetchSizeException($"Fetch size cannot be lower than 1!");
            }
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public static async ValueTask<string> Translate(string input, string fromCode, string toCode,
            IProgress<int>? progress)
        {
            //TODO Make the correct way to split
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }
            var inputChunks = Chunks(input, Setting.Instance.TranslateChunkSize);
            var totalChunk = inputChunks.Count();
            string translation = "";

            var total = 0.0;
            foreach ( var chunk in inputChunks )
            {
                var url =
                    $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromCode}&tl={toCode}&dt=t&q={Uri.EscapeDataString(chunk)}";

                total += 1;
                try
                {
                    using HttpClient client = new();
                    var result = await client.GetStringAsync(url);
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
                        Debug.WriteLine($"Done parsing {total} chunk");
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
}

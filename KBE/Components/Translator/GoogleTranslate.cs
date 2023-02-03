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

        static IEnumerable<string> Chunks(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        public static async ValueTask<string> Translate(string input, string fromCode, string toCode, IProgress<int>? progress)
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
                string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                    fromCode, toCode, Uri.EscapeDataString(chunk));

                total += 1;

                try
                {
                    using (HttpClient client = new())
                    {
                        string result = await client.GetStringAsync(url);
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

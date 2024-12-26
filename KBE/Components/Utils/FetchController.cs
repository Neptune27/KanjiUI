using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBE.Components.Settings;
using KBE.Enums;

namespace KBE.Components.Utils
{

    class Fetching
    {
        private static readonly HttpClient Client = new();
        private readonly List<Task<HttpResponseMessage>> requests = new();

        //
        private List<string> Urls { get; set; } = new List<string> { };

        private List<Dictionary<string, string>>
            Values
        { get; set; } = new();

        private static Setting Setting => Setting.Instance;

        public static int Size { get => Setting.FetchSize; set => Setting.FetchSize = value; }
        private int offset = 0;
        public readonly FetchingOptions Options;

        public Fetching(FetchingOptions options = FetchingOptions.GET)
        {
            this.Options = options;
        }

        private void QueueRequest()
        {
            requests.Clear();
            var end = offset + Size;
            for (int i = offset; i < end; i++)
            {
                if (i >= Urls.Count)
                {
                    break;
                }
                if (Options == FetchingOptions.GET)
                {
                    requests.Add(Client.GetAsync(Urls[i]));
                }
                else
                {
                    var content = new FormUrlEncodedContent(Values[i]);
                    requests.Add(Client.PostAsync(Urls[i], content));
                }
            }
            offset += Size;
        }

        public async Task<List<string>> FetchURLs(IProgress<int>? progress)
        {
            var lst = new List<string>();


            QueueRequest();
            while (requests.Count != 0)
            {
                await Task.WhenAll(requests);
                var responses = requests.Select
                    (
                        task => task.Result
                    );
                foreach (var response in responses)
                {
                    lst.Add(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
                progress?.Report(lst.Count / Urls.Count * 100);
                QueueRequest();
            }

            return lst;
        }

        static async Task<List<string>> GetJisho(IEnumerable<string> strings, IProgress<int>? progress)
        {
            var fetcher = new Fetching();
            foreach (var str in strings)
            {
                fetcher.Urls.Add($"https://jisho.org/search/{str}%20%23kanji");
            }
            return await fetcher.FetchURLs(progress);
        }

        static async Task<List<string>> GetMazii(IEnumerable<string> strings, IProgress<int>? progress)
        {
            var fetcher = new Fetching(options: FetchingOptions.POST);
            foreach (var str in strings)
            {
                fetcher.Urls.Add($"https://mazii.net/api/search");
                var dict = new Dictionary<string, string>
                {
                    { "dict", "javi" },
                    { "page", "1" },
                    { "query", $"{str}" },
                    { "type", "kanji" }
                };
                fetcher.Values.Add(dict);
            }
            return await fetcher.FetchURLs(progress);
        }

        static async public Task<Dictionary<string, string[]>> FetchAll(IEnumerable<string> strings, IProgress<int>? jishoProgress = null, IProgress<int>? maziiProgress = null)
        {
            var jishoTask = GetJisho(strings, jishoProgress);
            var maziiTask = GetMazii(strings, maziiProgress);

            var jisho = await jishoTask.ConfigureAwait(false);
            var mazii = await maziiTask.ConfigureAwait(false);

            var dict = strings.Select((chr, index) => new { chr, index })
                .ToDictionary(pair => pair.chr, pair => new string[2] { jisho[pair.index], mazii[pair.index] });

            return dict;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components
{

    enum FetchingOptions
    {
        POST,
        GET
    }

    class Fetching
    {
        private static readonly HttpClient client = new();
        private List<Task<HttpResponseMessage>> requests = new();

        //
        public List<string> Urls { get; set; } = new List<string> { };
        public List<Dictionary<string, string>>
            Values
        { get; set; } = new();


        readonly Setting setting = Setting.GetSetting();

        public int Size { get => setting.FetchSize; set => setting.FetchSize = value; }
        private int offset = 0;
        public FetchingOptions options;

        public Fetching(FetchingOptions options = FetchingOptions.GET)
        {
            this.options = options;
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
                if (options == FetchingOptions.GET)
                {
                    requests.Add(client.GetAsync(Urls[i]));
                }
                else
                {
                    var content = new FormUrlEncodedContent(Values[i]);
                    requests.Add(client.PostAsync(Urls[i], content));
                }
            }
            offset += Size;
        }

        public async Task<List<string>> FetchURLs()
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

                QueueRequest();
            }

            return lst;
        }

        static async Task<List<string>> GetJisho(IEnumerable<string> strings)
        {
            var fetcher = new Fetching();
            foreach (var str in strings)
            {
                fetcher.Urls.Add($"https://jisho.org/search/{str}%20%23kanji");
            }
            return await fetcher.FetchURLs();
        }

        static async Task<List<string>> GetMazii(IEnumerable<string> strings)
        {
            var fetcher = new Fetching(options: FetchingOptions.POST);
            foreach (var str in strings)
            {
                fetcher.Urls.Add($"https://mazii.net/api/search");
                var dict = new Dictionary<string, string>();
                dict.Add("dict", "javi");
                dict.Add("page", "1");
                dict.Add("query", $"{str}");
                dict.Add("type", "kanji");
                fetcher.Values.Add(dict);
            }
            return await fetcher.FetchURLs();
        }

        static async public Task<Dictionary<string, string[]>> FetchAll(IEnumerable<string> strings)
        {
            var JishoTask = GetJisho(strings);
            var MaziiTask = GetMazii(strings);

            var Jisho = await JishoTask;
            var Mazii = await MaziiTask;

            var dict = strings.Select((chr, index) => new { chr, index })
                .ToDictionary(pair => pair.chr, pair => new string[2] { Jisho[pair.index], Mazii[pair.index] });

            return dict;
        }

    }
}
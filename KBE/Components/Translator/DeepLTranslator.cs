using DocumentFormat.OpenXml.Vml;
using KBE.Components.Settings;
using KBE.Components.Translator;
using KBE.Components.Utils;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace KBE.Components.Translator
{
    public partial class DeepLTranslator
    {
        public IPlaywright playwright = null;
        public IBrowser browser = null;
        public IPage page = null;

        ~DeepLTranslator()
        {
            if (page != null) {
                page.CloseAsync();
            }
        }

        public async Task<string> GetTranslation(string fromCode, string toCode, string value, IProgress<int>? progress = null)
        {
            if (string.IsNullOrWhiteSpace(value)) {
                return "";
            }

            if (value.Trim() == "\n")
            {
                return "\n";
            }

            if (browser == null)
            {
                playwright = await Playwright.CreateAsync();
                browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");
                var contexts = browser.Contexts;
                var pages = contexts[0].Pages;
                page = pages[pages.Count-1];
            }


            var newValue = EscapeLine().Replace(value, "%0A");
            var chunks = newValue.ToChunks(Setting.Instance.TranslateChunkSize);
            string combineString = string.Empty;

            var total = 0;
            var totalChunk = chunks.Count();
            foreach (var chunk in chunks)
            {
                string url = $"https://www.deepl.com/en/translator#{fromCode}/{toCode}/{chunk}";
                combineString += await GetTranslationByChunk(page, url).ConfigureAwait(false);
                
                total++;
                progress?.Report(total / totalChunk * 100);
            }

            return combineString;
        }

        private static async Task<string> GetTranslationByChunk(IPage page, string url)
        {
            await page.GotoAsync(url);
            //await page.GotoAsync("https://www.deepl.com/en/translator#ja/en/%E8%B2%B4%E6%96%B9%E3%81%AF%E3%81%AA%E3%82%93%E3%81%AE%E3%81%9F%E3%82%81%E3%81%AB%E3%82%B2%E3%83%BC%E3%83%A0%E3%82%92%E3%81%97%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F%0A%0A%E3%80%80%E4%B8%96%E7%95%8C%E4%B8%AD%E3%81%AE%E7%B2%BE%E9%9C%8A%E9%81%94%E3%82%92%E6%8D%95%E3%82%89%E3%81%88%E3%81%A6%E3%81%84%E3%81%9F%E9%82%AA%E7%A5%9E%E3%82%B0%E3%83%A9%E3%83%88%E3%83%BC%E3%83%8B%E3%82%A8%E3%81%8C%E6%96%83%E3%82%8C%E3%82%8B%E3%80%82%0A%0A%E3%81%9D%E3%81%AE%E5%B7%A8%E8%BA%AF%E3%81%AE%E5%86%85%E5%81%B4%E3%81%8B%E3%82%89%E7%88%86%E3%81%9C%E3%82%8B%E3%82%88%E3%81%86%E3%81%AB%E3%81%97%E3%81%A6%E8%89%B2%E5%8F%96%E3%82%8A%E5%8F%96%E3%82%8A%E3%81%AE%E5%85%89%E3%81%8C%E4%B8%96%E7%95%8C%E4%B8%AD%E3%81%AB%E6%95%A3%E3%82%89%E3%81%B0%E3%81%A3%E3%81%A6%E3%81%84%E3%81%8F%E3%80%82%E3%81%9D%E3%81%AE%E6%A7%98%E5%AD%90%E3%82%92%E6%B6%99%E3%82%92%E6%B5%81%E3%81%97%E3%81%AA%E3%81%8C%E3%82%89%E8%A6%8B%E3%81%A4%E3%82%81%E3%81%A6%E3%81%84%E3%81%9F%E5%B0%91%E5%A5%B3%E3%81%8C%E5%96%9C%E3%81%B3%E3%81%AE%E6%B6%99%E3%81%A8%E7%AC%91%E9%A1%94%E3%81%A7%E3%81%93%E3%81%A1%E3%82%89%E3%81%B8%E3%81%A8%E6%8C%AF%E3%82%8A%E5%90%91%E3%81%8F%E3%80%82%0A%0A");
            var textAreas = page.Locator("d-textarea");
            while (true)
            {
                var texts = await textAreas.AllInnerTextsAsync();
                if (texts.Count != 2)
                {
                    throw new Exception("[ERROR] Huh???");
                }

                if (texts[1] != "\n")
                {
                    return texts[1];
                }
                await Task.Delay(200);
            }


        }

        [GeneratedRegex("[\\r\\n]+")]
        private static partial Regex EscapeLine();
    }
}

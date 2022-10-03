using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using HtmlAgilityPack;
using KBE.Models;
using System.Diagnostics;
//List<string> list = new () { "光" };
//var s = await KBE.WordController.FetchAll(list);
//int a = 0;

//string tmp;

//using (var sr = new StreamReader("..\\..\\..\\Test.txt"))
//{
//    // Read the stream as a string, and write the string to the console.
//    tmp = sr.ReadToEnd();
//}
//var a = JsonSerializer.Deserialize<KBE.MaziiAPI>(tmp);
////var d = JsonSerializer.Deserialize<KBE.MaziiAPIResults>(a.results[0].ToString());
namespace KBE.Components
{
    //enum KanjiDict
    //{
    //    KANJI,
    //    MEANING,
    //    ON,
    //    KUN,
    //    LEVEL,
    //    ENGLISH,
    //    VIETNAMESE,
    //    STROKES,
    //    PARTS,
    //    TAUGHT
    //}

    public class MaziiAPI
    {
        public int status { get; set; } = 400;
        public MaziiAPIResults[] results { get; set; } = new MaziiAPIResults[0];
    }

    public class MaziiAPIResults
    {
        public string comp { get; set; } = "";
        public string level { get; set; } = "";
        public string kun { get; set; } = "";
        public string on { get; set; } = "";
        public string mean { get; set; } = "";
        public string detail { get; set; } = "";
    }

    class JishoAPI
    {
        static HtmlDocument doc = new();

        public string strokes { get; set; } = "";
        public string english { get; set; } = "";
        public string taught { get; set; } = "";
        public string jlpt { get; set; } = "";
        public string radicals { get; set; } = "";

        public JishoAPI(string htmlContent)
        {
            doc.LoadHtml(htmlContent);
            strokes = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[1]/div/div[2]/div[1]")
                .InnerText.ToString().Replace("\n", "").Trim();
            english = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[1]/div[1]")
                .InnerText.ToString().Replace("\n", "").Trim();
            taught = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[2]/div/div[1]")
                .InnerText.ToString().Replace("\n", "").Trim();
            try
            {
                jlpt = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[2]/div/div[2]")
    .InnerText.ToString().Replace("\n", "").Trim();
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex);
                jlpt = "0";
            }

            radicals = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[1]/div/div[2]/div[2]/dl/dd/span")
                .InnerText.ToString().Replace("\n", "").Trim();

        }
    }

    public class KanjiController
    {
        #region Init
        public static Setting Setting { get; } = Setting.GetSetting();
        #endregion

        #region Get Kanji

        //#region Preprocessor
        //private static List<string> SearchTermToList(string searchWords)
        //{
        //    List<string> list = new();

        //}
        //#endregion

        //private static async Task<SortedSet<KanjiWord>> GetKanjiFromDatabase(IEnumerable<string> strings)
        //{

        //    SortedSet<KanjiWord> result = new(new KanjiWordComparer());
        //}


        public static async Task<SortedSet<KanjiWord>> GetKanjiFromDatabaseAsync()
        {
            return await SQLController.GetAllKanjiWordsAsync();
        }

        public static async Task<bool> GetKanjiNotInDatabaseFromInternet(string rawString)
        {
            List<string> filteredString = KanjiProcessor.FilterProcessing(rawString, new() { isOnlyKanj = true });
            return await GetKanjiNotInDatabaseFromInternet(filteredString);
            
        }

        public static async Task<bool> GetKanjiNotInDatabaseFromInternet(List<string> filteredKanji)
        {
            var checkTasks = filteredKanji.ToList().Select(kanji => SQLController.CheckExistAsync(kanji));
            var isInDatabase = await Task.WhenAll(checkTasks);
            List<string> kanjiStrings = new();

            for (int i = 0; i < filteredKanji.Count; i++)
            {
                if (isInDatabase[i] == false)
                {
                    kanjiStrings.Add(filteredKanji[i]);
                }
            }

            if (kanjiStrings.Count == 0)
            {
                return false;
            }

            var result = await GetKanjiFromInternet(kanjiStrings);
            await AddToDatabase(result);
            return true;
        }

        public static async Task AddToDatabase(SortedSet<KanjiWord> kanjis)
        {
            await SQLController.AddMultipleKanjisAsync(kanjis);
        }

        public static void UpdateKanji(KanjiWord kanji)
        {
            Task.Run(()=>SQLController.Update(kanji));
        }
        


        private static async Task<SortedSet<KanjiWord>> GetKanjiFromInternet(List<string> strings)
        {

            var rawWord = await Fetching.FetchAll(strings);
            SortedSet<KanjiWord> kanjiList = new(new KanjiWordComparer());
            foreach (var (word, rawContent) in rawWord)
            {
                kanjiList.Add(ProcessWord(word, rawContent));
            }
            return kanjiList;
        }
        #endregion

        #region Processing
        public static KanjiWord ProcessWord(string word, string[] wordDict)
        {
            JishoAPI jishoAPI = new(wordDict[0]);
            var maziiApiResults = ProcessMazii(wordDict[1]);
            KanjiWord kanji = new(kanji: word, sinoVietnamese: maziiApiResults.mean, on: maziiApiResults.on, kun: maziiApiResults.kun,
                level: maziiApiResults.level, radicals: jishoAPI.radicals, english: jishoAPI.english, vietnamese: maziiApiResults.detail,
                strokes: jishoAPI.strokes, parts: maziiApiResults.comp, taught: jishoAPI.taught);
            return kanji;
        }

        public static MaziiAPIResults ProcessMazii(string maziiRaw)
        {
            var maziiObj = JsonSerializer.Deserialize<MaziiAPI>(maziiRaw);
            if (maziiObj is null)
            {
                throw new Exception("Parse Error");
            }
            return maziiObj.results[0];
        }
        #endregion

        //public static List<KanjiWord> GetAllFromDatabase()
        //{

        //}


        //public static Dictionary<KanjiDict, string> ProcessJisho(string jishoRaw)
        //{
        //    doc.Load(jishoRaw);
        //    var strokes = doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[1]/div/div[2]/div[1]").ToString;
        //}
    }
}

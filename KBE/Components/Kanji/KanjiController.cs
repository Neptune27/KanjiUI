﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using KBE.Models;
using System.Diagnostics;
using KBE.Components.SQL;
using KBE.Components.Utils;
using KBE.Components.Settings;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Reflection;
using KBE.Components.Kanji.Jisho;
using KBE.Components.Kanji.Mazii;
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
namespace KBE.Components.Kanji
{

    public class KanjiController
    {
        #region Init
        public static Setting Setting => Setting.Instance;
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


        public static async Task<List<KanjiWord>> GetKanjiFromDatabaseAsync()
        {
            return await SQLController.GetAllKanjiWordsAsync();
        }

        public static async Task<bool> GetKanjiNotInDatabaseFromInternet(string rawString, IProgress<int>? JishoProgress = null, IProgress<int>? MaziiProgress = null)
        {
            List<string> filteredString = KanjiProcessor.FilterProcessing(rawString, new() { IsOnlyKanji = true });
            return await GetKanjiNotInDatabaseFromInternet(filteredString, JishoProgress, MaziiProgress);

        }

        public static async Task<bool> GetKanjiNotInDatabaseFromInternet(List<string> filteredKanji, IProgress<int>? jishoProgress = null, IProgress<int>? maziiProgress = null)
        {
            var checkTasks = filteredKanji.ToList().Select(SQLController.CheckExistAsync);
            var isInDatabase = await Task.WhenAll(checkTasks).ConfigureAwait(false);
            List<string> kanjiStrings = filteredKanji.Where((t, i) => isInDatabase[i] == false).ToList();

            if (kanjiStrings.Count == 0)
            {
                return false;
            }

            var result = await GetKanjiFromInternet(kanjiStrings, jishoProgress, maziiProgress);
            await AddToDatabase(result);
            return true;
        }

        public static async Task AddToDatabase(List<KanjiWord> kanjis)
        {
            await SQLController.AddMultipleKanjisAsync(kanjis);
        }

        public static void UpdateKanji(KanjiWord kanji)
        {
            Task.Run(() => SQLController.Update(kanji));
        }



        public static async Task<List<KanjiWord>> GetKanjiFromInternet(List<string> strings, IProgress<int>? jishoProgress = null, IProgress<int>? maziiProgress = null)
        {

            var rawWord = await Fetching.FetchAll(strings, jishoProgress, maziiProgress);
            var kanjiList = rawWord.Select((kVP) => ProcessWord(kVP.Key, kVP.Value));
            return kanjiList.ToList();
        }
        #endregion

        #region Processing
        public static KanjiWord ProcessWord(string word, string[] wordDict)
        {
            JishoAPI jishoAPI = new(wordDict[0]);
            var maziiApiResults = ProcessMazii(wordDict[1]);
            maziiApiResults.compDetail ??= [];
            var compDetail = maziiApiResults.compDetail;
            var compDetailString = string.Join(", ", compDetail.Select(i => $"{i.h}: {i.w}"));
            maziiApiResults.level ??= [];
            var level = string.Join(", ", maziiApiResults.level);
            KanjiWord kanji = new(kanji: word, sinoVietnamese: maziiApiResults.mean, on: maziiApiResults.on, kun: maziiApiResults.kun,
                level: level, radicals: jishoAPI.radicals, english: jishoAPI.english, vietnamese: maziiApiResults.detail,
                strokes: jishoAPI.strokes, parts: compDetailString, taught: jishoAPI.taught);
            return kanji;
        }

        private static MaziiAPI MaziiErrorTemplate()
        {
            return new()
            {
                status = 404,
                results = [new() { mean = "N/A" }]
            };
        }

        public static MaziiAPIResults ProcessMazii(string maziiRaw)
        {
            try
            {
                var maziiObj = JsonSerializer.Deserialize(maziiRaw, MaziiJsonContext.Default.MaziiAPI) ?? throw new Exception($"{maziiRaw} not found");
                if (maziiObj.status != 200)
                {
                    return MaziiErrorTemplate().results[0];
                }
                if (maziiObj.results.Length == 0)
                {
                    return MaziiErrorTemplate().results[0];
                }

                return maziiObj switch
                {
                    null => throw new Exception("Parse Error"),
                    _ => maziiObj.results[0]
                };
            }
            catch (Exception ex)
            {

                Setting.Logger.Error(ex.Message);
                throw;
            }
        }
        #endregion
    }
}

using KBE.Models;
using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBE.Components.Kanji;
using KBE.Components.Settings;

namespace KBE.Components.SQL
{


    public class SQLController
    {
        #region Field
        static Setting Setting => Setting.Instance;
        static readonly SqliteConnection _connection = new($"Data Source={Setting.DatabaseConnectDirectory}");
        static readonly SQLController _controller = new();
        #endregion

        #region Constructor/Singleton

        SQLController()
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS KANJIWORD (
                    KANJI TEXT PRIMARY KEY UNIQUE,
                    SINOVIETNAMESE TEXT,
                    ONYUMI TEXT,
                    KUNYUMI TEXT,
                    LEVEL TEXT,
                    ENGLISH TEXT,
                    VIETNAMESE TEXT,
                    STROKES TEXT,
                    RADICAL TEXT,
                    PARTS TEXT,
                    TAUGHT TEXT)
            ";
            command.ExecuteNonQuery();
        }

        static public SQLController GetController()
        {
            return _controller;
        }
        #endregion

        #region DEBUG
#if DEBUG
        public static async Task AddTmpWord()
        {
            using (var transaction = await _connection.BeginTransactionAsync().ConfigureAwait(false))
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                @"
                    INSERT INTO KANJIWORD
                    VALUES ($KANJI, $SINOVIETNAMESE, $ONYUMI, $KUNYUMI, $LEVEL, $ENGLISH, $VIETNAMESE, $STROKES, $RADICAL, $PARTS, $TAUGHT)
                ";

                var pKanji = command.CreateParameter();
                pKanji.ParameterName = "$KANJI";
                pKanji.Value = "SD";
                command.Parameters.Add(pKanji);


                var pSino = command.CreateParameter();
                pSino.ParameterName = "$SINOVIETNAMESE";
                pSino.Value = "S";
                command.Parameters.Add(pSino);


                var pOn = command.CreateParameter();
                pOn.ParameterName = "$ONYUMI";
                pOn.Value = "O";
                command.Parameters.Add(pOn);


                var pKun = command.CreateParameter();
                pKun.ParameterName = "$KUNYUMI";
                pKun.Value = "K";
                command.Parameters.Add(pKun);


                var pLevel = command.CreateParameter();
                pLevel.ParameterName = "$LEVEL";
                pLevel.Value = "L";
                command.Parameters.Add(pLevel);

                var pEnglish = command.CreateParameter();
                pEnglish.ParameterName = "$ENGLISH";
                pEnglish.Value = "E";
                command.Parameters.Add(pEnglish);

                var pVietnamese = command.CreateParameter();
                pVietnamese.ParameterName = "$VIETNAMESE";
                pVietnamese.Value = "V";
                command.Parameters.Add(pVietnamese);

                var pStrokes = command.CreateParameter();
                pStrokes.ParameterName = "$STROKES";
                pStrokes.Value = "S";
                command.Parameters.Add(pStrokes);

                var pRadical = command.CreateParameter();
                pRadical.ParameterName = "$RADICAL";
                pRadical.Value = "R";
                command.Parameters.Add(pRadical);

                var pParts = command.CreateParameter();
                pParts.ParameterName = "$PARTS";
                pParts.Value = "P";
                command.Parameters.Add(pParts);

                var pTaught = command.CreateParameter();
                pTaught.ParameterName = "$TAUGHT";
                pTaught.Value = "T";
                command.Parameters.Add(pTaught);


                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
#endif

        #endregion

        #region Check From Database

        public static async Task<bool> CheckExistAsync(string kanji)
        {
            return await CheckExistAsync(new KanjiWord() { Kanji = kanji });
        }

        public static async Task<bool> CheckExistAsync(KanjiWord kanji)
        {
            var command = _connection.CreateCommand();
            command.CommandText = @$"SELECT KANJI FROM KANJIWORD WHERE KANJI = '{kanji.Kanji}'";
            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                var isExist = new object[1];

                while (reader.Read())
                {
                    var name = reader.GetValues(isExist);
                }
                return isExist[0] is not null;
            }
        }

        #endregion

        #region Add To Database
        public static async Task AddKanjiAsync(KanjiWord kanji)
        {
            using (var transaction = await _connection.BeginTransactionAsync().ConfigureAwait(false))
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                @"
                    INSERT INTO KANJIWORD
                    VALUES ($KANJI, $SINOVIETNAMESE, $ONYUMI, $KUNYUMI, $LEVEL, $ENGLISH, $VIETNAMESE, $STROKES, $RADICAL, $PARTS, $TAUGHT)
                ";

                var pKanji = command.CreateParameter();
                pKanji.ParameterName = "$KANJI";
                kanji.Kanji = kanji.Kanji is null ? "" : kanji.Kanji;
                pKanji.Value = kanji.Kanji;
                command.Parameters.Add(pKanji);


                var pSino = command.CreateParameter();
                pSino.ParameterName = "$SINOVIETNAMESE";

                kanji.SinoVietnamese = kanji.SinoVietnamese is null ? "" : kanji.SinoVietnamese;

                pSino.Value = kanji.SinoVietnamese;
                command.Parameters.Add(pSino);


                var pOn = command.CreateParameter();
                pOn.ParameterName = "$ONYUMI";
                kanji.Onyumi = kanji.Onyumi is null ? "" : kanji.Onyumi;

                pOn.Value = kanji.Onyumi;
                command.Parameters.Add(pOn);


                var pKun = command.CreateParameter();
                pKun.ParameterName = "$KUNYUMI";
                kanji.Kunyumi = kanji.Kunyumi is null ? "" : kanji.Kunyumi;

                pKun.Value = kanji.Kunyumi;
                command.Parameters.Add(pKun);


                var pLevel = command.CreateParameter();
                pLevel.ParameterName = "$LEVEL";
                kanji.Level = kanji.Level is null ? "" : kanji.Level;

                pLevel.Value = kanji.Level;
                command.Parameters.Add(pLevel);

                var pEnglish = command.CreateParameter();
                pEnglish.ParameterName = "$ENGLISH";
                kanji.English = kanji.English is null ? "" : kanji.English;

                pEnglish.Value = kanji.English;
                command.Parameters.Add(pEnglish);

                var pVietnamese = command.CreateParameter();
                pVietnamese.ParameterName = "$VIETNAMESE";
                kanji.Vietnamese = kanji.Vietnamese is null ? "" : kanji.Vietnamese;

                pVietnamese.Value = kanji.Vietnamese;
                command.Parameters.Add(pVietnamese);

                var pStrokes = command.CreateParameter();
                pStrokes.ParameterName = "$STROKES";
                kanji.Strokes = kanji.Strokes is null ? "" : kanji.Strokes;

                pStrokes.Value = kanji.Strokes;
                command.Parameters.Add(pStrokes);

                var pRadical = command.CreateParameter();
                pRadical.ParameterName = "$RADICAL";
                kanji.Radicals = kanji.Radicals is null ? "" : kanji.Radicals;

                pRadical.Value = kanji.Radicals;
                command.Parameters.Add(pRadical);

                var pParts = command.CreateParameter();
                pParts.ParameterName = "$PARTS";
                kanji.Parts = kanji.Parts is null ? "" : kanji.Parts;

                pParts.Value = kanji.Parts;
                command.Parameters.Add(pParts);

                var pTaught = command.CreateParameter();
                pTaught.ParameterName = "$TAUGHT";
                kanji.Taught = kanji.Taught is null ? "" : kanji.Taught;

                pTaught.Value = kanji.Taught;
                command.Parameters.Add(pTaught);

                try
                {
                    command.ExecuteNonQuery();
                    await transaction.CommitAsync();
                }
                catch (SqliteException sqlEx)
                {
                    Console.WriteLine(sqlEx.Message);
                }
            }
        }

        public static async Task AddMultipleKanjisAsync(SortedSet<KanjiWord> kanjis)
        {
            var tasks = kanjis.ToList().Select(kanji => AddKanjiAsync(kanji));
            await Task.WhenAll(tasks);
        }

        #endregion

        #region Delete From Database
        public static async Task DeleteKanjiAsync(KanjiWord kanji)
        {
            using (var transaction = await _connection.BeginTransactionAsync().ConfigureAwait(false))
            {
                var command = _connection.CreateCommand();
                command.CommandText = @"DELETE FROM KANJIWORD WHERE KANJI=$KANJI";
                var pKanji = command.CreateParameter();
                pKanji.ParameterName = "$KANJI";
                pKanji.Value = kanji.Kanji;
                command.Parameters.Add(pKanji);
                try
                {
                    command.ExecuteNonQuery();
                    await transaction.CommitAsync();
                }
                catch (SqliteException sqlEx)
                {
                    Console.WriteLine(sqlEx.Message);
                }
            }
        }
        #endregion

        #region Get From Database

        public static async Task<SortedSet<KanjiWord>> GetAllKanjiWordsAsync()
        {

            foreach (var item in Setting.SearchOptions.GetType().GetProperties())
            {
                Console.WriteLine(item.GetValue(Setting.SearchOptions));
                Console.WriteLine(item);
            }

            var command = _connection.CreateCommand();
            command.CommandText = @"
                    SELECT * FROM KANJIWORD
                    ";

            SortedSet<KanjiWord> kanjis = new(new KanjiWordComparer());
            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                var obj = new string[11];

                while (reader.Read())
                {
                    var name = reader.GetValues(obj);
                    kanjis.Add(KanjiWord.ToKanjiWord(obj));
                }
            }
            return kanjis;
        }

        private static async Task<SortedSet<KanjiWord>> SearchKanjiAsync(string word, string options)
        {

            var command = _connection.CreateCommand();
            var text = @$"
                    SELECT * FROM KANJIWORD WHERE {options} LIKE '%{word}%'
                    ";

            command.CommandText = text;
            SortedSet<KanjiWord> kanjis = new(new KanjiWordComparer());
            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                var obj = new string[11];

                while (reader.Read())
                {
                    var name = reader.GetValues(obj);
                    kanjis.Add(KanjiWord.ToKanjiWord(obj));
                }
            }
            return kanjis;
        }

        public static async Task<SortedSet<KanjiWord>> SearchKanjiAsync(string word)
        {
            var SQLSetting = Setting.SearchOptions;
            SortedSet<KanjiWord> kanjis = new(new KanjiWordComparer());
            foreach (var property in SQLSetting.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(SQLSetting);
                if (propertyValue is bool boolean)
                {
                    if (boolean == true)
                    {
                        kanjis.UnionWith(await SearchKanjiAsync(word, property.Name.ToUpper()));
                    }
                }
            }
            return kanjis;
        }

        public static async Task<SortedSet<KanjiWord>> GetMultipleKanjiAsync(IEnumerable<string> stringsList)
        {
            SortedSet<KanjiWord> kanjiWords = new(new KanjiWordComparer());
            List<Task<SortedSet<KanjiWord>>> tasks = new();
            foreach (var strings in stringsList)
            {
                tasks.Add(SearchKanjiAsync(strings));
            }
            await Task.WhenAll(tasks);
            var results = tasks.Select(task => task.Result);
            foreach (var result in results)
            {
                kanjiWords.UnionWith(result);
            }
            return kanjiWords;
        }

        #endregion

        #region Update Database
        public static async Task Update(KanjiWord kanji)
        {
            using (var transaction = await _connection.BeginTransactionAsync().ConfigureAwait(false))
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                @"
                    UPDATE KANJIWORD SET SINOVIETNAMESE=$SINOVIETNAMESE, 
                        ONYUMI = $ONYUMI, KUNYUMI = $KUNYUMI, LEVEL = $LEVEL, ENGLISH = $ENGLISH, 
                        VIETNAMESE = $VIETNAMESE, STROKES = $STROKES, RADICAL = $RADICAL, PARTS = $PARTS, 
                        TAUGHT = $TAUGHT WHERE KANJI = $KANJI
                ";

                var pKanji = command.CreateParameter();
                pKanji.ParameterName = "$KANJI";
                kanji.Kanji = kanji.Kanji is null ? "" : kanji.Kanji;
                pKanji.Value = kanji.Kanji;
                command.Parameters.Add(pKanji);


                var pSino = command.CreateParameter();
                pSino.ParameterName = "$SINOVIETNAMESE";

                kanji.SinoVietnamese = kanji.SinoVietnamese is null ? "" : kanji.SinoVietnamese;

                pSino.Value = kanji.SinoVietnamese;
                command.Parameters.Add(pSino);


                var pOn = command.CreateParameter();
                pOn.ParameterName = "$ONYUMI";
                kanji.Onyumi = kanji.Onyumi is null ? "" : kanji.Onyumi;

                pOn.Value = kanji.Onyumi;
                command.Parameters.Add(pOn);


                var pKun = command.CreateParameter();
                pKun.ParameterName = "$KUNYUMI";
                kanji.Kunyumi = kanji.Kunyumi is null ? "" : kanji.Kunyumi;

                pKun.Value = kanji.Kunyumi;
                command.Parameters.Add(pKun);


                var pLevel = command.CreateParameter();
                pLevel.ParameterName = "$LEVEL";
                kanji.Level = kanji.Level is null ? "" : kanji.Level;

                pLevel.Value = kanji.Level;
                command.Parameters.Add(pLevel);

                var pEnglish = command.CreateParameter();
                pEnglish.ParameterName = "$ENGLISH";
                kanji.English = kanji.English is null ? "" : kanji.English;

                pEnglish.Value = kanji.English;
                command.Parameters.Add(pEnglish);

                var pVietnamese = command.CreateParameter();
                pVietnamese.ParameterName = "$VIETNAMESE";
                kanji.Vietnamese = kanji.Vietnamese is null ? "" : kanji.Vietnamese;

                pVietnamese.Value = kanji.Vietnamese;
                command.Parameters.Add(pVietnamese);

                var pStrokes = command.CreateParameter();
                pStrokes.ParameterName = "$STROKES";
                kanji.Strokes = kanji.Strokes is null ? "" : kanji.Strokes;

                pStrokes.Value = kanji.Strokes;
                command.Parameters.Add(pStrokes);

                var pRadical = command.CreateParameter();
                pRadical.ParameterName = "$RADICAL";
                kanji.Radicals = kanji.Radicals is null ? "" : kanji.Radicals;

                pRadical.Value = kanji.Radicals;
                command.Parameters.Add(pRadical);

                var pParts = command.CreateParameter();
                pParts.ParameterName = "$PARTS";
                kanji.Parts = kanji.Parts is null ? "" : kanji.Parts;

                pParts.Value = kanji.Parts;
                command.Parameters.Add(pParts);

                var pTaught = command.CreateParameter();
                pTaught.ParameterName = "$TAUGHT";
                kanji.Taught = kanji.Taught is null ? "" : kanji.Taught;

                pTaught.Value = kanji.Taught;
                command.Parameters.Add(pTaught);

                try
                {
                    command.ExecuteNonQuery();
                    await transaction.CommitAsync();
                }
                catch (SqliteException sqlEx)
                {
                    Debug.WriteLine(sqlEx.Message);
                }
            }
        }

        #endregion
    }
}

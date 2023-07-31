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
using Microsoft.EntityFrameworkCore;

namespace KBE.Components.SQL
{


    public class SQLController
    {
        #region Field
        static Setting Setting => Setting.Instance;
        private static readonly SqliteConnection _connection = new($"Data Source={Setting.DatabaseConnectDirectory}");
        #endregion

        #region Check From Database

        public static async Task<bool> CheckExistAsync(string kanji)
        {
            return await CheckExistAsync(new KanjiWord() { Kanji = kanji });
        }

        public static async Task<bool> CheckExistAsync(KanjiWord kanji)
        {

            using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);
            return await db.KANJIWORD.AnyAsync(k => k.Kanji == kanji.Kanji).ConfigureAwait(false);
        }

        #endregion

        #region Add To Database
        public static async Task AddKanjiAsync(KanjiWord kanji)
        {
            await AddMultipleKanjisAsync(new() { kanji });
        }

        public static async Task AddMultipleKanjisAsync(List<KanjiWord> kanjis)
        {
            try
            {
                using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);
                var addTasks = kanjis.Select(async (k) =>
                {
                    return await db.AddAsync(k); 
                });
                await Task.WhenAll(addTasks).ConfigureAwait(false);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"[INFO]: {ex.Message}");
            }

        }

        #endregion

        #region Delete From Database
        public static async Task DeleteKanjiAsync(KanjiWord kanji)
        {
            await Task.Run(() =>
            {
                using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);
                db.Remove(kanji);
                db.SaveChanges();
            }).ConfigureAwait(false);
        }
        #endregion


        #region Create Database if not exist

        public static Task CheckDatabaseExist()
        {
            using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);
            return db.Database.EnsureCreatedAsync();
        }

        #endregion

        #region Get From Database


        //TODO: Reimplement this.
        public static async Task<List<KanjiWord>> GetAllKanjiWordsAsync()
        {

            await CheckDatabaseExist().ConfigureAwait(false);

            using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);

            var items = await db.KANJIWORD.AsNoTracking().OrderBy(i => i.Kanji).ToListAsync().ConfigureAwait(false);

            return items;
        }
        #endregion

        #region Update Database
        public static async Task Update(KanjiWord kanji)
        {

            using var db = new KanjiWordContext(Setting.DatabaseConnectDirectory);

            var trackedKanji = await db.KANJIWORD.FindAsync(kanji.Kanji).ConfigureAwait(false);

            if (trackedKanji is null) {
                return;
            }

            trackedKanji.UpdateFrom(kanji);
            await db.SaveChangesAsync();
        }

        #endregion
    }
}

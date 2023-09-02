using KBE.Enums;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KBE.Components.Settings
{

    public enum ESaveAsType
    {
        TEXT,
        DOCX
    }

    public enum ERandoSaveOption
    {
        ALL,
        WRONG_ONLY,
        RIGHT_ONLY
    }


    public class SearchOptions
    {
        public bool Kanji { get; set; } = true;
        public bool SinoVietnamese { get; set; } = true;
        public bool Onyumi { get; set; } = true;
        public bool Kunyumi { get; set; } = true;
        public bool English { get; set; } = true;
        public bool Vietnamese { get; set; } = false;
        public bool Strokes { get; set; } = true;
        public bool Radicals { get; set; } = false;
        public bool Level { get; set; } = false;
    }

    public class SaveOptions : SearchOptions
    {
        public bool Color { get; set; } = true;
    }

    public class Setting
    {

#if !DEBUG
        public static string Directory = ".";
        public static string FilePath = $"{Directory}\\Setting\\setting.json";
#elif DEBUG
        public static string Directory { get; set; } = "..\\..\\..\\..";
        public static string FilePath { get; set; } = $"{Directory}\\Setting\\setting.json";
#endif
        public static JsonSerializerOptions SaveOptions { get; set; } = new() { WriteIndented = true };

        public int FetchSize { get; set; }
        public string DatabaseConnectDirectory { get; set; } = "";
        public SearchOptions SearchOptions { get; set; } = new();
        public SaveOptions SaveOption { get; set; } = new();
        public string Filter { get; set; } = "";


        public string TranslateFromCodeName { get; set; } = "";
        public string TranslateToCodeName { get; set; } = "";
        public bool ShowCursorKanji { get; set; } = true;
        public int SearchDelayInMs { get; set; } = 100;


        public EKanjiShowingType QuestionType { get; set; } = EKanjiShowingType.Kanji;
        public EKanjiShowingType AnswerType { get; set; } = EKanjiShowingType.English;
        public bool MoveNextAfterSelection { get; set; } = true;
        public int TotalRandomLength { get; set; } = 0;
        public RandoSaveModel RandoSave { get; set; } = new();
        public RandoAutoSaveModel RandoAutoSave { get; set; } = new();

        public int TranslateChunkSize { get; set; }

        public bool LossySearch { get; set; } = true;

        public static Setting Instance { get; private set; } = MakeSetting();

        public static Setting GetSetting()
        {
            return Instance;
        }

        static string ReadFromFile()
        {
            string output = "";
            try
            {
                using (var sr = new StreamReader(FilePath))
                {
                    // Read the stream as a string, and write the string to the console.
                    output = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return output;
        }

        static Setting MakeSetting()
        {
            try
            {
                Setting? setting = JsonSerializer.Deserialize<Setting>(ReadFromFile(), SettingJsonContext.Default.Setting);

                return setting ?? throw new NullReferenceException();
            }
            catch (Exception e) when (e is JsonException || e is NullReferenceException)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);


                ResetToAndSaveDefaultSetting();
                return MakeSetting();
            }

        }

        public delegate void OnDatabaseChangedHandler();


        public event OnDatabaseChangedHandler? OnDatabaseChanged;

        public void RaisedOnDatabaseChanged()
        {
            OnDatabaseChanged?.Invoke();
        }


        public static void ResetToAndSaveDefaultSetting()
        {
            var defaultSetting = new Setting
            {
                FetchSize = 100,
                DatabaseConnectDirectory = $"{Directory}\\Data\\DefaultDatabase.db",
                SearchOptions = new SearchOptions(),
                SaveOption = new(),
                LossySearch = true,
                Filter = "◇『』　、。」「～←…・々",
                TranslateFromCodeName = "Japanese",
                TranslateToCodeName = "English",
                QuestionType = EKanjiShowingType.Kanji,
                AnswerType = EKanjiShowingType.English,
                TranslateChunkSize = 1500,
                MoveNextAfterSelection = true,
                TotalRandomLength = 0,
                ShowCursorKanji = true,
                SearchDelayInMs = 100,
                RandoSave = new() { SaveAsType = ESaveAsType.TEXT, SaveOption = ERandoSaveOption.ALL },
                RandoAutoSave = new() { SaveAsType = ESaveAsType.TEXT, SaveOption = ERandoSaveOption.ALL, IsEnable = true, SaveLocation = $"{Directory}\\Data\\Autosave" }
            };

            System.IO.Directory.CreateDirectory($"{Directory}\\Setting");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data\\Img");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data\\Autosave");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data\\Template");

            using (StreamWriter sw = new($"{FilePath}"))
            {
                var str = JsonSerializer.Serialize(defaultSetting, SaveOptions);
                sw.Write(str);
            }
            Instance = defaultSetting;
        }

        public void SaveSetting()
        {
            using StreamWriter sw = new($"{FilePath}");
            Debug.WriteLine(JsonSerializer.Serialize(this, SaveOptions));
            sw.Write(JsonSerializer.Serialize(this, SaveOptions));
        }

    }
}


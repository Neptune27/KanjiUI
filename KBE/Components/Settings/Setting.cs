using KBE.Enums;
using KBE.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
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

		public static string LogPath { get; set; } = $"{Directory}\\Data\\log.txt";
		public static readonly Logger Logger = new LoggerConfiguration()
												.WriteTo.Debug()
												.WriteTo.File(LogPath)
												.CreateLogger();

		public static JsonSerializerOptions SaveOptions { get; set; } = new() { WriteIndented = true };

		public int FetchSize { get; set; }
		public string DatabaseConnectDirectory { get; set; } = "";
		public SearchOptions SearchOptions { get; set; } = new();
		public SaveOptions SaveOption { get; set; } = new();
		public string Filter { get; set; } = "";


		public string TranslateSource { get; set; } = "";

		public string GoogleTranslateFromCodeName { get; set; } = "";
		public string GoogleTranslateToCodeName { get; set; } = "";

		public string DeepLTranslateFromCodeName { get; set; } = "";
		public string DeepLTranslateToCodeName { get; set; } = "";

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

		public bool Furigana { get; set; } = false;

		public bool FuriganaHiragana { get; set; } = true;
		public bool FuriganaRomanji { get; set; } = true;
		public bool ConnectedRomanji { get; set; } = true;

		public bool WVDeveloperMode { get; set; } = false;
		public bool UnsafeJapaneseAnalyzer { get; set; } = true;

        public bool SortOrderByDescending { get; set; } = false;
		public EKanjiShowingType OrderByOption { get; set; } = EKanjiShowingType.Kanji;
		public EKanjiShowingType FirstEnglishOption { get; set; } = EKanjiShowingType.English;

        public bool GoToFirstItemWhenSubmitted { get; set; } = false;


        public List<CopyToExcel> CopyToExcelOptions { get; set; } = CreateCopyDefault();


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

        public delegate void OnSortingOrderChangedHandler();


        public event OnSortingOrderChangedHandler? OnSortingOrderChanged;

        public void RaisedOnSortingOrderChanged()
        {
            OnSortingOrderChanged?.Invoke();
        }



        private static List<CopyToExcel> CreateCopyDefault()
		{
			return [
					new CopyToExcel {
						IsEnable = true,
						Name = nameof(KanjiWord.Kanji),
					},
					new CopyToExcel {
						IsEnable = true,
						Name = nameof(KanjiWord.Kunyumi),
					},
					new CopyToExcel {
						IsEnable = true,
						Name = nameof(KanjiWord.Onyumi),
					},
					new CopyToExcel {
						IsEnable = true,
						Name = nameof(KanjiWord.English),
					},
					new CopyToExcel {
						IsEnable = true,
						Name = nameof(KanjiWord.SinoVietnamese),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Strokes),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Radicals),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Taught),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Parts),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Vietnamese),
					},
					new CopyToExcel {
						IsEnable = false,
						Name = nameof(KanjiWord.Level),
					},
				];
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
				Filter = "◇『』　、。」「～←…・々→〜",

				GoogleTranslateFromCodeName = "Japanese",
				GoogleTranslateToCodeName = "English",

				DeepLTranslateFromCodeName = "Japanese",
				DeepLTranslateToCodeName = "English",

				TranslateSource = "Google Translator",

				QuestionType = EKanjiShowingType.Kanji,
				AnswerType = EKanjiShowingType.English,
				TranslateChunkSize = 1500,
				MoveNextAfterSelection = true,
				TotalRandomLength = 0,
				ShowCursorKanji = true,
				SearchDelayInMs = 100,
				RandoSave = new() { SaveAsType = ESaveAsType.TEXT, SaveOption = ERandoSaveOption.ALL },
				RandoAutoSave = new() { SaveAsType = ESaveAsType.TEXT, SaveOption = ERandoSaveOption.ALL, IsEnable = true, SaveLocation = $"{Directory}\\Data\\Autosave" },

				Furigana = false,
				FuriganaHiragana = true,
				FuriganaRomanji = true,
				ConnectedRomanji = true,

				WVDeveloperMode = false,
				UnsafeJapaneseAnalyzer = true,
				CopyToExcelOptions = CreateCopyDefault(),

				OrderByOption = EKanjiShowingType.Kanji,
				SortOrderByDescending = false,

                FirstEnglishOption = EKanjiShowingType.English,
                GoToFirstItemWhenSubmitted = false,
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
			Logger.Information(JsonSerializer.Serialize(this, SaveOptions));
			sw.Write(JsonSerializer.Serialize(this, SaveOptions));
		}

	}
}


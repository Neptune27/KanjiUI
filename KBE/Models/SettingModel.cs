using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models
{
    public partial class SettingModel : ObservableObject
    {
        private static Setting Setting => Setting.Instance;

        public SettingModel()
        {
			CopyToExcelOptions.CollectionChanged += CopyToExcelOptions_CollectionChanged;
        }

		private void CopyToExcelOptions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Setting.CopyToExcelOptions = new(CopyToExcelOptions.Select(it => new CopyToExcel(it)));
		}

		public int FetchSize
        {
            get => Setting.FetchSize;
            set
            {
                OnPropertyChanging(nameof(FetchSize));

                Setting.FetchSize = value;

                OnPropertyChanged(nameof(FetchSize));

            }
        }



        public int TotalRandomLength
        {
            get => Setting.TotalRandomLength;
            set
            {
                OnPropertyChanging(nameof(TotalRandomLength));

                Setting.TotalRandomLength = value;

                OnPropertyChanged(nameof(TotalRandomLength));

            }
        }


        public int QuestionType
        {
            get => (int)Setting.QuestionType;
            set
            {
                OnPropertyChanging(nameof(QuestionType));

                Setting.QuestionType = (Enums.EKanjiShowingType)value;

                OnPropertyChanged(nameof(QuestionType));

            }
        }


        public int AnswerType
        {
            get => (int)Setting.AnswerType;
            set
            {
                OnPropertyChanging(nameof(AnswerType));

                Setting.AnswerType = (Enums.EKanjiShowingType)value;

                OnPropertyChanged(nameof(AnswerType));

            }
        }

        public string DatabaseConnectDirectory
        {
            get => Setting.DatabaseConnectDirectory;
            set {
                if (DatabaseConnectDirectory == value)
                {
                    return;
                }
                
                OnPropertyChanging(nameof(DatabaseConnectDirectory));

                Setting.DatabaseConnectDirectory = value;
                Setting.RaisedOnDatabaseChanged();

                OnPropertyChanged(nameof(DatabaseConnectDirectory));

            }
        }

        public string Filter
        {
            get => Setting.Filter;
            set
            {
                if (Filter == value)
                {
                    return;
                }

                OnPropertyChanging(nameof(Filter));

                Setting.Filter = value;

                OnPropertyChanged(nameof(Filter));

            }
        }

        #region Search Types
        public bool Kanji
        {
            get => Setting.SearchOptions.Kanji;
            set
            {
                if (Kanji == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kanji));
                Setting.SearchOptions.Kanji = value;
                OnPropertyChanged(nameof(Kanji));
            }
        }

        public bool SinoVietnamese
        {
            get => Setting.SearchOptions.SinoVietnamese;
            set
            {
                if (SinoVietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SinoVietnamese));
                Setting.SearchOptions.SinoVietnamese = value;
                OnPropertyChanged(nameof(SinoVietnamese));
            }
        }

        public bool Onyumi
        {
            get => Setting.SearchOptions.Onyumi;
            set
            {
                if (Onyumi == value)
                {
                    return;
                }

                OnPropertyChanging(nameof(Onyumi));
                Setting.SearchOptions.Onyumi = value;
                OnPropertyChanged(nameof(Onyumi));
            }
        }

        public bool Kunyumi
        {
            get => Setting.SearchOptions.Kunyumi;
            set
            {
                if (Kunyumi == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kunyumi));
                Setting.SearchOptions.Kunyumi = value;
                OnPropertyChanged(nameof(Kunyumi));

            }
        }

        public bool English
        {
            get => Setting.SearchOptions.English;
            set
            {
                if (English == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(English));
                Setting.SearchOptions.English = value;
                OnPropertyChanged(nameof(English));

            }
        }

        public bool Vietnamese
        {
            get => Setting.SearchOptions.Vietnamese;
            set
            {
                if (Vietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Vietnamese));
                Setting.SearchOptions.Vietnamese = value;
                OnPropertyChanged(nameof(Vietnamese));

            }
        }

        public bool Strokes
        {
            get => Setting.SearchOptions.Strokes;
            set
            {
                if (Strokes == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Strokes));
                Setting.SearchOptions.Strokes = value;
                OnPropertyChanged(nameof(Strokes));
            }
        }

        public bool Radicals
        {
            get => Setting.SearchOptions.Radicals;
            set
            {
                if (Radicals == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Radicals));
                Setting.SearchOptions.Radicals = value;
                OnPropertyChanged(nameof(Radicals));
            }
        }

        public bool Level
        {
            get => Setting.SearchOptions.Level;
            set
            {
                if (Level == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Level));
                Setting.SearchOptions.Level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        #endregion

        #region Search Options
        public bool LossySearch
        {
            get => Setting.LossySearch;
            set
            {
                if (LossySearch == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(LossySearch));
                Setting.LossySearch = value;
                OnPropertyChanged(nameof(LossySearch));
            }
        }
        #endregion

        #region Translate Setting

        public int TranslateChunkSize
        {
            get => Setting.TranslateChunkSize;
            set
            {
                if (TranslateChunkSize == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(TranslateChunkSize));
                Setting.TranslateChunkSize = value;
                OnPropertyChanged(nameof(TranslateChunkSize));
            }
        }

        public bool ShowCursorKanji
        {
            get => Setting.ShowCursorKanji;
            set
            {
                if (ShowCursorKanji == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(ShowCursorKanji));
                Setting.ShowCursorKanji = value;
                OnPropertyChanged(nameof(ShowCursorKanji));
            }
        }

        public int SearchDelayInMs
        {
            get => Setting.SearchDelayInMs;
            set
            {
                if (SearchDelayInMs == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SearchDelayInMs));
                Setting.SearchDelayInMs = value;
                OnPropertyChanged(nameof(SearchDelayInMs));
            }
        }

        #endregion

        #region Save Type
        public bool KanjiSave
        {
            get => Setting.SaveOption.Kanji;
            set
            {
                if (KanjiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(KanjiSave));
                Setting.SaveOption.Kanji = value;
                OnPropertyChanged(nameof(KanjiSave));
            }
        }

        public bool SinoVietnameseSave
        {
            get => Setting.SaveOption.SinoVietnamese;
            set
            {
                if (SinoVietnameseSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SinoVietnameseSave));
                Setting.SaveOption.SinoVietnamese = value;
                OnPropertyChanged(nameof(SinoVietnameseSave));
            }
        }

        public bool OnyumiSave
        {
            get => Setting.SaveOption.Onyumi;
            set
            {
                if (OnyumiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(OnyumiSave));
                Setting.SaveOption.Onyumi = value;
                OnPropertyChanged(nameof(OnyumiSave));
            }
        }

        public bool KunyumiSave
        {
            get => Setting.SaveOption.Kunyumi;
            set
            {
                if (KunyumiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(KunyumiSave));
                Setting.SaveOption.Kunyumi = value;
                OnPropertyChanged(nameof(KunyumiSave));
            }
        }


        public bool EnglishSave
        {
            get => Setting.SaveOption.English;
            set
            {
                if (EnglishSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(EnglishSave));
                Setting.SaveOption.English = value;
                OnPropertyChanged(nameof(EnglishSave));
            }
        }

        public bool VietnameseSave
        {
            get => Setting.SaveOption.Vietnamese;
            set
            {
                if (VietnameseSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(VietnameseSave));
                Setting.SaveOption.Vietnamese = value;
                OnPropertyChanged(nameof(VietnameseSave));
            }
        }

        public bool StrokesSave
        {
            get => Setting.SaveOption.Strokes;
            set
            {
                if (StrokesSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(StrokesSave));
                Setting.SaveOption.Strokes = value;
                OnPropertyChanged(nameof(StrokesSave));
            }
        }

        public bool RadicalsSave
        {
            get => Setting.SaveOption.Radicals;
            set
            {
                if (RadicalsSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(RadicalsSave));
                Setting.SaveOption.Radicals = value;
                OnPropertyChanged(nameof(RadicalsSave));
            }
        }
        public bool LevelSave
        {
            get => Setting.SaveOption.Level;
            set
            {
                if (LevelSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(LevelSave));
                Setting.SaveOption.Level = value;
                OnPropertyChanged(nameof(LevelSave));
            }
        }

        public bool ColorSave
        {
            get => Setting.SaveOption.Color;
            set
            {
                if (ColorSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(ColorSave));
                Setting.SaveOption.Color = value;
                OnPropertyChanged(nameof(ColorSave));
            }
        }


        #endregion

        #region Rando

        public int RandoSaveSaveOption
        {
            get => (int)Setting.RandoSave.SaveOption;
            set
            {

                OnPropertyChanging(nameof(RandoSaveSaveOption));
                Setting.RandoSave.SaveOption = (ERandoSaveOption)value;
                OnPropertyChanged(nameof(RandoSaveSaveOption));
            }
        }


        public int RandoSaveSaveAs
        {
            get => (int)Setting.RandoSave.SaveAsType;
            set
            {
                OnPropertyChanging(nameof(RandoSaveSaveAs));
                Setting.RandoSave.SaveAsType = (ESaveAsType)value;
                OnPropertyChanged(nameof(RandoSaveSaveAs));
            }
        }

        public int RandoAutoSaveSaveOption
        {
            get => (int)Setting.RandoAutoSave.SaveOption;
            set
            {
                OnPropertyChanging(nameof(RandoAutoSaveSaveOption));
                Setting.RandoAutoSave.SaveOption = (ERandoSaveOption)value;
                OnPropertyChanged(nameof(RandoAutoSaveSaveOption));

            }
        }

        public int RandoAutoSaveSaveAs
        {
            get => (int)Setting.RandoAutoSave.SaveAsType;
            set
            {
                OnPropertyChanging(nameof(RandoAutoSaveSaveAs));

                Setting.RandoAutoSave.SaveAsType = (ESaveAsType)value;
                OnPropertyChanged(nameof(RandoAutoSaveSaveOption));

            }
        }

        public bool RandoAutoSaveIsEnable
        {
            get => Setting.RandoAutoSave.IsEnable;
            set
            {
                OnPropertyChanging(nameof(RandoAutoSaveIsEnable));

                Setting.RandoAutoSave.IsEnable = value;
                OnPropertyChanged(nameof(RandoAutoSaveIsEnable));

            }
        }

        public string RandoAutoSaveLocation
        {
            get => Setting.RandoAutoSave.SaveLocation;
            set
            {
                OnPropertyChanging(nameof(RandoAutoSaveLocation));
                Setting.RandoAutoSave.SaveLocation = value;
                OnPropertyChanged(nameof(RandoAutoSaveLocation));

            }
        }


		#endregion

		#region Furigana

		public bool FuriganaHiragana
		{
			get => Setting.FuriganaHiragana;
			set
			{
				OnPropertyChanging(nameof(FuriganaHiragana));

				Setting.FuriganaHiragana = value;
				OnPropertyChanged(nameof(FuriganaHiragana));

			}
		}

		public bool FuriganaRomanji
		{
			get => Setting.FuriganaRomanji;
			set
			{
				OnPropertyChanging(nameof(FuriganaRomanji));
				Setting.FuriganaRomanji = value;
				OnPropertyChanged(nameof(FuriganaRomanji));

			}
		}


		public bool ConnectedRomanji
		{
			get => Setting.ConnectedRomanji;
			set
			{
				OnPropertyChanging(nameof(ConnectedRomanji));
				Setting.ConnectedRomanji = value;
				OnPropertyChanged(nameof(ConnectedRomanji));

			}
		}
		#endregion


		#region Developer
		public bool WVDeveloperMode
		{
			get => Setting.WVDeveloperMode;
			set
			{
				OnPropertyChanging(nameof(WVDeveloperMode));
				Setting.WVDeveloperMode = value;
				OnPropertyChanged(nameof(WVDeveloperMode));

			}
		}

		public bool UnsafeJapaneseAnalyzer
		{
			get => Setting.UnsafeJapaneseAnalyzer;
			set
			{
				OnPropertyChanging(nameof(UnsafeJapaneseAnalyzer));
				Setting.UnsafeJapaneseAnalyzer = value;
				OnPropertyChanged(nameof(UnsafeJapaneseAnalyzer));

			}
		}

        #endregion

        #region CopyToExcel
        public ObservableCollection<CopyToExcelOption> 
            CopyToExcelOptions { get; set; } = new(Setting.Instance.CopyToExcelOptions.Select(it => new CopyToExcelOption(it)));

		#endregion


		public void Save()
        {
            Setting.SaveSetting();
        }

        public void SetDefault()
        {
            Setting.ResetToAndSaveDefaultSetting();
        }

    }
}

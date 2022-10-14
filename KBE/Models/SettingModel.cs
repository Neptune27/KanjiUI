using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models
{
    public partial class SettingModel : ObservableObject
    {
        private Setting setting => Setting.Instance;


        public int FetchSize
        {
            get => setting.FetchSize;
            set
            {
                //if (FetchSize == value)
                //{
                //    return;
                //}
                OnPropertyChanging(nameof(FetchSize));

                setting.FetchSize = value;

                OnPropertyChanged(nameof(FetchSize));

            }
        }

        public string DatabaseConnectDirectory
        {
            get => setting.DatabaseConnectDirectory;
            set {
                if (DatabaseConnectDirectory == value)
                {
                    return;
                }
                
                OnPropertyChanging(nameof(DatabaseConnectDirectory));

                setting.DatabaseConnectDirectory = value;

                OnPropertyChanged(nameof(DatabaseConnectDirectory));

            }
        }

        #region Search Types
        public bool Kanji
        {
            get => setting.SearchOptions.Kanji;
            set
            {
                if (Kanji == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kanji));
                setting.SearchOptions.Kanji = value;
                OnPropertyChanged(nameof(Kanji));
            }
        }

        public bool SinoVietnamese
        {
            get => setting.SearchOptions.SinoVietnamese;
            set
            {
                if (SinoVietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SinoVietnamese));
                setting.SearchOptions.SinoVietnamese = value;
                OnPropertyChanged(nameof(SinoVietnamese));
            }
        }

        public bool Onyumi
        {
            get => setting.SearchOptions.Onyumi;
            set
            {
                if (Onyumi == value)
                {
                    return;
                }

                OnPropertyChanging(nameof(Onyumi));
                setting.SearchOptions.Onyumi = value;
                OnPropertyChanged(nameof(Onyumi));
            }
        }

        public bool Kunyumi
        {
            get => setting.SearchOptions.Kunyumi;
            set
            {
                if (Kunyumi == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kunyumi));
                setting.SearchOptions.Kunyumi = value;
                OnPropertyChanged(nameof(Kunyumi));

            }
        }

        public bool English
        {
            get => setting.SearchOptions.English;
            set
            {
                if (English == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(English));
                setting.SearchOptions.English = value;
                OnPropertyChanged(nameof(English));

            }
        }

        public bool Vietnamese
        {
            get => setting.SearchOptions.Vietnamese;
            set
            {
                if (Vietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Vietnamese));
                setting.SearchOptions.Vietnamese = value;
                OnPropertyChanged(nameof(Vietnamese));

            }
        }

        public bool Strokes
        {
            get => setting.SearchOptions.Strokes;
            set
            {
                if (Strokes == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Strokes));
                setting.SearchOptions.Strokes = value;
                OnPropertyChanged(nameof(Strokes));
            }
        }

        public bool Radicals
        {
            get => setting.SearchOptions.Radicals;
            set
            {
                if (Radicals == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Radicals));
                setting.SearchOptions.Radicals = value;
                OnPropertyChanged(nameof(Radicals));
            }
        }

        public bool Level
        {
            get => setting.SearchOptions.Level;
            set
            {
                if (Level == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Level));
                setting.SearchOptions.Level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        #endregion

        #region Search Options
        public bool LossySearch
        {
            get => setting.LossySearch;
            set
            {
                if (LossySearch == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(LossySearch));
                setting.LossySearch = value;
                OnPropertyChanged(nameof(LossySearch));
            }
        }
        #endregion

        #region Save Type
        public bool KanjiSave
        {
            get => setting.SaveOption.Kanji;
            set
            {
                if (KanjiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(KanjiSave));
                setting.SaveOption.Kanji = value;
                OnPropertyChanged(nameof(KanjiSave));
            }
        }

        public bool SinoVietnameseSave
        {
            get => setting.SaveOption.SinoVietnamese;
            set
            {
                if (SinoVietnameseSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SinoVietnameseSave));
                setting.SaveOption.SinoVietnamese = value;
                OnPropertyChanged(nameof(SinoVietnameseSave));
            }
        }

        public bool OnyumiSave
        {
            get => setting.SaveOption.Onyumi;
            set
            {
                if (OnyumiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(OnyumiSave));
                setting.SaveOption.Onyumi = value;
                OnPropertyChanged(nameof(OnyumiSave));
            }
        }

        public bool KunyumiSave
        {
            get => setting.SaveOption.Kunyumi;
            set
            {
                if (KunyumiSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(KunyumiSave));
                setting.SaveOption.Kunyumi = value;
                OnPropertyChanged(nameof(KunyumiSave));
            }
        }


        public bool EnglishSave
        {
            get => setting.SaveOption.English;
            set
            {
                if (EnglishSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(EnglishSave));
                setting.SaveOption.English = value;
                OnPropertyChanged(nameof(EnglishSave));
            }
        }

        public bool VietnameseSave
        {
            get => setting.SaveOption.Vietnamese;
            set
            {
                if (VietnameseSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(VietnameseSave));
                setting.SaveOption.Vietnamese = value;
                OnPropertyChanged(nameof(VietnameseSave));
            }
        }

        public bool StrokesSave
        {
            get => setting.SaveOption.Strokes;
            set
            {
                if (StrokesSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(StrokesSave));
                setting.SaveOption.Strokes = value;
                OnPropertyChanged(nameof(StrokesSave));
            }
        }

        public bool RadicalsSave
        {
            get => setting.SaveOption.Radicals;
            set
            {
                if (RadicalsSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(RadicalsSave));
                setting.SaveOption.Radicals = value;
                OnPropertyChanged(nameof(RadicalsSave));
            }
        }
        public bool LevelSave
        {
            get => setting.SaveOption.Level;
            set
            {
                if (LevelSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(LevelSave));
                setting.SaveOption.Level = value;
                OnPropertyChanged(nameof(LevelSave));
            }
        }

        public bool ColorSave
        {
            get => setting.SaveOption.Color;
            set
            {
                if (ColorSave == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(ColorSave));
                setting.SaveOption.Color = value;
                OnPropertyChanged(nameof(ColorSave));
            }
        }


        #endregion

        public void Save()
        {
            setting.SaveSetting();
        }

        public void SetDefault()
        {
            Setting.ResetToAndSaveDefaultSetting();
        }

    }
}

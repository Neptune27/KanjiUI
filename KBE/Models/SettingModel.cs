using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components;
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
        private readonly Setting setting = Setting.Instance;


        public int FetchSize
        {
            get => setting.FetchSize;
            set
            {
                if (FetchSize == value)
                {
                    return;
                }
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
            get => setting.SQLKanjiOption.Kanji;
            set
            {
                if (Kanji == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kanji));
                setting.SQLKanjiOption.Kanji = value;
                OnPropertyChanged(nameof(Kanji));
            }
        }

        public bool SinoVietnamese
        {
            get => setting.SQLKanjiOption.SinoVietnamese;
            set
            {
                if (SinoVietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(SinoVietnamese));
                setting.SQLKanjiOption.SinoVietnamese = value;
                OnPropertyChanged(nameof(SinoVietnamese));
            }
        }

        public bool Onyumi
        {
            get => setting.SQLKanjiOption.Onyumi;
            set
            {
                if (Onyumi == value)
                {
                    return;
                }

                OnPropertyChanging(nameof(Onyumi));
                setting.SQLKanjiOption.Onyumi = value;
                OnPropertyChanged(nameof(Onyumi));
            }
        }

        public bool Kunyumi
        {
            get => setting.SQLKanjiOption.Kunyumi;
            set
            {
                if (Kunyumi == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Kunyumi));
                setting.SQLKanjiOption.Kunyumi = value;
                OnPropertyChanged(nameof(Kunyumi));

            }
        }

        public bool English
        {
            get => setting.SQLKanjiOption.English;
            set
            {
                if (English == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(English));
                setting.SQLKanjiOption.English = value;
                OnPropertyChanged(nameof(English));

            }
        }

        public bool Vietnamese
        {
            get => setting.SQLKanjiOption.Vietnamese;
            set
            {
                if (Vietnamese == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Vietnamese));
                setting.SQLKanjiOption.Vietnamese = value;
                OnPropertyChanged(nameof(Vietnamese));

            }
        }

        public bool Strokes
        {
            get => setting.SQLKanjiOption.Strokes;
            set
            {
                if (Strokes == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Strokes));
                setting.SQLKanjiOption.Strokes = value;
                OnPropertyChanged(nameof(Strokes));
            }
        }

        public bool Radical
        {
            get => setting.SQLKanjiOption.Radical;
            set
            {
                if (Radical == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Radical));
                setting.SQLKanjiOption.Radical = value;
                OnPropertyChanged(nameof(Radical));
            }
        }

        public bool Level
        {
            get => setting.SQLKanjiOption.Level;
            set
            {
                if (Level == value)
                {
                    return;
                }
                OnPropertyChanging(nameof(Level));
                setting.SQLKanjiOption.Level = value;
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

        public void Save()
        {
            setting.SaveSetting();
        }

        public void SetDefault()
        {
            OnPropertyChanging(nameof(Kanji));
            Setting.ResetToAndSaveDefaultSetting();
            OnPropertyChanged(nameof(Kanji));
        }

    }
}

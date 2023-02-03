using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KBE.Components.Kanji;
using KBE.Components.Settings;
using KBE.Components.Translator;
using KBE.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanjiUI.ViewModels
{
    internal partial class TranslateViewModel : MasterDetailViewModel<KanjiWord>
    {
        private string inputText;

        public string InputText { 
            get { return inputText; } 
            set
            {
                if (inputText == value) {
                    return;
                }
                SetProperty(ref inputText, value);
                _ = TranslateText();
                _ = SearchKanjiAsync();
            } 
        }

        [ObservableProperty] 
        private string outputText;

        [ObservableProperty]
        private string fromCodeName;

        [ObservableProperty]
        private string toCodeName;



        [ObservableProperty]
        int maziiProgress = 0;

        [ObservableProperty]
        int jishoProgress = 0;

        [ObservableProperty]
        int translateProgress = 0;

        [ObservableProperty]
        private Visibility translateProgressVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility progressVisibility = Visibility.Collapsed;



        public ICommand TranslateCommand => new AsyncRelayCommand(TranslateText);
        public ICommand OpenNewWindowCommand => new RelayCommand(OpenNewWindow_CommandExecuted);

        public TranslateViewModel()
        {
            FromCodeName = Setting.Instance.TranslateFromCodeName;
            ToCodeName = Setting.Instance.TranslateToCodeName;

        }

        public Action<bool> OnTranslatorError { get; set; }

        private void OpenNewWindow_CommandExecuted()
        {

            var window = new Shell();
            window.Activate();
        }

        private async Task TranslateText()
        {


            var fromCode = typeof(LanguageCodes).GetField(FromCodeName).GetValue(null).ToString();
            var toCode = typeof(LanguageCodes).GetField(ToCodeName).GetValue(null).ToString();

            TranslateProgress = 0;
            var translateProgress = new Progress<int>(percent => TranslateProgress = percent);

            TranslateProgressVisibility = Visibility.Visible;
            try
            {
                OutputText = await GoogleTranslate.Translate(InputText, fromCode, toCode, translateProgress);
            }
            catch (TranslatorFetchSizeException)
            {
                OnTranslatorError?.Invoke(true);
            }

            TranslateProgressVisibility = Visibility.Collapsed;

            await SearchKanjiAsync();
        }

        public void TranslateSelectionChanged()
        {
            Setting.Instance.TranslateToCodeName = ToCodeName;
            Setting.Instance.TranslateFromCodeName = FromCodeName;
            Setting.Instance.SaveSetting();
        }

        private async Task SearchKanjiAsync()
        {
            var text = "";
            if (FromCodeName == "Japanese")
            {
                text = InputText;
            }
            else if (ToCodeName == "Japanese")
            {
                text = OutputText;
            }

            var maziiProgress = new Progress<int>(percent => MaziiProgress = percent);

            var jishoProgress = new Progress<int>(percent => JishoProgress = percent);

            var current = Current;
            ProgressVisibility = Visibility.Visible;


            if (await KanjiController.GetKanjiNotInDatabaseFromInternet(text, jishoProgress, maziiProgress))
            {
                WeakReferenceMessenger.Default.Send(new KanjiUpdateMessage(true));
            }

            ProgressVisibility = Visibility.Collapsed;

            JishoProgress = 0;
            MaziiProgress = 0;
        }


 

        public override bool ApplyFilter(KanjiWord item, string filter)
        {
            throw new NotImplementedException();
        }
    }
}

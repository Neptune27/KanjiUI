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
        [ObservableProperty]
        private string inputText;

		public Action<string> OnInputChanged { get; set; }

		partial void OnInputTextChanged(string value)
		{
			_ = TranslateText();
			_ = SearchKanjiAsync();
            OnInputChanged?.Invoke(value);
        }

		[ObservableProperty] 
        private string outputText;

		public Action<string> OnOutputChanged { get; set; }

		partial void OnOutputTextChanged(string value)
		{

            OnOutputChanged?.Invoke(value);
		}

		[ObservableProperty]
        private string fromCodeName;

        [ObservableProperty]
        private string toCodeName;

        [ObservableProperty]
        private string translatorSource;


        [ObservableProperty]
        int maziiProgress = 0;

        [ObservableProperty]
        int jishoProgress = 0;

        [ObservableProperty]
        int translateProgress = 0;

        [ObservableProperty]
        bool furigana = Setting.Instance.Furigana;

		partial void OnFuriganaChanged(bool value)
		{
            Setting.Instance.Furigana = value;
            Setting.Instance.SaveSetting();
		}

		[ObservableProperty]
        private Visibility translateProgressVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility progressVisibility = Visibility.Collapsed;

        public void TranslatorSourceChanged()
        {
            var value = TranslatorSource;

            if (value == "Google Translator")
            {
                ToCodeName = Setting.Instance.GoogleTranslateToCodeName;
                FromCodeName = Setting.Instance.GoogleTranslateFromCodeName;
            }
            else if (value == "DeepL Translator")
            {
                ToCodeName = Setting.Instance.DeepLTranslateToCodeName;
                FromCodeName = Setting.Instance.DeepLTranslateFromCodeName;
            }
        }

        private DeepLTranslator deepLTranslator = new();


        public ICommand TranslateCommand => new AsyncRelayCommand(TranslateText);
        public ICommand OpenNewWindowCommand => new RelayCommand(OpenNewWindow_CommandExecuted);

        

        public TranslateViewModel()
        {
            TranslatorSource = Setting.Instance.TranslateSource;
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
            Debug.WriteLine(fromCode);
            Debug.WriteLine(toCode);

            TranslateProgress = 0;
            var translateProgress = new Progress<int>(percent => TranslateProgress = percent);

            TranslateProgressVisibility = Visibility.Visible;
            try
            {
                if (TranslatorSource == "Google Translator") {
                    OutputText = await GoogleTranslate.Translate(InputText, fromCode, toCode, translateProgress);

                }
                else
                {
                    OutputText = await deepLTranslator.GetTranslation(fromCode, toCode, InputText, translateProgress);
                }
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
            if (ToCodeName == null || FromCodeName == null)
            {
                return;
            }

            if (TranslatorSource == "Google Translator")
            {
                Setting.Instance.GoogleTranslateToCodeName = ToCodeName;
                Setting.Instance.GoogleTranslateFromCodeName = FromCodeName;
            }
            else
            {
                Setting.Instance.DeepLTranslateToCodeName = ToCodeName;
                Setting.Instance.DeepLTranslateFromCodeName = FromCodeName;
            }

           
            Setting.Instance.SaveSetting();
        }

        public void FuriganaChanged()
        {
			Setting.Instance.Furigana = Furigana;
            Setting.Instance.SaveSetting();

		}

		public void TranslatorSelectionChanged()
        {
            Setting.Instance.TranslateSource = TranslatorSource;
            Setting.Instance.SaveSetting();
        }

        public void OnSwapButtonClicked()
        {
            (FromCodeName, ToCodeName) = (ToCodeName, FromCodeName);
            (InputText, OutputText) = (OutputText, InputText);

            TranslateSelectionChanged();
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

// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using KanjiUI.Utils;
using KBE.Components.Kanji;
using KBE.Components.Settings;
using KBE.Components.Translator;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TranslatePage : Page
    {
        DispatcherTimer timer = new ();
        TextBox selectedTextBox;


        public TranslatePage()
        {
			//Ensuring the singleton is created and initialized for thread-safe operations
			//Or else when async/parallel it will produce an COMException Error
			JapanesePhoneticAnalyzer.GetWords("");

			timer.Tick += SearchInHome;
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            ViewModel.OnTranslatorError = (obj) =>
            {
                if (Setting.Instance.TranslateChunkSize <= 0)
                {
                    OpenErrorDialog(title: $"Translator Fetch Size {Setting.Instance.TranslateChunkSize} is too small!",
                        content: $"Please set the translator fetch size in the option panel higher than 0!");
                }
                else
                {
                    OpenErrorDialog(title: $"Translator Fetch Size {Setting.Instance.TranslateChunkSize} is too large!",
                        content: $"Please lower the translator fetch size in the option panel!");
                }
            };

            ViewModel.OnInputChanged = OnInputChanged;
            ViewModel.OnOutputChanged = OnOutputChanged;

            PopulateToFromCB();

			var dir = Directory.GetCurrentDirectory();
			var working = Path.GetFullPath(dir + "\\" + Setting.Directory);
			WebView.Source = new Uri($"file:///{working}/Assets/Html/Translate.html");

			Ensure();


		}

        private async void OnInputChanged(string value)
        {
            InputTextBox.Text = await FuriganaHelpers.GetFuriganaTextBySetting(value, ViewModel.FromCodeName);
        }

		private async void OnOutputChanged(string value)
		{
			OutputTextBox.Text = await FuriganaHelpers.GetFuriganaTextBySetting(value, ViewModel.ToCodeName);
		}


		private void PopulateToFromCB()
        {

            var source = TranslatorComboBox.SelectedValue as string;
            source ??= Setting.Instance.TranslateSource;

            Type type;
            if (source == "Google Translator")
            {
                type = typeof(LanguageCodes);
            }
            else
            {
                type = typeof(DeepLLanguageCodes);
            }

            FromCodeComboBox.Items.Clear();
            ToCodeComboBox.Items.Clear();


            foreach (var p in type.GetFields())
            {
                FromCodeComboBox.Items.Add(p.Name);
                ToCodeComboBox.Items.Add(p.Name);
            }
        }


        private async void Ensure()
        {
            await WebView.EnsureCoreWebView2Async();
        }

        private void SearchInHome(object sender, object e)
        {
            timer.Stop();
            var textBox = selectedTextBox;
            if (textBox is null)
            {
                return;
            }

            if (textBox.SelectionLength != 0)
            {
                WeakReferenceMessenger.Default.Send(new FilterChangedMessage(textBox.Text.
                    Substring(textBox.SelectionStart, textBox.SelectionLength)
                    ));
                return;
            }

            if (textBox.SelectionStart == 0)
            {
                return;
            }
            WeakReferenceMessenger.Default.Send(new CurrentWordSelectedMesssage(textBox.Text[textBox.SelectionStart - 1].ToString()));
        }

        private async void OpenErrorDialog(string title, string content)
        {
            ErrorDialog.Title = title;
            ErrorRun.Text = content;
            await ErrorDialog.ShowAsync();
        }


        private void TranslateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.TranslateSelectionChanged();
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            selectedTextBox = sender as TextBox;
            timer.Stop();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Setting.Instance.SearchDelayInMs);
            timer.Start();
            
        }

        private void TranslatorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            PopulateToFromCB();
            ViewModel.TranslatorSelectionChanged();
            ViewModel.TranslatorSourceChanged();
        }

		private void OutputTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
            OutputTextBox.Text = ViewModel.OutputText;
		}

		private async void OutputTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (ViewModel.OutputText == OutputTextBox.Text)
			{
				return;
			}
			ViewModel.OutputText = OutputTextBox.Text;
            OutputTextBox.Text = await FuriganaHelpers.GetFuriganaTextBySetting(OutputTextBox.Text, ViewModel.ToCodeName);
		}

		private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			InputTextBox.Text = ViewModel.InputText;

		}

		private async void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
            if (ViewModel.InputText == InputTextBox.Text)
            {
                return;
            }
            ViewModel.InputText = InputTextBox.Text;
			InputTextBox.Text = await FuriganaHelpers.GetFuriganaTextBySetting(ViewModel.InputText, ViewModel.FromCodeName);

		}
	}
}

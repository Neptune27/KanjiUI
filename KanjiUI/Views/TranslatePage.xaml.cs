// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
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


            PopulateToFromCB();

            Ensure();
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
            await webView.EnsureCoreWebView2Async();
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
            WeakReferenceMessenger.Default.Send(new CurrentWordSelectedMesssage(textBox.Text[textBox.SelectionStart - 1]));
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
    }
}

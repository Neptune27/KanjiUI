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

        private bool isTriggerFilter = false;

        public TranslatePage()
        {


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

            Type type = typeof(LanguageCodes);
            foreach (var p in type.GetFields())
            {
                FromCodeComboBox.Items.Add(p.Name);
                ToCodeComboBox.Items.Add(p.Name);
            }

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
            var textBox = sender as TextBox;
            if (textBox.SelectionLength != 0) {
                isTriggerFilter = true;
                WeakReferenceMessenger.Default.Send(new FilterChangedMessage(textBox.Text.
                    Substring(textBox.SelectionStart, textBox.SelectionLength)
                    ));
            }
            else if (isTriggerFilter)
            {
                WeakReferenceMessenger.Default.Send(new FilterChangedMessage(""));
                isTriggerFilter = false;
            }
        }
    }
}

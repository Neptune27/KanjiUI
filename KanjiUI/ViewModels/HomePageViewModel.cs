using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KBE.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using KBE.Enums;
using KBE.Components.Kanji;
using KBE.Components.Utils;
using KBE.Components.Settings;
using KBE.Components.SQL;
using CommunityToolkit.Mvvm.Messaging;

namespace KanjiUI.ViewModels
{
    internal partial class HomePageViewModel : MasterDetailViewModel<KanjiWord>
    {
        private static HomePageViewModel Instance { get; set; }
        private static Setting SettingInstance { get => Setting.Instance; }

        private readonly ObservableCollection<KanjiWord> items = new();

        public override ObservableCollection<KanjiWord> Items => string.IsNullOrEmpty(filter)
        ? items
        : new ObservableCollection<KanjiWord>(items.Where(i => ApplyFilter(i, filter)));

        public ICommand OpenMaziiLinkCommand => new RelayCommand(OpenMaziiLinkCommand_Executed);
        public ICommand OpenJishoLinkCommand => new RelayCommand(OpenJishoLinkCommand_Executed);
        public ICommand ResetKanjiCommand => new AsyncRelayCommand(ResetItem);
        public ICommand DeleteKanjiCommand => new AsyncRelayCommand(DeleteItem);

        private async Task DeleteItem()
        {
            await SQLController.DeleteKanjiAsync(Current);
            items.Remove(Current);
            OnPropertyChanged(nameof(Items));
        }

        [ObservableProperty]
        int maziiProgress = 0;

        [ObservableProperty]
        int jishoProgress = 0;

        [ObservableProperty]
        private Visibility progressVisibility = Visibility.Collapsed;


        public static HomePageViewModel GetInstance()
        {
            return Instance;
        }


        public HomePageViewModel()
        {
            StartUpTask();
        }

        public async void StartUpTask()
        {
            if (items.Count == 0)
            {
                var kanjiItem = await KanjiController.GetKanjiFromDatabaseAsync();
                kanjiItem.ToList().ForEach(Items.Add);

                WeakReferenceMessenger.Default.Register<KanjiUpdateMessage>(this, (r, m) =>
                {
                    _ = RenewItems();
                });

                WeakReferenceMessenger.Default.Register<FilterChangedMessage>(this, (r, m) =>
                {
                    _ = SetFilter(m.Value);
                });
                Instance = this;
            }
        }


        public override KanjiWord UpdateItem(KanjiWord item, KanjiWord original)
        {
            KanjiController.UpdateKanji(item);
            return original.UpdateFrom(item);
        }

        public override bool ApplyFilter(KanjiWord item, string filter)
        {
            var processedFilter = KanjiProcessor.FilterProcessing(filter, new FilterProcessingOption()
            {
                IsKanjiEnable = SettingInstance.SearchOptions.Kanji,
                IsLossySearch = SettingInstance.LossySearch
            });


            return item.ApplyFilter(processedFilter, SettingInstance.SearchOptions);
        }

        public async Task SetFilter(string value)
        {
            Filter = value;
            var current = Current;
            await GetFiltered();
            OnPropertyChanged(nameof(Items));

            if (current is null)
            {
                return;
            }

            foreach (var item in Items)
            {
                if (String.Compare(current.Kanji, item.Kanji, StringComparison.Ordinal) == 0)
                {
                    current = item;
                    break;
                }
            }
            Current = current;
        }

        private async Task GetFiltered()
        {
            var maziiProgress = new Progress<int>(percent => MaziiProgress = percent);

            var jishoProgress = new Progress<int>(percent => JishoProgress = percent);

            if (string.IsNullOrEmpty(Filter))
            {
                return;
            }

            ProgressVisibility = Visibility.Visible;
            if (await KanjiController.GetKanjiNotInDatabaseFromInternet(filter, jishoProgress, maziiProgress))
            {
                await RenewItems();
            }

            MaziiProgress = 0;
            JishoProgress = 0;

            ProgressVisibility = Visibility.Collapsed;
        }

        private async Task RenewItems()
        {
            var res = await KanjiController.GetKanjiFromDatabaseAsync();
            OnPropertyChanging(nameof(Items));
            items.Clear();

            foreach (var kanji in res)
            {
                items.Add(kanji);
            }

            OnPropertyChanged(nameof(Items));
        }

        private async Task ResetItem()
        {

            var maziiProgress = new Progress<int>(percent =>
            {
                MaziiProgress = percent;
            });

            var jishoProgress = new Progress<int>(percent =>
            {
                JishoProgress = percent;
            });

            var current = Current;
            ProgressVisibility = Visibility.Visible;


            var kanjis = await KanjiController.GetKanjiFromInternet(new() { current.Kanji }, jishoProgress, maziiProgress);
            foreach (var kanji in kanjis)
            {
                current.UpdateFrom(kanji);
            }
            KanjiController.UpdateKanji(current);
            ProgressVisibility = Visibility.Collapsed;

            JishoProgress = 0;
            MaziiProgress = 0;
        }

        public void OpenMaziiLinkCommand_Executed()
        {
            OpenURL($"https://mazii.net/search/kanji?dict=javi&query={Current.Kanji}&hl=vi-VN");

        }

        public void OpenJishoLinkCommand_Executed()
        {
            OpenURL($"https://jisho.org/search/{Current.Kanji}%20%23kanji");

        }

        public async Task<EErrorType> CreateDocxCommand_Executed(string location)
        {
            DocxProcessor processor = new("Kanjis", $"{location}");

            var itemsList = Items.ToList();

            var resId = await processor.CreateKanjiDocument(itemsList, SettingInstance.SaveOption);

            Debug.WriteLine(resId);

            return resId;
        }


        public async Task<EErrorType> CreateTextCommand_Executed(string location)
        {
            TextProcessor processor = new("Kanjis", $"{location}");

            var itemsList = Items.ToList();

            var resId = await processor.CreateFile(itemsList, SettingInstance.SaveOption);

            Debug.WriteLine(resId);

            return resId;
        }

        private static void OpenURL(string url)
        {
            Process myProcess = new Process();

            try
            {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = url;
                myProcess.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Process.Start(url);
        }

        //private static void SetClipboardContent(DataPackage dataPackage)
        //{
        //    Clipboard.SetContent(dataPackage);
        //}

    }

}

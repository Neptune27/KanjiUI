using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBE.Models;
using KBE.Components;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Security.Cryptography.X509Certificates;
using Windows.ApplicationModel.DataTransfer;

namespace KanjiUI.ViewModels
{
    internal class HomePageViewModel : MasterDetailViewModel<KanjiWord>
    {
        private static HomePageViewModel Instance { get; set; }
        private static Setting SettingInstance { get; } = Setting.GetSetting();
        private static DataPackage DataPackage { get; set; } = new();


        private readonly ObservableCollection<KanjiWord> items = ShellViewModel.KanjiWords;

        public override ObservableCollection<KanjiWord> Items => filter is null
        ? items
        : new ObservableCollection<KanjiWord>(items.Where(i => ApplyFilter(i, filter)));

        public ICommand CopyMaziiLinkCommand => new RelayCommand(CopyMaziiLinkCommand_Executed);
        public ICommand CopyJishoLinkCommand => new RelayCommand(CopyJishoLinkCommand_Executed);


        public static HomePageViewModel GetInstance()
        {
            return Instance;
        }


        public HomePageViewModel()
        {
            var kanjiItem = KanjiController.GetKanjiFromDatabaseAsync().GetAwaiter().GetResult();
            kanjiItem.ToList().ForEach(i => Items.Add(i));
            Instance = this;
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
                isKanjiEnable = SettingInstance.SQLKanjiOption.Kanji,
                isLossySearch = SettingInstance.LossySearch
            });


            return item.ApplyFilter(processedFilter, SettingInstance.SQLKanjiOption);
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
                if (current.Kanji.CompareTo(item.Kanji) == 0)
                {
                    current = item;
                    break;
                }
            }

            Current = current;
        }

        private async Task GetFiltered()
        {
            if (Filter == "")
            {
                return;
            }
            if (await KanjiController.GetKanjiNotInDatabaseFromInternet(filter))
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
        }

        public void CopyMaziiLinkCommand_Executed()
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;
            DataPackage.SetText(value: $"https://mazii.net/search/kanji?dict=javi&query={Current.Kanji}&hl=vi-VN");
            SetClipboardContent(DataPackage);

        }

        public void CopyJishoLinkCommand_Executed()
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;
            DataPackage.SetText(value:$"https://jisho.org/search/{Current.Kanji}%20%23kanji");
            SetClipboardContent(DataPackage);

        }

        private static void SetClipboardContent(DataPackage dataPackage)
        {
            Clipboard.SetContent(dataPackage);
        }

    }

}

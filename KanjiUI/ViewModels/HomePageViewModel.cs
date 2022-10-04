using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBE.Models;
using KBE.Components;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace KanjiUI.ViewModels
{
    internal class HomePageViewModel : MasterDetailViewModel<KanjiWord>
    {
        private static HomePageViewModel Instance { get; set; }
        private static Setting SettingInstance { get; } = Setting.GetSetting();


        private readonly ObservableCollection<KanjiWord> items = ShellViewModel.KanjiWords;

        public override ObservableCollection<KanjiWord> Items => filter is null
        ? items
        : new ObservableCollection<KanjiWord>(items.Where(i => ApplyFilter(i, filter)));


        private async Task GetFiltered()
        {
            if (await KanjiController.GetKanjiNotInDatabaseFromInternet(filter))
            {
                var res = await KanjiController.GetKanjiFromDatabaseAsync();
                OnPropertyChanging(nameof(items));
                items.Clear();
                foreach (var kanji in res)
                {
                    items.Add(kanji);
                }
            }
        }

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


        //public new string Filter
        //{
        //    get => filter;
        //    set
        //    {
        //        var current = Current;

        //        SetProperty(ref filter, value);
        //        OnPropertyChanged(nameof(Items));

        //        if (current is not null && Items.Contains(current))
        //        {
        //            Current = current;
        //        }
        //    }
        //}

        public override KanjiWord UpdateItem(KanjiWord item, KanjiWord original)
        {
            KanjiController.UpdateKanji(item);
            return original.UpdateFrom(item);
            //var hasCurrent = HasCurrent;

            //var index = -1;
            //for (int i = 0; i < items.Count; i++)
            //{
            //    if (items[i].Kanji == original.Kanji)
            //    {
            //        index = i;
            //        break;
            //    }
            //}
            //Debug.WriteLine($"Ah {item == original}");
            //Debug.WriteLine($"Ahh {items[index] == item}");
            //var _ = Items;
            //items[index] = new KanjiWord(); // Raises CollectionChanged.

            //if (hasCurrent && !HasCurrent)
            //{
            //    // Restore Current.
            //    Current = item;
            //}

            ////KanjiController.UpdateKanji(item);

            //return item;
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
            var current = Current;


            SetProperty(ref filter, value);

            await GetFiltered();

            OnPropertyChanged(nameof(Items));

            if (current is not null && Items.Contains(current))
            {
                Current = current;
            }



        }

    }

}

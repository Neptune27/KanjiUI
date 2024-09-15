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
using KanjiUI.Views;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Numerics;

namespace KanjiUI.ViewModels
{
    internal partial class HomePageViewModel : MasterDetailViewModel<KanjiWord>
    {


        private static HomePageViewModel Instance { get; set; }
        private static Setting SettingInstance { get => Setting.Instance; }

        private readonly ObservableCollection<KanjiWord> items = [];

        private bool isLoaded = false;

        public override ObservableCollection<KanjiWord> Items => string.IsNullOrEmpty(filter?.Trim())
        ? items
        : new ObservableCollection<KanjiWord>(items.AsParallel().AsOrdered().Where(i => ApplyFilter(i, filter?.Trim())));

        public ICommand OpenMaziiLinkCommand => new RelayCommand(OpenMaziiLinkCommand_Executed);
        public ICommand OpenJishoLinkCommand => new RelayCommand(OpenJishoLinkCommand_Executed);
        public ICommand ResetKanjiCommand => new AsyncRelayCommand(ResetItem);
        public ICommand DeleteKanjiCommand => new AsyncRelayCommand(DeleteItem);

        public ICommand SendToRandoCommand => new RelayCommand(SendCurrentWordList);
        public ICommand SaveToClipboardCommand => new RelayCommand(SaveToClipboard);

        private string GenerateSaveToExcel()
        {
            var options = SettingInstance.CopyToExcelOptions.Where(it => it.IsEnable);

            var resultList = options.Select(it =>
            {
                var value = Current.GetValueOf(it.Name);
                return it.Name switch
                {
                    nameof(KanjiWord.Kunyumi) => value.Replace(" ", "、"),
					nameof(KanjiWord.Onyumi) => value.Replace(" ", "、"),
                    _ => value
				};
            });
            return string.Join("\t",resultList);
        }

        private void SaveToClipboard() 
		{
            var data = new DataPackage();

            data.SetText(GenerateSaveToExcel());

            Clipboard.SetContent(data);
		}

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
        }

        public override async Task LoadData()
        {
            if (!isLoaded)
            {
                await StartUpTask();
                isLoaded = true;
            }
        }



        public async Task StartUpTask()
        {
            if (items.Count != 0)
            {
                return;
            }



            var kanjiItem = await KanjiController.GetKanjiFromDatabaseAsync().ConfigureAwait(false);
            kanjiItem.ForEach(Items.Add);

            WeakReferenceMessenger.Default.Register<KanjiUpdateMessage>(this, (r, m) =>
            {
                _ = RenewItems();
            });

            WeakReferenceMessenger.Default.Register<FilterChangedMessage>(this, (r, m) =>
            {
                _ = SetFilter(m.Value);
            });

            WeakReferenceMessenger.Default.Register<CurrentWordSelectedMesssage>(this, (r, m) =>
            {
                _ = SelectKanjiByCursorInTranslate(m.Value);
            });

            Instance = this;

            Setting.Instance.OnDatabaseChanged += async () =>
            {
                await RenewItems();
            };
        }

        private async Task ResetFilter()
        {
            if (Items != items)
            {
                await SetFilter("");
            }
        }

        public async Task SelectKanjiByCursorInTranslate(string word)
        {
            if (SettingInstance.ShowCursorKanji)
            {
                await GetItemByCursor(word);
            }
            else
            {
                await ResetFilter();
            }

        }

        private async Task GetItemByCursor(string word)
        {
            if (word.Length != 1)
            {
                throw new IncorrectLengthError("This can only have length of 1");
            }
			Setting.Logger.Information("[Home]: Cursor: {@word}", word);

			var kanjis = KanjiProcessor.GetKanjis(word);
            if (!kanjis.Any())
            {
                await ResetFilter();
                return;
            }

            var item = Items.FirstOrDefault(k => k.Kanji == word);
            if (item is null)
            {
                await ResetFilter();
                Current = Items.FirstOrDefault(k => k.Kanji == word,
                    Items[0]);
                return;
            }

            Current = item;
        }

        public void SendCurrentWordList()
        {
            Shell.CurrentShellList.ForEach(s => s.SetContentFrame(typeof(Rando), 2));
            WeakReferenceMessenger.Default.Send(new SendKanjiMessage(Items));
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
            if (string.IsNullOrEmpty(Filter))
            {
                return;
            }


            var maziiProgress = new Progress<int>(percent => MaziiProgress = percent);

            var jishoProgress = new Progress<int>(percent => JishoProgress = percent);

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

            Setting.Logger.Information(resId.ToString());

            return resId;
        }


        public async Task<EErrorType> CreateTextCommand_Executed(string location)
        {
            TextProcessor processor = new("Kanjis", $"{location}");

            var itemsList = Items.ToList();

            var resId = await processor.CreateFile(itemsList, SettingInstance.SaveOption);
			Setting.Logger.Information(resId.ToString());


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

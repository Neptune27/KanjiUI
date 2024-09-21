using CommunityToolkit.Mvvm.Input;
using KBE.Enums;
using KBE.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.Diagnostics;
using KBE.Components.Settings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
		}



        public ICommand EditCommand => new AsyncRelayCommand(OpenEditDialog);
        public ICommand SearchCommand => new AsyncRelayCommand(OpenSearchDialog);
        public ICommand CreateDocxCommand => new AsyncRelayCommand(CreateDocxCommand_Executed);
        public ICommand CreateTextCommand => new AsyncRelayCommand(CreateTextCommand_Executed);

        public ICommand UpdateCommand => new RelayCommand(Update);
        public ICommand FilterCommand => new RelayCommand(Filter);

        private async Task OpenEditDialog()
        {
            var clone = ViewModel.Current.Clone();

            EditDialog.Title = $"Edit {ViewModel.Current.Kanji} ";
            EditDialog.PrimaryButtonCommand = UpdateCommand;
            EditDialog.DataContext = clone;
            await EditDialog.ShowAsync();
        }

        private async Task OpenSearchDialog()
        {
            FindDialog.Title = "Search";
            FindDialog.PrimaryButtonCommand = FilterCommand;
            await FindDialog.ShowAsync();
        }

        private async Task OpenErrorDialog(string title, string content)
        {
            ErrorDialog.Title = title;
            ErrorRun.Text = content;
            await ErrorDialog.ShowAsync();
        }

        private async Task CreateDocxCommand_Executed()
        {
            var file = await OpenFileSave("Word Document", ".docx");
            if (file is null)
            {
                return;
            }
			Setting.Logger.Information("File name: {@file}", file.Path);
			var resId = await ViewModel.CreateDocxCommand_Executed($"{file.Path}");
            await ShowError(resId);
        }

        private async Task CreateTextCommand_Executed()
        {
            var file = await OpenFileSave("Text Document", ".txt");
            if (file is null)
            {
                return;
            }

			Setting.Logger.Information("File name: {@file}", file.Path);
            var resId = await ViewModel.CreateTextCommand_Executed($"{file.Path}");
            await ShowError(resId);
        }


        private async Task ShowError(EErrorType resId)
        {
            if (resId == EErrorType.NORMAL)
            {
                await OpenErrorDialog("Export successfully", "File has been save successfully!");
            }
            if (resId == EErrorType.FILE_IN_USE)
            {
                await OpenErrorDialog("File is currently in use by another app", content: "Please close the file and try again.");
            }
        }

        private async Task<StorageFile> OpenFileSave(string name, string extension)
        {
            var savePicker = new FileSavePicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WindowNative.GetWindowHandle(App.Window);

            // Associate the HWND with the file picker
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Use file picker like normal!
            //folderPicker.FileTypeFilter.Add("*");
            savePicker.FileTypeChoices.Add($"{name}", new List<string>() { $"{extension}"});
            savePicker.SuggestedFileName = "New Document";

            return await savePicker.PickSaveFileAsync();
        }

        private void Update()
        {
            ViewModel.UpdateItem(EditDialog.DataContext as KanjiWord, ViewModel.Current);
        }

        private void Filter()
        {
            var _ = ViewModel.SetFilter(SearchContent.Text);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //ViewModel.Filter = args.QueryText;
            _ = ViewModel.SetFilter(args.QueryText);

        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.Filter = sender.Text;
            }

        }

        private void AutoSuggestKBA_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            AutoSuggest.Focus(FocusState.Programmatic);
        }

        private void AutoSuggest_OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

			Setting.Logger.Information("HomePage Loaded");
            await ViewModel.LoadData();
        }
    }
}

using CommunityToolkit.Mvvm.Input;
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
    }
}

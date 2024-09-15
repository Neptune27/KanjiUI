using CommunityToolkit.Mvvm.Input;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

        }

        public ICommand FileBrowserCommand => new AsyncRelayCommand(FileBrowserDialog);
        public ICommand RandoFolderBrowserCommand => new AsyncRelayCommand(RandoFolderBrowserDialog);

        private async Task RandoFolderBrowserDialog()
        {
            var file = await OpenFolderBrowser();
            if (file == null)
            {
                return;
            }

            RandoFolderDirectory.Text = file.Path;
            RandoFolderDirectory.Focus(FocusState.Pointer);
        }

        private async Task FileBrowserDialog()
        {
            var file = await OpenFileBrowser();
            if (file == null)
            {
                return;
            }

            DatabaseDirectory.Text = file.Path;
            DatabaseDirectory.Focus(FocusState.Pointer);
        }

        private async Task<StorageFile> OpenFileBrowser()
        {
            var folderPicker = new FileOpenPicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WindowNative.GetWindowHandle(App.Window);

            // Associate the HWND with the file picker
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            // Use file picker like normal!

            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFileAsync();
        }

        private async Task<StorageFolder> OpenFolderBrowser()
        {
            var folderPicker = new FolderPicker();

            var hwnd = WindowNative.GetWindowHandle(App.Window);

            InitializeWithWindow.Initialize(folderPicker, hwnd);

            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }
    }
}

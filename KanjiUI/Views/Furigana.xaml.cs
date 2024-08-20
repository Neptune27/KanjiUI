using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using DocumentFormat.OpenXml.Math;
using Windows.UI.WebUI;
using Windows.Globalization;
using KanjiUI.Utils;
using KBE.Components.Settings;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using KBE.Components.Kanji;
using KUnsafe;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Furigana : Page
{

	private readonly DispatcherTimer timer = new ();

	public Furigana()
	{
		this.InitializeComponent();
		NavigationCacheMode = NavigationCacheMode.Required;


		var dir = Directory.GetCurrentDirectory();
		var working = Path.GetFullPath(dir + "\\" + Setting.Directory);
		FuriganaWV.Source = new Uri($"file:///{working}/Assets/Html/Furigana.html");

		timer.Tick += SearchInHome;

		InitializeAsync();
	}

	private string _selectedText = string.Empty;

	private void SearchInHome(object sender, object e)
	{
		timer.Stop();

        if (string.IsNullOrWhiteSpace(_selectedText))
        {
			return;
        }

        if (_selectedText.Length == 1)
        {
	        WeakReferenceMessenger.Default.Send(new CurrentWordSelectedMesssage(_selectedText));
		}
		else
		{
			WeakReferenceMessenger.Default.Send(new FilterChangedMessage(_selectedText));
		}
	}

	public ICommand SaveAsPDFCommand => new RelayCommand(SaveAsPDF);

	private void SaveAsPDF()
	{
		FuriganaWV.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);


		//var location = await SaveHelpers.OpenFileSaveAsync("PDF", ".pdf");
		//var printSetting = FuriganaWV.CoreWebView2.Environment.CreatePrintSettings();
		//printSetting.HeaderTitle = "Furigana";
		//printSetting.FooterUri = "";
		//await FuriganaWV.CoreWebView2.PrintToPdfAsync(location.Path, printSetting);
	}

	private async void InitializeAsync()
	{
		await FuriganaWV.EnsureCoreWebView2Async(null);

		FuriganaWV.CoreWebView2.ContextMenuRequested += delegate (CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs e)
		{
			List<CoreWebView2ContextMenuItem> newMenu = [];
			foreach (var item in e.MenuItems)
			{
				switch (item.Name)
				{
					case "copy":
					case "paste":
					case "cut":
					case "reload":
					case "other":
						newMenu.Add(item);
						break;
					case "inspectElement":
						if (Setting.Instance.WVDeveloperMode)
						{
							newMenu.Add(item);
						}
						break;
					default:
						break;
				}
			}

			e.MenuItems.Clear();
			foreach (var item in newMenu)
			{
				e.MenuItems.Add(item);
			}
		};

		FuriganaWV.WebMessageReceived += delegate (WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args) 
		{
			var content = args.TryGetWebMessageAsString();
			if (content == null)
			{
				return;
			}

			_selectedText = content;
			ResetTimer();


		};
	}


	private string _current = string.Empty;

	private async void FuriganaTextBox_LostFocus(object sender, RoutedEventArgs e)
	{
        if (_current == FuriganaTextBox.Text)
        {
			return;
        }
		_current = FuriganaTextBox.Text;

		var converted = await FuriganaHelpers.ToFuriganaRomanjiHtml(FuriganaTextBox.Text);
		FuriganaWV.CoreWebView2.PostWebMessageAsString(converted);
		await ViewModel.Translate(FuriganaTextBox.Text);


	}

	private void FuriganaTextBox_SelectionChanged(object sender, RoutedEventArgs e)
	{

		var textBox = FuriganaTextBox;
		if (textBox is null)
		{
			return;
		}



		if (textBox.SelectionStart == 0)
		{
			return;
		}
		if (textBox.SelectionLength != 0)
		{
			_selectedText = textBox.Text.
				Substring(textBox.SelectionStart, textBox.SelectionLength);
		}
		else
		{
			_selectedText = textBox.Text[textBox.SelectionStart - 1].ToString();
		}


		ResetTimer();

	}

	private void ResetTimer()
	{
		timer.Stop();
		timer.Interval = new TimeSpan(0, 0, 0, 0, Setting.Instance.SearchDelayInMs);
		timer.Start();
	}
}

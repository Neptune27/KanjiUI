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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Furigana : Page
{
    public Furigana()
    {
        this.InitializeComponent();
		NavigationCacheMode = NavigationCacheMode.Required;

		//Ensuring the singleton is created and initialized for thread-safe operations
		//Or else when async/parallel it will produce an COMException Error
		JapanesePhoneticAnalyzer.GetWords("");

		var dir = Directory.GetCurrentDirectory();
		var working = Path.GetFullPath(dir + "\\"+ Setting.Directory);
		FuriganaWV.Source = new Uri($"file:///{working}/Assets/Html/Furigana.html");
        InitializeAsync();
    }
    private async void InitializeAsync()
	{
		await FuriganaWV.EnsureCoreWebView2Async(null);
	}


	private async void FuriganaTextBox_LostFocus(object sender, RoutedEventArgs e)
	{
		var converted = await FuriganaHelpers.ToFuriganaRomanjiHtml(FuriganaTextBox.Text);
		FuriganaWV.CoreWebView2.PostWebMessageAsString(converted);

	}
}
